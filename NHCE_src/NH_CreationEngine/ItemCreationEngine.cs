using MsbtLite;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace NH_CreationEngine
{
    public static class ItemCreationEngine
    {
        private static readonly char[][] knownJunkData = new char[][]
        {
            new char[] { (char)0x000E, (char)0x0032, (char)0x000A, (char)0x0010, (char)0x0006, (char)0x0064, (char)0x0065, (char)0x0020, (char)0x0006, (char)0x0064, (char)0x0027, (char)0x2060, (char)0x000E, (char)0x006E, (char)0x001E, (char)0x0000 }, // fr
            new char[] { (char)0x000A, '\0', '\0', '\0', (char)0x0002, (char)0x0073 }, // de
            new char[] { (char)0x000E, (char)0x0032, (char)0x0016, (char)0x000A, '\0', '\0', (char)0x0002, (char)0x006e },
            new char[] { (char)0x000E, (char)0x0032, (char)0x0016 },
            new char[] { (char)0x0014, '\0', (char)0x0004, (char)0x0065, (char)0x006E, (char)0x0004, (char)0x0065, (char)0x006E, (char)0x0004, (char)0x0065, (char)0x006E },
            new char[] { (char)0x000C, '\0', '\0', '\0', (char)0x0004, (char)0x0065, (char)0x0073 },
            new char[] { (char)0x000C, '\0', '\0', (char)0x0002, (char)0x006E, (char)0x0002, (char)0x006E },
            new char[] { (char)0x000E, (char)0x006E, (char)0x001E, '\0' },
            new char[] { (char)0x000E, '\0', '\0', '\0', (char)0x0006, (char)0x0065, (char)0x006E, (char)0x0073 },
            new char[] { (char)0x000E, '\0', (char)0x0002, (char)0x006E, (char)0x0002, (char)0x006E, (char)0x0002, (char)0x006E },
            new char[] { (char)0x000E, '\0', '\0', '\0', (char)0x0006, (char)0x0073, (char)0x0065, (char)0x0073 },
            new char[] { (char)0x000A, '\0', '\0', '\0', (char)0x0002, (char)0x0027 },
            new char[] { (char)0x000E, '\0', '\0', (char)0x0008, (char)0x0002, (char)0x0004 }, // jp
            new char[] { (char)0x000E, '\0', '\0', (char)0x0010, (char)0x0006, (char)0x000C },
            new char[] { (char)0x000E, '\0', '\0', (char)0x000A, (char)0x0002, (char)0x0006 },
            new char[] { (char)0x000E, '\0', '\0', (char)0x0008, (char)0x0002, (char)0x0004 },
            new char[] { (char)0x000E, '\0', '\0', (char)0x000C, (char)0x0004, (char)0x0008 },
            new char[] { (char)0x000E, '\0', '\0', (char)0x0008, (char)0x0004, (char)0x0004 },
            new char[] { (char)0x000E, '\0', '\0', (char)0x000E, (char)0x0004, (char)0x000A },
            new char[] { (char)0x000E, '\0', '\0', (char)0x000A, (char)0x0004, (char)0x0006 },
            new char[] { (char)0x000E, '\0', '\0', (char)0x000C, (char)0x0006, (char)0x0008 },
            new char[] { (char)0x000E, '\0', '\0', (char)0x0012, (char)0x0008, (char)0x000E },
            new char[] { (char)0x000E, '\0', '\0', (char)0x0010, (char)0x0008, (char)0x000C },
            new char[] { (char)0x000E, '\0', '\0', (char)0x0006, (char)0x0002, (char)0x0002 },
            new char[] { (char)0x000E, '\0', '\0', (char)0x0014, (char)0x0008, (char)0x0010 },
            new char[] { (char)0x000E, '\0', '\0', (char)0x0012, (char)0x0006, (char)0x000E },
            new char[] { (char)0x000E, '\0', '\0', (char)0x000C, (char)0x0002, (char)0x0008 },
            new char[] { (char)0x000E, '\0', '\0', (char)0x000E, (char)0x0006, (char)0x000A },
            new char[] { (char)0x000E, '\0', '\0', (char)0x000E, (char)0x000E, (char)0x000A },
            new char[] { (char)0x000E, '\0', '\0', (char)0x0010, (char)0x0004, (char)0x000C },
            new char[] { (char)0x000E, '\0', '\0', (char)0x0010, (char)0x0004, (char)0x000C },
            new char[] { (char)0x0012, '\0', '\0', (char)0x0004, (char)0x0065, (char)0x006E, (char)0x0006 },
            new char[] { (char)0x000E, (char)0x0032, (char)0x001D, '\0' },
        };

        private const string bodyColorRootName = "text_body_color_";
        private const string bodyPartsRootName = "text_body_parts_";
        private const string fabricColorRootName = "text_fabric_color_";
        private const string fabricPartsRootName = "text_fabric_parts_";
        private const string itemListRootName = "text_item_";
        private const string villagerListRootName = "text_villager_";
        private const string villagerPhraseRootName = "text_phrase_";

        private static string[] itemLines = null;
        public static string[] ItemLines { get {
                if (itemLines == null)
                    itemLines = CreateItemList("en", false);
                return itemLines;
        } }

        private static string[] itemLinesJap = null;
        public static string[] ItemLinesJap
        {
            get
            {
                if (itemLinesJap == null)
                    itemLinesJap = CreateItemList("jp", false);
                return itemLinesJap;
            }
        }

        private static string[] bodyColorLines = null;
        public static string[] BodyColorLines
        {
            get
            {
                if (bodyColorLines == null)
                    CreateBodyFabricColorPartsList("en", false);
                return bodyColorLines;
            }
        }

        private static string[] fabricColorLines = null;
        public static string[] FabricColorLines
        {
            get
            {
                if (fabricColorLines == null)
                    CreateBodyFabricColorPartsList("en", false);
                return fabricColorLines;
            }
        }

        public static void CreateBodyFabricColorPartsList(string language, bool writeToFile = true)
        {
            MSBT[] loadedMSBTs = new MSBT[4]
            {
                TableProcessor.LoadMSBT(PathHelper.GetBodyColorNameItem(PathHelper.Languages[language])), // needs to be 0 because I'm lazy
                TableProcessor.LoadMSBT(PathHelper.GetBodyPartsNameItem(PathHelper.Languages[language])),
                TableProcessor.LoadMSBT(PathHelper.GetFabricColorNameItem(PathHelper.Languages[language])), // as above, but 2 this time
                TableProcessor.LoadMSBT(PathHelper.GetFabricPartsNameItem(PathHelper.Languages[language]))
            };
            int[][] separators = new int[][] // how much of a string we want between two separators in the msbt. "_" in this case 
            {
                new int [] {1, 2},
                new int [] {1},
                new int [] {1, 2},
                new int [] {1}
            };
            string[] filenames = new string[4] { bodyColorRootName, bodyPartsRootName, fabricColorRootName, fabricPartsRootName };

            for (int i = 0; i < loadedMSBTs.Length; ++i)
            {
                MSBT loaded = loadedMSBTs[i];
                List<string> entries = createTabbedLabelList(loaded, language, "\t", separators[i]);
                entries.Sort();
                if (writeToFile)
                    WriteOutFile(PathHelper.OutputPath, language, filenames[i] + language + ".txt", string.Join("", entries));

                if (i == 0)
                    bodyColorLines = entries.ToArray();
                if (i == 2)
                    fabricColorLines = entries.ToArray();
            }
        }
        public static void CreateVillagerList(string language)
        {
            MSBT[] loadedMSBTs = new MSBT[2]
            {
                TableProcessor.LoadMSBT(PathHelper.GetVillagerNameItem(PathHelper.Languages[language])),
                TableProcessor.LoadMSBT(PathHelper.GetVillagerNPCNameItem(PathHelper.Languages[language]))
            };

            List<string> rawEntries = new List<string>();
            foreach (MSBT loaded in loadedMSBTs)
            {
                List<string> entries = createTabbedLabelList(loaded, language);
                entries.Sort();
                rawEntries.AddRange(entries);
            }

            WriteOutFile(PathHelper.OutputPath, language, villagerListRootName + language + ".txt", string.Join("", rawEntries));
        }

        public static void CreateVillagerPhraseList(string language)
        {
            MSBT loadedMSBT = TableProcessor.LoadMSBT(PathHelper.GetVillagerNPCPhraseItem(PathHelper.Languages[language]));

            List<string> entries = createTabbedLabelList(loadedMSBT, language);
            entries.Sort();

            WriteOutFile(PathHelper.OutputPath, language, villagerPhraseRootName + language + ".txt", string.Join("", entries));
        }

        public static string[] CreateItemList(string language, bool writeToFile = true)
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
            // but first get everything we didn't find from the ItemParam table
            var table = TableProcessor.LoadTable(PathHelper.BCSVItemParamItem, (char)9, "0x54706054");
            Dictionary<int, string> itemNameJapNameDic = new Dictionary<int, string>();
            Dictionary<int, string> translationTypes = new Dictionary<int, string>();
            foreach (DataRow row in table.Rows)
            {
                string itemId = row["0x54706054"].ToString();
                int itemAsId = int.Parse(itemId) + 1;

                // get jap name
                string japName = row["0xB8CC232C"].ToString().Replace("\0", string.Empty);
                itemNameJapNameDic.Add(itemAsId, japName);

                //get type
                string type = row["0xFC275E86"].ToString().Replace("\0", string.Empty);
                translationTypes.Add(itemAsId, type);
            }

            int paramLargestNumber = itemNameJapNameDic.ElementAt(itemNameJapNameDic.Count - 1).Key;
            largestNumber = Math.Max(largestNumber, paramLargestNumber);

            // load the translation master
            string[] translationMaster = File.ReadAllLines(PathHelper.MasterTranslator);
            string[] lines = new string[largestNumber + 1];

            List<string> translationsTemp = new List<string>();
            List<string> translationsTempWithType = new List<string>();
            List<string> translationsHexes = new List<string>();

            // this can and should be cleaned up
            int translationRequests = 0;
            for (int i = 0; i < largestNumber + 1; ++i)
            {
                if (ID_ItemTable.ContainsKey(i))
                {
                    if (i < translationMaster.Length)
                    {   // the below if statement is incorrect (indexes are wrong) but I haven't had time to clean it up
                        if (i != 0 && ID_ItemTable[i].JapanesePercentage() > 0.3f && translationMaster[i - 1] != string.Empty && (language != "jp" && language != "zht" && language != "zhs" && language != "ko"))
                        {
                            // this is probably japanese in the msbt
                            string n = translationMaster[i - 1];
                            if (!n.EndsWith("(internal)", StringComparison.OrdinalIgnoreCase))
                                n += " (internal)";
                            lines[i] = n + "\r\n";
                            translationsTemp.Add(n + ", " + (i - 1).ToString("X").PadLeft(4, '0') + "  \r\n");
                            translationsHexes.Add((i - 1).ToString("X") + "\r\n");
                            translationsTempWithType.Add(n.PadRight(60, ' ') + (char)2 + itemNameJapNameDic[i].PadRight(30, ' ') + (char)2 + translationTypes[i].PadRight(25, ' ') + (char)2 + (i - 1).ToString("X").PadLeft(4, '0') + (char)2 + (i - 1).ToString().PadLeft(5, '0') + "  \r\n"); // char 2 is easier to split
                        }
                        else
                            lines[i] = ID_ItemTable[i];
                    }
                    else
                        lines[i] = ID_ItemTable[i];

                }
                else if (itemNameJapNameDic.ContainsKey(i))
                {
                    if (i < translationMaster.Length)
                    {
                        if (translationMaster[i - 1] != string.Empty && (language != "jp" && language != "zht" && language != "zhs" && language != "ko"))
                        {
                            string n = translationMaster[i - 1];
                            if (!n.EndsWith("(internal)", StringComparison.OrdinalIgnoreCase))
                                n += " (internal)";
                            lines[i] = n + "\r\n";
                            translationsTemp.Add(n + ", " + (i - 1).ToString("X").PadLeft(4, '0') + "  \r\n");
                            translationsHexes.Add((i - 1).ToString("X") + "\r\n");
                            translationsTempWithType.Add(n.PadRight(60, ' ') + (char)2 + itemNameJapNameDic[i].PadRight(30, ' ') + (char)2 + translationTypes[i].PadRight(25, ' ') + (char)2 + (i - 1).ToString("X").PadLeft(4, '0') + (char)2 + (i - 1).ToString().PadLeft(5, '0') + "  \r\n"); // char 2 is easier to split
                        }
                        else
                            lines[i] = itemNameJapNameDic[i] + "\r\n";
                    }
                    else
                        lines[i] = itemNameJapNameDic[i] + "\r\n";

                    translationRequests++;
                }
                else
                    lines[i] = "\r\n";
            }

            Console.WriteLine("{0} translation requests: {1}", language, translationRequests);

            // sort by name then type
            translationsTempWithType = translationsTempWithType.OrderBy(x => x.Split((char)2)[0]).ToList();
            translationsTempWithType = translationsTempWithType.OrderBy(x => x.Split((char)2)[2]).ToList();
            for (int i = 0; i < translationsTempWithType.Count; ++i)
                translationsTempWithType[i] = "| " + translationsTempWithType[i].Replace(((char)2).ToString(), " | ").Replace("\r\n", " |\r\n"); // github markdown

            if (writeToFile)
                WriteOutFile(PathHelper.OutputPath, language, itemListRootName + language + ".txt", string.Join("", lines));
            if (language == "en")
            {
                WriteOutFile(PathHelper.OutputPath, "strings", "InternalItemList.txt", string.Join("", translationsTemp));
                WriteOutFile(PathHelper.OutputPath, "strings", "InternalHexList.txt", string.Join("", translationsHexes));
                WriteOutFile(PathHelper.OutputPath, "strings", "InternalItemListSorted.txt", string.Join("", translationsTempWithType));
            }

            if (language == "en")
                itemLines = lines;

            return lines;
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

        //int array should be value of 2 (first and last) and we'll attach the two values
        private static List<string> createTabbedLabelList(MSBT loaded, string language = "en", string space = "\t", int[] splitEntry = null)
        {
            List<string> entries = new List<string>();
            for (int i = 0; i < loaded.LBL1.Labels.Count; ++i)
            {
                entries.Add(createTabbedLabel(loaded, i, language, space, splitEntry));
            }
            return entries;
        }

        private static string createTabbedLabel(MSBT loaded, int entry, string language = "en", string space = "\t", int[] splitEntry = null)
        {
            string keyLabel = loaded.LBL1.Labels[entry].ToString();
            string villagerName = loaded.FileEncoding.GetString(loaded.LBL1.Labels[entry].Value);
            string toUseAsKey = keyLabel;
            if (splitEntry != null)
            {
                string[] tmpSplit = keyLabel.Split('_');
                string tmp;
                if (splitEntry.Length == 1)
                    tmp = tmpSplit[splitEntry[0]];
                else
                    tmp = tmpSplit[splitEntry[0]] + "_" + tmpSplit[splitEntry[1]];

                toUseAsKey = tmp;
            }
            return toUseAsKey + space + villagerName.processString(toUseAsKey, language, PathHelper.LangPadAmount[language]) + "\r\n";
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
