using MsbtLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NH_CreationEngine
{
    public static class ItemCreationEngine
    {
        private static char[][] knownJunkData = new char[][]
        {
            new char[] { (char)0x000E, (char)0x0032, (char)0x000A, (char)0x0010, (char)0x0006, (char)0x0064, (char)0x0065, (char)0x0020, (char)0x0006, (char)0x0064, (char)0x0027, (char)0x2060, (char)0x000E, (char)0x006E, (char)0x001E, (char)0x0000 }, // fr
            new char[] { (char)0x000A, '\0', '\0', '\0', (char)0x0002, (char)0x0073 }, // de
            new char[] { (char)0x000E, (char)0x0032, (char)0x0016 },
            new char[] { (char)0x0014, '\0', (char)0x0004, (char)0x0065, (char)0x006E, (char)0x0004, (char)0x0065, (char)0x006E, (char)0x0004, (char)0x0065, (char)0x006E },
            new char[] { (char)0x000C, '\0', '\0', '\0', (char)0x0004, (char)0x0065, (char)0x0073 },
            new char[] { (char)0x000C, '\0', '\0', (char)0x0002, (char)0x006E, (char)0x0002, (char)0x006E },
            new char[] { (char)0x000E, (char)0x006E, (char)0x001E, '\0' },
            new char[] { (char)0x000E, '\0', '\0', '\0', (char)0x0006, (char)0x0065, (char)0x006E, (char)0x0073 },
            new char[] { (char)0x000E, '\0', (char)0x0002, (char)0x006E, (char)0x0002, (char)0x006E, (char)0x0002, (char)0x006E },
            new char[] { (char)0x000E, '\0', '\0', '\0', (char)0x0006, (char)0x0073, (char)0x0065, (char)0x0073 },
        };

        private const string itemListRootName = "text_item_";
        public static void CreateItemList(string language)
        {
            string rootPath = PathHelper.GetItemDirectory(PathHelper.Languages[language]);
            List<MSBT> loadedItemsEng = new List<MSBT>(TableProcessor.LoadAllMSBTs(rootPath));
            Dictionary<int, string> ID_ItemTable = new Dictionary<int, string>(getOutfits(language)); //preload with outfits

            // Add stuff not available
            ID_ItemTable.Add(0, string.Format("({0})", PathHelper.NoneNames[language])); // nothing
            ID_ItemTable.Add(5795, "DIY recipe\r\n"); // diy recipe

            int padAmount = PathHelper.LangPadAmount[language];
            foreach (MSBT loaded in loadedItemsEng)
            {
                for (int i = 0; i < loaded.LBL1.Labels.Count; ++i)
                {
                    string keyLabel = loaded.LBL1.Labels[i].ToString();
                    if (keyLabel.keyLabelShouldBeDiscarded()) continue;

                    string[] keyVars = keyLabel.Split('_', StringSplitOptions.RemoveEmptyEntries);
                    // item name
                    string itemName = loaded.FileEncoding.GetString(loaded.LBL1.Labels[i].Value);
                    itemName = itemName.processString(keyVars[0], language, padAmount);
                    itemName += "\r\n";

                    // item index
                    string itemIndex = keyVars[1];
                    int itemNumber = int.Parse(itemIndex);
                    itemNumber += 1; // to match file line number

                    ID_ItemTable.Add(itemNumber, itemName);
                }
            }

            // how big should we make our text file?
            int largestNumber = 0;
            foreach (var kvpItem in ID_ItemTable)
                if (kvpItem.Key > largestNumber)
                    largestNumber = kvpItem.Key;
            

            // write to file using empties where there are no items
            string[] lines = new string[largestNumber + 1];
            for (int i = 0; i < largestNumber+1; ++i)
                lines[i] = ID_ItemTable.ContainsKey(i) ? ID_ItemTable[i] : "\r\n";

            WriteOutFile(PathHelper.OutputPath, language, itemListRootName + language + ".txt", string.Join("", lines));
        }

        public static void WriteOutFile(string pathRoot, string language, string filename, string dataToWrite)
        {
            string path = pathRoot + Path.DirectorySeparatorChar + language;
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            string itemPath = path + Path.DirectorySeparatorChar + filename;
            File.WriteAllText(itemPath, dataToWrite);
            Console.WriteLine("Wrote " + itemPath);
        }

        private static Dictionary<int, string> getOutfits(string language)
        {
            string rootPath = PathHelper.GetOutfiteNameDirectory(PathHelper.Languages[language]);
            Dictionary<string, MSBT> loadedItems = TableProcessor.LoadAllMSBTs_GiveNames(rootPath);
            Dictionary<int[], string> outfitColors = getOutfitColors(language);
            Dictionary<int, string> toRet = new Dictionary<int, string>();

            int padAmount = PathHelper.LangPadAmount[language];
            foreach (var loadPair in loadedItems)
            {
                MSBT loaded = loadPair.Value;
                for (int i = 0; i < loaded.LBL1.Labels.Count; ++i)
                {
                    string keyLabel = loaded.LBL1.Labels[i].ToString();
                    if (keyLabel.keyLabelShouldBeDiscarded()) continue;
                    // item name
                    string itemName = loaded.FileEncoding.GetString(loaded.LBL1.Labels[i].Value);
                    itemName = itemName.processString(keyLabel, language, padAmount);
                    // get all possible variations
                    var itemVariations = outfitColors.Where(x => x.Key[1] == int.Parse(keyLabel));

                    foreach (var kvpVariations in itemVariations)
                    {
                        string colorValueName = kvpVariations.Value.processString(keyLabel, language, 0);
                        string variationItemName = string.Format(itemName + " ({0})", colorValueName);
                        int itemNumber = kvpVariations.Key[0];
                        itemNumber += 1; // to match file line number
                        variationItemName += "\r\n";
                        toRet.Add(itemNumber, variationItemName);
                    }
                }
            }

            return toRet;
        }

        // int array is 0: item ushort 1: accessory table val 
        private static Dictionary<int[], string> getOutfitColors(string language)
        {
            string rootPath = PathHelper.GetOutfitColorDirectory(PathHelper.Languages[language]);
            Dictionary<string, MSBT> loadedItems = TableProcessor.LoadAllMSBTs_GiveNames(rootPath);
            Dictionary<int[], string> toRet = new Dictionary<int[], string>();

            foreach (var loadPair in loadedItems)
            {
                MSBT loaded = loadPair.Value;
                for (int i = 0; i < loaded.LBL1.Labels.Count; ++i)
                {
                    string keyLabel = loaded.LBL1.Labels[i].ToString();
                    if (keyLabel.keyLabelShouldBeDiscarded()) continue;
                    string[] keyVars = keyLabel.Split('_', StringSplitOptions.RemoveEmptyEntries);
                    string ItemName = loaded.FileEncoding.GetString(loaded.LBL1.Labels[i].Value);
                    if (keyVars.Length != 3)
                        throw new Exception("This isn't an OutfitColorGroup " + loadPair.Key);
                    toRet.Add(new int[2] { int.Parse(keyVars[2]), int.Parse(keyVars[0]) }, ItemName);
                }
            }

            return toRet;
        }

        private static string processString(this string ItemName, string key, string language = "en", int padAmount = 0)
        {
            if (padAmount != 0 && ItemName.Length > padAmount)
                if (Convert.ToUInt32(ItemName[padAmount - 1]).ToString("X").StartsWith("CD")) // idk why but these paddings always hex value start with CD 
                    ItemName = ItemName.Substring(padAmount); // remove padding if any

            foreach (var junkSeq in knownJunkData)
            {
                string jSeq = new string(junkSeq);
                if (ItemName.Contains(jSeq))
                    ItemName = ItemName.Replace(jSeq, "");
            }

            //if (ItemName.Contains("Nook-Inc.-Türkishemd"))
            //    ItemName = 5.ToString();

            ItemName = ItemName.Replace("\n", "\r\n").TrimEnd('\0').Replace("\0", @"\0"); // remove trailing whitespace and garbage
            if (key.EndsWith("Fake"))
                ItemName += string.Format(" ({0})", PathHelper.ForgeryNames[language]);

            return ItemName;
        }

        private static bool keyLabelShouldBeDiscarded(this string keyLabel)
        {
            if (keyLabel.EndsWith("_pl")) return true; // don't need the plural name
            if (keyLabel.Contains("SequenceOnly")) return true; // don't need sequence

            return false;
        }
    }
}
