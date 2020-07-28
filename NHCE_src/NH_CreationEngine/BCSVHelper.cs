using System;
using System.Collections.Generic;
using System.Data;
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

        public static void CreateMenuIconParam(string outPath)
        {
            ExportParam(PathHelper.BCSVItemMenuIconItem, outPath, "0x87BF00E8", "0x036E8EBE", "0x87BF00E8");
        }

        public static void ExportParam(string csvPath, string outPath, string keyrow1, string keyrow2, string rowKey)
        {
            DataTable dt = TableProcessor.LoadTable(csvPath, '\t', rowKey);
            Dictionary<string, string> paramAsText = new Dictionary<string, string>();
            int currLargestLength = 0;
            foreach (DataRow row in dt.Rows)
            {
                var r1 = "'" + row[keyrow1].ToString().Replace("\0", string.Empty) + "'";
                var r2 = "'" + row[keyrow2].ToString().Replace("\0", string.Empty) + "'";
                if (r1.Length > currLargestLength)
                    currLargestLength = r1.Length;
                paramAsText.Add(r1, r2);
            }

            using (StreamWriter file = new StreamWriter(outPath, false))
                foreach (var wf in paramAsText)
                    file.WriteLine($"\t({wf.Key.PadRight(currLargestLength, ' ')}, {wf.Value}),");
        }
    }
}
