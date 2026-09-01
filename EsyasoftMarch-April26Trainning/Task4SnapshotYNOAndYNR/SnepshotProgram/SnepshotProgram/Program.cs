using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SnepshotProgram;
using System.Linq.Expressions;

class Program
{
    public static async Task Main()
    {

        Console.WriteLine("======================SnapShot Live Service Is Start=========================");

        Dictionary<int, List<MasterData>> masterDict = new Dictionary<int, List<MasterData>>();

        using (SqlConnection con = new SqlConnection(ConfigReader.conStr))
        {
            Console.WriteLine("[Step]===>  Load Master Data For Maping  ...");
            using (SqlCommand cmd = new SqlCommand("GetMasterData", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var data = new MasterData
                        {
                            EventId = Convert.ToInt32(reader["eventid"]),
                            EventMdmId = Convert.ToInt32(reader["event_mdm_id"]),
                            IsRestoration = Convert.ToInt32(reader["IsResterotion"])
                        };

                        if (!masterDict.ContainsKey(data.EventMdmId))
                            masterDict[data.EventMdmId] = new List<MasterData>();

                        masterDict[data.EventMdmId].Add(data);
                    }
                }
            }
            Console.WriteLine("[Step]===>  Master Data Mapping Is Complate ...");   

        }
    
      
        using (SqlConnection con = new SqlConnection(ConfigReader.conStr))
        {
            con.Open();
            HashSet<int> usedTbid = new HashSet<int>();

            while (true)
            {
                Console.WriteLine("============================ BATCH START ================================");
                Console.WriteLine("[Step]===>Get Data From Source");

                DataTable batch = DatabaseServicesFunctions.GetBatch(ConfigReader.conStr);
             

                if (batch.Rows.Count == 0)
                {
                    Console.WriteLine($"[Step]===>  No More Data found Sleep {ConfigReader.SleepTime} MiliSecound ...");
                    
                   await Task.Delay(ConfigReader.SleepTime);
                    continue;
                }

                Console.WriteLine("[Step]===>  Data Rows Found For Process => "+batch.Rows.Count);

                List<DataRow> rows = batch.AsEnumerable().ToList();
                var  keys = rows
                    .Select(r => new
                    {
                        event_mdm_id = Convert.ToInt32(r["event_mdm_id"]),
                        msnid = Convert.ToInt32(r["msnid"])
                    })
                    .Distinct()
                    .ToList();

                
                var keyJson = JsonConvert.SerializeObject(keys);
                Console.WriteLine("[Step]===>  Extract The Data From YNO And YNR Table ...");
                var allData = DatabaseServicesFunctions.ExecuteSPToList("GetYnoYnrFiltered", keyJson, con);
                List<DataRow> ynoList = allData
                    .Where(x => x["SourceType"].ToString() == "Yno")
                    .ToList();

                List<DataRow> ynrList = allData
                        .Where(x => x["SourceType"].ToString() == "Ynr")
                        .ToList();

                var destList = new List<object>();
                var ynoToAdd = new List<DataRow>();
                var ynrToAdd = new List<DataRow>();
                var ynoToDelete = new List<int>();
                var ynrToDelete = new List<int>();
                Console.WriteLine("[Step]===>  Data Paring Process Start  ...");
                foreach (var row in rows)
                {
                    try
                    {



                        int tbid = Convert.ToInt32(row["tbid"]);

                        if (usedTbid.Contains(tbid))
                            continue;

                        int eventId = Convert.ToInt32(row["event_id"]);
                        int eventMdmId = Convert.ToInt32(row["event_mdm_id"]);
                        int msnid = Convert.ToInt32(row["msnid"]);
                        DateTime ts = Convert.ToDateTime(row["ts"]);

                        var events = masterDict[eventMdmId];
                        //If the Event Type Other 
                        if (events.Count == 1)
                        {
                            destList.Add(new
                            {
                                occur = HelperFunction.RowToDict(row),
                                restore = HelperFunction.RowToDict(row)
                            });

                            usedTbid.Add(tbid);
                            continue;
                        }

                        // if the Event Type is Paired 

                        var occurEvent = events.First(x => x.IsRestoration == 0).EventId;
                        var restoreEvent = events.First(x => x.IsRestoration == 1).EventId;

                        // If Event Type is Occure
                        if (eventId == occurEvent)
                        {
                            var restore = rows
                                .Where(x => !usedTbid.Contains(Convert.ToInt32(x["tbid"])) &&
                                            Convert.ToInt32(x["event_mdm_id"]) == eventMdmId &&
                                            Convert.ToInt32(x["msnid"]) == msnid &&
                                            Convert.ToInt32(x["event_id"]) == restoreEvent &&
                                            Convert.ToDateTime(x["ts"]) > ts)
                                .OrderBy(x => Convert.ToDateTime(x["ts"]))
                                .FirstOrDefault();

                            if (restore != null)
                            {
                                destList.Add(new
                                {
                                    occur = HelperFunction.RowToDict(row),
                                    restore = HelperFunction.RowToDict(restore)
                                });

                                usedTbid.Add(tbid);
                                usedTbid.Add(Convert.ToInt32(restore["tbid"]));
                                continue;
                            }

                            var restoreFromYno = ynoList
                                .Where(x =>
                                    Convert.ToInt32(x["event_mdm_id"]) == eventMdmId &&
                                    Convert.ToInt32(x["msnid"]) == msnid &&
                                    Convert.ToDateTime(x["ts"]) > ts)
                                .OrderBy(x => Convert.ToDateTime(x["ts"]))
                                .FirstOrDefault();

                            if (restoreFromYno != null)
                            {
                                destList.Add(new
                                {
                                    occur = HelperFunction.RowToDict(row),
                                    restore = HelperFunction.RowToDict(restoreFromYno)
                                });

                                ynoToDelete.Add(Convert.ToInt32(restoreFromYno["tbid"]));
                                ynoList.Remove(restoreFromYno);

                                usedTbid.Add(tbid);
                            }
                            else
                            {
                                ynrToAdd.Add(row);
                                ynrList.Add(row);

                                usedTbid.Add(tbid);
                            }
                        }


                        else if (eventId == restoreEvent)
                        {
                            var occur = rows
                                .Where(x => !usedTbid.Contains(Convert.ToInt32(x["tbid"])) &&
                                            Convert.ToInt32(x["event_mdm_id"]) == eventMdmId &&
                                            Convert.ToInt32(x["msnid"]) == msnid &&
                                            Convert.ToInt32(x["event_id"]) == occurEvent &&
                                            Convert.ToDateTime(x["ts"]) < ts)
                                .OrderByDescending(x => Convert.ToDateTime(x["ts"]))
                                .FirstOrDefault();

                            if (occur != null)
                            {
                                destList.Add(new
                                {
                                    occur = HelperFunction.RowToDict(occur),
                                    restore = HelperFunction.RowToDict(row)
                                });

                                usedTbid.Add(tbid);
                                usedTbid.Add(Convert.ToInt32(occur["tbid"]));
                                continue;
                            }

                            var occurFromYnr = ynrList
                                .Where(x =>
                                    Convert.ToInt32(x["event_mdm_id"]) == eventMdmId &&
                                    Convert.ToInt32(x["msnid"]) == msnid &&
                                    Convert.ToDateTime(x["ts"]) < ts)
                                .OrderByDescending(x => Convert.ToDateTime(x["ts"]))
                                .FirstOrDefault();

                            if (occurFromYnr != null)
                            {
                                destList.Add(new
                                {
                                    occur = HelperFunction.RowToDict(occurFromYnr),
                                    restore = HelperFunction.RowToDict(row)
                                });

                                ynrToDelete.Add(Convert.ToInt32(occurFromYnr["tbid"]));
                                ynrList.Remove(occurFromYnr);

                                usedTbid.Add(tbid);
                            }
                            else
                            {
                                ynoToAdd.Add(row);
                                ynoList.Add(row);

                                usedTbid.Add(tbid);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[Error]=> {ex.Message}");
                    }
                }
       

                

                Console.WriteLine("[Step ] Total Pair Found For Destination Count: " + destList.Count);

                if (destList.Any())
                {
                    Console.WriteLine("[Step]===>  Insert Data Into Destination Table ...");
                    var json = JsonConvert.SerializeObject(destList);
                    DatabaseServicesFunctions.insertDestination(json, con);
                }
                if(ynoToAdd.Any()|| ynrToAdd.Any()|| ynoToDelete.Any()|| ynrToDelete.Any())
                {
                    DatabaseServicesFunctions.YnoYnrInsertDelete(ynoToAdd, ynrToAdd, ynoToDelete, ynrToDelete,con);
                }
                int lastId = rows.Max(r => Convert.ToInt32(r["tbid"]));
                DatabaseServicesFunctions.UpdateBatchCounter(lastId, con);
                Console.WriteLine("[Step]===>  Batch Counter Updated To => " + lastId);
                usedTbid.Clear();
                Console.WriteLine("============================ BATCH END ================================");

            }

        }
    }
 
}
