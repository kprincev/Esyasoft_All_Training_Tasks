using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task6ReadXmlFileRealWorkingDataToJsonInDb
{
    public class HelperMethods
    {
        public static void MoveFile(string sourcePath, string targetPath)
        {
            if (File.Exists(targetPath))
            {
                File.Delete(targetPath);
            }
            File.Move(sourcePath, targetPath);
        }



        private static readonly ConcurrentDictionary<string, byte> ExistingDirectoriesCache = new ConcurrentDictionary<string, byte>();

        public static string GetDynamicDestinationPath(string baseTargetFolder, string payloadType, string fileName)
        {
            string todayDate = DateTime.Now.ToString("yyyy-MM-dd");
            string targetDirectory = Path.Combine(baseTargetFolder, payloadType, todayDate);

            // Thread-safe folder check & creation
            ExistingDirectoriesCache.GetOrAdd(targetDirectory, dir =>
            {
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                return 0;
            });

            return Path.Combine(targetDirectory, fileName);
        }
    }
}
