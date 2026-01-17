using NHSE.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace NH_CreationEngine
{
    public static class ModelCreationEngine
    {
        [Serializable]
        public class ItemTextureVariation
        {
            public ushort ItemID;
            public byte VariationIndex;

            public ItemTextureVariation (ushort itemId, byte variation)
            {
                ItemID = itemId;
                VariationIndex = variation;
            }

            public static bool operator ==(ItemTextureVariation itv1, ItemTextureVariation itv2)
            {
                return itv1.ItemID == itv2.ItemID && itv1.VariationIndex == itv2.VariationIndex;
            }

            public static bool operator !=(ItemTextureVariation itv1, ItemTextureVariation itv2)
            {
                return itv1.ItemID != itv2.ItemID || itv1.VariationIndex != itv2.VariationIndex;
            }

            public override string ToString()
            {
                return ItemID.ToString("X") + '_' + VariationIndex.ToString("X");
            }
        }

        public static void DoItemSearchUnitIcon()
        {
            var table = TableProcessor.LoadTable(PathHelper.BCSVItemParamItem, (char)9, "0x54706054");
            var menuIconTable = TableProcessor.LoadTable(PathHelper.BCSVItemUnitIconItem, (char)9, 2);
            var unitIconMap = getUnitIconMap();

            List<string> allPossibleItemName = new List<string>(Directory.GetDirectories(PathHelper.ModelPath, "UnitIcon*", SearchOption.TopDirectoryOnly)); // UnitIcons are models

            // variations don't matter with unit icons
            Dictionary<ItemTextureVariation, string> itemIdPathMap = new Dictionary<ItemTextureVariation, string>();

            foreach (DataRow row in table.Rows)
            {
                string itemId = row["0x54706054"].ToString();

                // get the menu icon hash
                string iconHash = row[47].ToString().Replace("\0", string.Empty);
                int rowNum = unitIconMap[iconHash];
                var unitIconRowNeeded = menuIconTable.Rows[rowNum];
                string unitIconFilename = unitIconRowNeeded[5].ToString().Replace("\0", string.Empty) + ".";
                string fullPath = allPossibleItemName.Find(x => x.Contains(unitIconFilename));

                string variationNumber = unitIconRowNeeded[3].ToString();

                ItemTextureVariation toAdd = new ItemTextureVariation(ushort.Parse(itemId), byte.Parse(variationNumber));
                itemIdPathMap.Add(toAdd, fullPath);
            }

            // new pointer map
            Dictionary<string, List<ItemTextureVariation>> pointerMap = new Dictionary<string, List<ItemTextureVariation>>();
            foreach (var iPath in itemIdPathMap)
            {
                // create item name to file relation
                if (pointerMap.ContainsKey(iPath.Value))
                {
                    // add us to the list of people that want this image
                    var thisList = pointerMap[iPath.Value];
                    thisList.Add(iPath.Key);
                }
                else
                {
                    // create a relation
                    var newListWithUs = new List<ItemTextureVariation>();
                    newListWithUs.Add(iPath.Key);
                    pointerMap.Add(iPath.Value, newListWithUs);
                }
            }

            // create the files, naming them the first thing in the list
            if (Directory.Exists(PathHelper.OutputPathUnitModelsMain))
                Directory.Delete(PathHelper.OutputPathUnitModelsMain, true); // we are copying files here so this needs to go

            Directory.CreateDirectory(PathHelper.OutputPathUnitModelsMain);

            Dictionary<string, List<ItemTextureVariation>> writtenFileMap = new Dictionary<string, List<ItemTextureVariation>>();
            foreach (var pm in pointerMap)
            {
                // get the file
                if (pm.Key == string.Empty)
                    continue;
                string[] files = Directory.GetFiles(pm.Key, "*.bfres", SearchOption.AllDirectories);
                string fileWanted = new List<string>(files).Find(x => x.Contains("output.bfres", StringComparison.OrdinalIgnoreCase));
                // copy file to output directory
                string newFileName = pm.Value[0].ToString();
                string newFilePath = PathHelper.OutputPathUnitModelsMain + Path.DirectorySeparatorChar + newFileName + ".bfres";
                File.Copy(fileWanted, newFilePath);
                Console.WriteLine("Copied {0} to {1}", fileWanted, newFilePath);
                // add to map
                writtenFileMap.Add(newFileName, pm.Value);
            }

            // create pointer file
            using (StreamWriter file = new StreamWriter(PathHelper.OutputPathUnitPointerFile, false))
                foreach (var wf in writtenFileMap)
                    foreach (var s in wf.Value)
                        file.WriteLine("{0},{1}", s.ToString().Split('_')[0], wf.Key);


            Console.WriteLine("Generated pointer file at: {0}", PathHelper.OutputPathUnitPointerFile);
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
