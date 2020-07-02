using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Threading;

namespace NH_CreationEngine
{
    public static class SpriteCreationEngine
    {

        public static void DoItemSearch()
        {
            Dictionary<string, string> ItemHexPath = new Dictionary<string, string>();

            // load the itemparam table
            var table = TableProcessor.LoadTable(PathHelper.BCSVItemParamItem, (char)9, 69); // 69 is item number, 91 is possible and 89 is always
            var menuIconTable = TableProcessor.LoadTable(PathHelper.BCSVItemMenuIconItem, (char)9, 2);

            List<string> allPossibleItemName = new List<string>(Directory.GetDirectories(PathHelper.ModelPath, "Layout_FtrIcon_*", SearchOption.TopDirectoryOnly)); // Layouts are icons
            var itemHexes = getAllItemHexes(table);
            foreach (string itemName in itemHexes)
            {
                string[] split = itemName.Split("_");
                if (split.Length == 1)
                {
                    // no variations
                }
                else
                {
                    // variations
                }
            }
        }

        private static List<string> getAllItemHexes(DataTable mainParamTable)
        {
            List<string> toReturn = new List<string>();
            // create maps of body parts and fabrics we can use
            Dictionary<string, List<string>> bodyPartMap = getHashedRemakeList(ItemCreationEngine.BodyColorLines);
            Dictionary<string, List<string>> fabricPartMap = getHashedRemakeList(ItemCreationEngine.FabricColorLines);

            foreach (DataRow row in mainParamTable.Rows)
            {
                string itemNumber = row[69].ToString().Replace("\0", string.Empty);

                if (bodyPartMap.ContainsKey(itemNumber))
                {
                    if (fabricPartMap.ContainsKey(itemNumber))
                        toReturn.AddRange(getItemNamesFromHashes(itemNumber, bodyPartMap[itemNumber], fabricPartMap[itemNumber]));
                    else
                        toReturn.AddRange(getItemNamesFromHashes(itemNumber, bodyPartMap[itemNumber]));
                }
                else
                    toReturn.Add(itemNumber);
            }

            return toReturn;
        }

        private static Dictionary<string, List<string>> getHashedRemakeList(string[] lines)
        {
            Dictionary<string, List<string>> toReturn = new Dictionary<string, List<string>>(); // string is the item id and the list is the variation number counts
            foreach (string line in lines)
            {
                string firstHalf = line.Split("\t")[0]; // don't need the language name
                string[] mappable = firstHalf.Split("_");
                mappable[0] = mappable[0].TrimStart('0');
                if (toReturn.ContainsKey(mappable[0]))
                {
                    List<string> values = toReturn[mappable[0]];
                    if (!values.Contains(mappable[1]))
                        values.Add(mappable[1]);
                }
                else
                {
                    List<string> toAdd = new List<string>();
                    toAdd.Add(mappable[1]);
                    if (mappable[1] != "0")
                        toAdd.Add("0"); // vital that we have a zero value, some of the items don't have them
                    toReturn.Add(mappable[0], toAdd);
                }
            }
            return toReturn;
        }

        private static List<string> getItemNamesFromHashes(string itemid, List<string> bodyParts, List<string> fabricParts = null)
        {
            List<string> build = new List<string>();
            if (fabricParts != null)
            {
                foreach (string b in bodyParts)
                    foreach (string f in fabricParts)
                        build.Add(string.Format("{0}_{1}_{2}", itemid, b, f));
            }
            else
                foreach (string b in bodyParts)
                    build.Add(string.Format("{0}_{1}", itemid, b));

            return build;
        }

        // functions that were used to bruteforce, are no longer required
        
        public static void DoItemSearchOld()
        {
            Dictionary<string, string> ItemHexPath = new Dictionary<string, string>();

            // load the itemparam table
            var table = TableProcessor.LoadTable(PathHelper.BCSVItemParamItem, (char)9, 69); // 69 is item number, 91 is possible and 89 is always
            var menuIconTable = TableProcessor.LoadTable(PathHelper.BCSVItemMenuIconItem, (char)9, 2);
            List<string> allPossibleItemName = new List<string>(Directory.GetDirectories(PathHelper.ModelPath, "Layout*", SearchOption.TopDirectoryOnly)); // Layouts are icons
            allPossibleItemName.RemoveAll(x => x.Contains("ClosetIcon")); // Bigger textures that aren't needed for now

            foreach (DataRow row in table.Rows)
            {
                string itemNamePossible = row[91].ToString().Replace("\0", string.Empty);
                string itemNameExists = row[89].ToString().Replace("\0", string.Empty);
                List<string> hits = new List<string>();

                if (row[69].ToString() == "2750")
                {
                    string f = "?"; // hacky breakpoint checking
                }

                // try to find this as filename
                if (itemNamePossible != string.Empty)
                    hits.AddRange(allPossibleItemName.FindAll(x => x.Contains(itemNamePossible)));
                if (itemNameExists != string.Empty)
                    hits.AddRange(allPossibleItemName.FindAll(x => x.Contains(itemNameExists)));

                //remove doubles
                //TODO

                if (hits.Count == 0)
                {
                    StringBuilder notFoundItem = new StringBuilder(row[69].ToString());
                    notFoundItem.Append(string.Format(" ({0}) ", ItemCreationEngine.ItemLines[int.Parse(row[69].ToString())+1].Split("\r\n")[0]));
                    notFoundItem.Append("Not found");
                    Console.WriteLine(notFoundItem.ToString());
                }
                else
                {
                    ItemHexPath.Add(row[69].ToString(), hits[0]);
                }
            }

            Console.WriteLine("Found {0} paths for sprites.", ItemHexPath.Count);
        }

        // previously used for bruteforcing, no longer needed
        public static string FindMenuIcon(string jap, string kind, DataTable menuIconTable, DataTable itemKindTable, List<string> paths, bool searchItemKind = true, bool reverseContains = false, int menuJapIndex = 2, int filenameIndex = 1, int itemKindNameIndex = 26, int itemKindJapIndex = 27)
        {
            if (jap == string.Empty)
                return string.Empty;

            for (int i = menuIconTable.Rows.Count - 1; i > 0; --i) // reversed foreach
            {
                DataRow row = menuIconTable.Rows[i];
                if (reverseContains ? row[menuJapIndex].ToString().Replace("\0", string.Empty).Contains(jap) : jap.Contains(row[menuJapIndex].ToString().Replace("\0", string.Empty)))
                {
                    // try to get path
                    string found = paths.Find(x => x.Contains(row[filenameIndex].ToString().Replace("\0", string.Empty)));
                    if (found != string.Empty)
                    {
                        return found;
                    }
                }
            }

            // try to split by の
            for (int i = menuIconTable.Rows.Count - 1; i > 0; --i) // reversed foreach
            {
                DataRow row = menuIconTable.Rows[i];
                if (reverseContains ? row[menuJapIndex].ToString().Replace("\0", string.Empty).Split("の")[0].Contains(jap) : jap.Contains(row[menuJapIndex].ToString().Replace("\0", string.Empty).Split("の")[0]))
                {
                    // try to get path
                    string found = paths.Find(x => x.Contains(row[filenameIndex].ToString().Replace("\0", string.Empty)));
                    if (found != string.Empty)
                    {
                        return found;
                    }
                }
            }

            // ok let's look it up in the item kind list
            if (searchItemKind)
            {
                if (ClassCreationEngine.ItemKindList == null)
                    ClassCreationEngine.CreateItemKind(false);
                List<string> ItemKindsUsed = ClassCreationEngine.ItemKindList;
                if (kind.StartsWith("Kind_")) kind = kind.Substring(5);
                foreach (DataRow row in itemKindTable.Rows)
                {
                    if (row[itemKindNameIndex].ToString().Replace("\0", string.Empty) == kind)
                    {
                        return FindMenuIcon(row[itemKindJapIndex].ToString().Replace("\0", string.Empty), kind, menuIconTable, itemKindTable, paths, false, true, menuJapIndex, filenameIndex, itemKindNameIndex, itemKindJapIndex);
                    }
                }
            }


            // nope
            return string.Empty;
        }

    }
}
