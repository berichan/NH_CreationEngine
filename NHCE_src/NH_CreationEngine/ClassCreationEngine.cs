using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace NH_CreationEngine
{
    public static class ClassCreationEngine
    {
        private const string itemKindRootName = "ItemKind";
        private const string itemColorRootName = "ItemCustomColor";
        private const string itemRCPRootName = "ItemRemakeCommonPattern";
        private const string itemRCPCRootName = "ItemRemakeCommonPatternCategory";
        private const string itemRemakeRootName = "ItemRemakeInfoData";
        private const string itemRemakeUtilRootName = "ItemRemakeUtil";

        private const string itemKindBytesFilename = "item_kind.bin";

        // stuff that we keep because other things use it
        public static List<string> ItemKindList;

        public static void CreateRCPC() => createEnumFillerClass(1, PathHelper.BCSVItemRCPCItem, itemRCPCRootName, 2);
        public static void CreateRCP() => createEnumFillerClass(4, PathHelper.BCSVItemRCPItem, itemRCPRootName, 6, "FtrCmnFabric");
        public static void CreateCustomColor() => createEnumFillerClass(0, PathHelper.BCSVItemColorItem, itemColorRootName, 1, "", 2);

        public static void CreateItemKind()
        {
            const char pad = ' ';
            var table = TableProcessor.LoadTable(PathHelper.BCSVItemParamItem, (char)9, 69); // 80 is kind 69 is number
            string templatePath = PathHelper.GetFullTemplatePathTo(itemKindRootName);
            string outputPath = PathHelper.GetFullOutputPathTo(templatePath);
            string preClass = File.ReadAllText(templatePath);

            int tabCount = countCharsBefore(preClass, "{data}");

            List<string> kinds = new List<string>();
            foreach (DataRow row in table.Rows)
            {
                string extract = row[80].ToString();
                extract = extract.Replace("\0", string.Empty) + "\r\n";
                for (int i = 0; i < tabCount; ++i)
                    extract = extract + pad;
                if (!kinds.Contains(extract))
                    kinds.Add(extract);
            }

            kinds.Sort();
            string kindAtEnd = kinds[kinds.Count - 1].Split("\r\n")[0]; // remove trails from last item
            kinds[kinds.Count - 1] = kindAtEnd;
            preClass = replaceData(preClass, string.Join("", kinds));
            writeOutFile(outputPath, preClass);

            // keep the itemkind list but remove stuff we don't want
            ItemKindList = new List<string>();
            foreach (string s in kinds)
                ItemKindList.Add(s.Split(',')[0]);

            // create bytes data
            string[] itemLines = ItemCreationEngine.ItemLines;
            byte[] itemKindBytes = new byte[itemLines.Length];
            for (int i = 0; i < itemLines.Length; ++i)
            {
                DataRow nRow = table.Rows.Find(i.ToString());
                if (nRow != null)
                {
                    string checker = nRow[80].ToString().Replace("\0", string.Empty) + "\r\n" + "".PadRight(tabCount, pad);
                    itemKindBytes[i] = (byte)ItemKindList.IndexOf(checker);
                }
                else
                    itemKindBytes[i] = 0;
            }

            writeOutBytes(PathHelper.OutputPathBytes + Path.DirectorySeparatorChar + itemKindBytesFilename, itemKindBytes);
        }

        public static void CreateRemakeUtil()
        {
            var table = TableProcessor.LoadTable(PathHelper.BCSVItemParamItem, (char)9, 69); // 80 is kind 69 is number
            string templatePath = PathHelper.GetFullTemplatePathTo(itemRemakeUtilRootName);
            string outputPath = PathHelper.GetFullOutputPathTo(templatePath);
            string preClass = File.ReadAllText(templatePath);
            int tabCount = countCharsBefore(preClass, "{data}");

            List<string> varIndexes = new List<string>();
            foreach (DataRow row in table.Rows)
            {
                string extract = row[66].ToString(); // index of variation
                if (extract == "-1") continue;

                string extractItemId = row[69].ToString();
                string inserter = "{" + extractItemId.PadLeft(5, '0') + ", " + extract.PadLeft(4, '0') + @"}, // " + ItemCreationEngine.ItemLines[int.Parse(extractItemId) + 1];
                for (int i = 0; i < tabCount; ++i)
                    inserter = inserter + ' ';
                varIndexes.Add(inserter);
            }

            string varAtEnd = varIndexes[varIndexes.Count - 1].Split("\r\n")[0]; // remove trails from last item
            varIndexes[varIndexes.Count - 1] = varAtEnd;

            preClass = replaceData(preClass, string.Join("", varIndexes));
            writeOutFile(outputPath, preClass);
        }

        public static void CreateRemakeInfoData()
        {
            var table = TableProcessor.LoadTable(PathHelper.BCSVItemParamRemakeItem, (char)9, 20); 
            string templatePath = PathHelper.GetFullTemplatePathTo(itemRemakeRootName);
            string outputPath = PathHelper.GetFullOutputPathTo(templatePath);
            string preClass = File.ReadAllText(templatePath);

            int tabCount = countCharsBefore(preClass, "{data}");
            List<string> remakeRow = new List<string>();
            foreach (DataRow row in table.Rows)
            {
                string extract = buildDicEntryFromDataRow(row, 18, 20, 38, 22, 39, 41);
                extract = extract.Replace("\0", string.Empty);//  + "\r\n"; we don't need rn because the item list has it at the end of each entry and we use it to comment
                for (int i = 0; i < tabCount; ++i)
                    extract = extract + ' ';
                remakeRow.Add(extract);
            }

            string remakeAtEnd = remakeRow[remakeRow.Count - 1].Split("\r\n")[0]; // remove trails from last item
            remakeRow[remakeRow.Count - 1] = remakeAtEnd;
            preClass = replaceData(preClass, string.Join("", remakeRow));
            writeOutFile(outputPath, preClass);
        }

        // all ints are column numbers. sB1 = start byte 1, SB2 = start byte 2
        private static string buildDicEntryFromDataRow(DataRow row, int itemid, int index, int varCount, int sB1, int sB2, int fp0bool)
        {
            // load the only values we need
            string s_itemid = row[itemid].ToString().PadLeft(5, '0');
            string s_index = row[index].ToString().PadLeft(4, '0');
            string s_varCount = row[varCount].ToString();
            //string s_byteArrayCount = row[byteArrayCount].ToString();
            string s_fp0bool = row[fp0bool].ToString() == "1" ? "true" : "false";
            string s_bc0 = string.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}", row[sB1].tSP(), row[sB1 + 2].tSP(), row[sB1 + 4].tSP(), row[sB1 + 6].tSP(), row[sB1 + 8].tSP(), row[sB1 + 10].tSP(), row[sB1 + 12].tSP(), row[sB1 + 14].tSP());
            string s_bc1 = string.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}", row[sB1 + 1].tSP(), row[sB1 + 3].tSP(), row[sB1 + 5].tSP(), row[sB1 + 7].tSP(), row[sB1 + 9].tSP(), row[sB1 + 11].tSP(), row[sB1 + 13].tSP(), row[sB1 + 15].tSP());
            string s_fc0 = string.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}", row[sB2].tSP(), row[sB2 + 3].tSP(), row[sB2 + 5].tSP(), row[sB2 + 7].tSP(), row[sB2 + 9].tSP(), row[sB2 + 11].tSP(), row[sB2 + 13].tSP(), row[sB2 + 15].tSP());
            string s_fc1 = string.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}", row[sB2 + 1].tSP(), row[sB2 + 4].tSP(), row[sB2 + 6].tSP(), row[sB2 + 8].tSP(), row[sB2 + 10].tSP(), row[sB2 + 12].tSP(), row[sB2 + 14].tSP(), row[sB2 + 16].tSP());
            string comment = @" // " + ItemCreationEngine.ItemLines[int.Parse(s_itemid)+1];
            StringBuilder sb = new StringBuilder("{");
            sb = sb.Append(s_index).Append(", new ItemRemakeInfo(").Append(s_index).Append(", ").Append(s_itemid).Append(", ").Append(s_varCount == "255" ? "-1" : s_varCount.PadLeft(2, ' '));
            sb = sb.Append(", new byte[] {").Append(s_bc0).Append("}, new byte[] {").Append(s_bc1).Append("}, new byte[] {").Append(s_fc0).Append("}, new byte[] {").Append(s_fc1).Append("}, "); // bytes
            sb = sb.Append(s_fp0bool.PadLeft(5, ' ')).Append(")},").Append(comment);

            return sb.ToString();

        }

        private static void createEnumFillerClass(int id, string bscvPath, string filename, int itemRow, string replaceWithNothing = "", int commentRow = -1)
        {
            var table = TableProcessor.LoadTable(bscvPath, (char)9, id);
            string templatePath = PathHelper.GetFullTemplatePathTo(filename);
            string outputPath = PathHelper.GetFullOutputPathTo(templatePath);
            string preClass = File.ReadAllText(templatePath);

            int tabCount = countCharsBefore(preClass, "{data}");

            List<string> kinds = new List<string>();
            foreach (DataRow row in table.Rows)
            {
                string extract = row[itemRow].ToString();
                string extractComment = "";
                if (commentRow != -1)
                    extractComment = @" // " + row[commentRow].ToString().Replace("\0", string.Empty);

                string root = extract.Replace("\0", string.Empty);
                if (replaceWithNothing != "")
                    root = root.Replace(replaceWithNothing, string.Empty);
                extract = root + " = " + kinds.Count.ToString() + "," + extractComment + "\r\n";
                for (int i = 0; i < tabCount; ++i)
                    extract = extract + ' ';
                if (!kinds.Contains(extract))
                    kinds.Add(extract);
            }

            string kindAtEnd = kinds[kinds.Count - 1].Split("\r\n")[0]; // remove trails from last item
            kinds[kinds.Count - 1] = kindAtEnd;

            preClass = replaceData(preClass, string.Join("", kinds));

            writeOutFile(outputPath, preClass);
        }

        private static void writeOutFile(string pathToFile, string data)
        {
            string cleanPath = Path.GetDirectoryName(pathToFile);
            if (!Directory.Exists(cleanPath))
                Directory.CreateDirectory(cleanPath);
            File.WriteAllText(pathToFile, data);
            Console.WriteLine("Wrote " + pathToFile);
        }

        private static void writeOutBytes(string pathToFile, byte[] bytes)
        {
            string cleanPath = Path.GetDirectoryName(pathToFile);
            if (!Directory.Exists(cleanPath))
                Directory.CreateDirectory(cleanPath);
            File.WriteAllBytes(pathToFile, bytes);
            Console.WriteLine("Wrote " + pathToFile);
        }

        private static string replaceData(string original, params string[] data)
        {
            if (data.Length < 2)
                return original.Replace("{data}", data[0]);

            string toReturn = original;
            for (int i = 0; i < data.Length; ++i)
            {
                toReturn = toReturn.Replace("{data" + i.ToString() + "}", data[i]);
            }
            return toReturn;
        }

        private static int countCharsBefore(string original, string quantifier, char typeToCount = ' ') // tabs for correct indentation
        {
            int toRet = 0;
            int index = original.IndexOf(quantifier);
            for (int i = index-1; i > 0; --i)
            {
                if (original[i] == typeToCount)
                    toRet++;
                else
                    break;
            }
            return toRet;
        }
        private static string tSP(this object o, int padCount = 2) => o.ToString().PadLeft(padCount, '0'); // padding for object
    }
}
