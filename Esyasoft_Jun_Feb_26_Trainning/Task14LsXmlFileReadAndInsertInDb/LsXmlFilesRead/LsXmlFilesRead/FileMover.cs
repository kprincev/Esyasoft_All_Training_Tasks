using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LsXmlFilesRead
{
    public static class FileMover
    {
        public static string Move(string src, string destFolder)
        {
            string dest = Path.Combine(destFolder, Path.GetFileName(src));
            if (File.Exists(dest))
            {
                File.Delete(dest);
            }
            File.Move(src, dest);
            return dest;
        }

        public static void SafeMove(string src, string destFolder)
        {
            if (File.Exists(src))
                Move(src, destFolder);
        }
    }

}
