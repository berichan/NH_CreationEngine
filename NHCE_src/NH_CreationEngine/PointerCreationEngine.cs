using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace NH_CreationEngine
{
    public static class PointerCreationEngine
    {

        public static void GenerateUnitIconPointer()
        {
            var table = TableProcessor.LoadTable(PathHelper.BCSVItemParamItem, (char)9, "0x54706054");
            var menuIconTable = TableProcessor.LoadTable(PathHelper.BCSVItemUnitIconItem, (char)9, 2);
            var unitIconMap = getUnitIconMap();
            var pointerMap = getMap(PathHelper.AdditionalFlowerMapName);

            Dictionary<string, string> itemIdPathMap = new Dictionary<string, string>();

            foreach (DataRow row in table.Rows)
            {
                string itemId = row["0x54706054"].ToString();
                ushort idDec = ushort.Parse(itemId);

                if (!pointerMap.ContainsKey(idDec.ToString("X")))
                {
                    // get the menu icon hash
                    string iconHash = row[47].ToString().Replace("\0", string.Empty);
                    int rowNum = unitIconMap[iconHash];
                    var unitIconRowNeeded = menuIconTable.Rows[rowNum];
                    string unitIconFilename = unitIconRowNeeded[4].ToString().Replace("\0", string.Empty);

                    itemIdPathMap.Add(idDec.ToString("X"), unitIconFilename);
                }
                else
                {
                    itemIdPathMap.Add(idDec.ToString("X"), pointerMap[idDec.ToString("X")]);
                }
            }

            // create pointer file
            using (StreamWriter file = new StreamWriter(PathHelper.OutputPathBeriUnitPointerFile, false))
                foreach (var wf in itemIdPathMap)
                {
                    Console.WriteLine("writing {0} as {1}.", wf.Key, wf.Value);
                    file.WriteLine("{0},{1}", wf.Key, wf.Value);
                }

            Console.WriteLine("Generated pointer file at: {0}", PathHelper.OutputPathBeriUnitPointerFile);
        }

        private static Dictionary<string, string> getMap(string path)
        {
            if (!File.Exists(path))
            {
                Console.WriteLine("[WARNING] No unit hashmap dump exists. Exiting...");
                return null;
            }

            Dictionary<string, string> toReturn = new Dictionary<string, string>();
            foreach (string line in File.ReadLines(path))
            {
                string[] lines = line.Split(",");
                if (lines.Length == 2)
                    toReturn.Add(lines[0], lines[1]);
            }
            return toReturn;
        }

        private static Dictionary<string, int> getUnitIconMap()
        {
            string unitIconHashPath = PathHelper.UnitIconDumpName;
            if (!File.Exists(unitIconHashPath))
            {
                Console.WriteLine("[WARNING] No unit icon hashmap dump exists. Exiting...");
                return null;
            }

            Dictionary<string, int> toReturn = new Dictionary<string, int>();
            foreach (string line in File.ReadLines(unitIconHashPath))
            {
                string[] lines = line.Split(",");
                if (lines.Length == 2)
                    toReturn.Add(lines[0], int.Parse(lines[1]));
            }
            return toReturn;
        }
    }
}
