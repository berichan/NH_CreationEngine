using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NHSE.Parsing;

namespace NH_CreationEngine
{
    public class BCSVHelper
    {
        public static void RedumpBCSV()
        {
            /*if (Directory.Exists(PathHelper.BCSVPath))
                Directory.Delete(PathHelper.BCSVPath);
            Directory.CreateDirectory(PathHelper.BCSVPath);

            string[] allFiles = Directory.GetFiles(PathHelper.BCSVPathRaw, "*.bcsv");

            foreach (var filePath in allFiles)
            {
                BCSV bCSV = new BCSV()
            }*/

            GameBCSVDumper.UpdateDumps(PathHelper.BCSVPathRaw, PathHelper.BCSVPath, true);
        }
    }
}
