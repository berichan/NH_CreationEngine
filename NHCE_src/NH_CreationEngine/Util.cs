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

        public static void PrintBigFolders(string path)
        {
            Dictionary<string, long> foldersSize = new Dictionary<string, long>();
            string[] dirs = Directory.GetDirectories(path, "", SearchOption.TopDirectoryOnly);
            foreach (string s in dirs)
                foldersSize.Add(s, DirSize(new DirectoryInfo(s)));

            var myList = foldersSize.ToList();
            myList.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));

            foreach (var info in myList)
                Console.WriteLine("{0}: {1} bytes", info.Key, info.Value);
        }

        public static long DirSize(DirectoryInfo d)
        {
            long size = 0;
            // Add file sizes.
            FileInfo[] fis = d.GetFiles();
            foreach (FileInfo fi in fis)
            {
                size += fi.Length;
            }
            // Add subdirectory sizes.
            DirectoryInfo[] dis = d.GetDirectories();
            foreach (DirectoryInfo di in dis)
            {
                size += DirSize(di);
            }
            return size;
        }
    }
}
