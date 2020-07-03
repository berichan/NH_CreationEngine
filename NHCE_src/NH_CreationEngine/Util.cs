using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace NH_CreationEngine
{
    public static class Util
    {

        public static void SearchAllBCSVFor(string sequence)
        {
            var rawTables = loadAllTablesRaw();
            Console.WriteLine("Search started for: " + sequence);
            foreach (var blobHash in rawTables)
                if (blobHash.Value.Contains(sequence, StringComparison.OrdinalIgnoreCase)) Console.WriteLine(sequence + " is in " + blobHash.Key);

            Console.WriteLine("Search completed");
        }

        private static Dictionary<string, string> loadAllTablesRaw()
        {
            string[] items = Directory.GetFiles(PathHelper.BCSVPath);
            string[] tableItems = items.Where(x => x.EndsWith(".csv") || x.EndsWith(".csv".ToUpper())).ToArray();
            Dictionary<string, string> toRet = new Dictionary<string, string>();
            foreach (string file in tableItems)
            {
                toRet.Add(Path.GetFileName(file), File.ReadAllText(file));
            }
            return toRet;
        }
    }
}
