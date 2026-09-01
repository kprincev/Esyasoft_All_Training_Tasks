
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.AspNetCore.Authorization;

namespace MdmsApiCreateForDataSync.Controllers
{   
    [Route("api/[controller]")]
    [Authorize]
    [ApiController] 

    public class MeterController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public MeterController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("fetchdata")]
        public async Task<IActionResult> FetchData([FromBody] MeterRequest request)
        {
            DataTable dt = new DataTable();

            try
            {
                string jsonData = System.Text.Json.JsonSerializer.Serialize(request);

                using (SqlConnection con = new SqlConnection(
                    _configuration.GetConnectionString("db")))
                {
                    using (SqlCommand cmd = new SqlCommand("GetMeterData", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@json", jsonData);

                        con.Open();

                     
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(dt);

                       
                    }
                }
                var rows = new List<Dictionary<string, object>>();

                foreach (DataRow row in dt.Rows)
                {
                    var dict = new Dictionary<string, object>();

                    foreach (DataColumn col in dt.Columns)
                    {
                        dict[col.ColumnName] = row[col];
                    }

                    rows.Add(dict);
                }

                return Ok(rows);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}