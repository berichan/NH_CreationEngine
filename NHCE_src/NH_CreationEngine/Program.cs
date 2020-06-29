using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using MsbtLite;

namespace NH_CreationEngine
{
    class Program
    {
        //edit this
        public const string dumpPath = @"D:\Switch\ACNH\Unpackv2\patched_acnh_1_2_1\"; // should have your romBCSVfs and romSARCfs in it. Use https://github.com/kwsch/ACNH_Dumper


        static void Main(string[] args)
        {
            //ShowNMTFile();

            var watch = System.Diagnostics.Stopwatch.StartNew();

            //DoEverything();
            ClassCreationEngine.CreateRecipeUtil();

            watch.Stop();
            Console.WriteLine(string.Format("All files written in {0}ms", watch.ElapsedMilliseconds));
        }

        static void DoEverything()
        {
            foreach (string k in PathHelper.Languages.Keys)
            {
                ItemCreationEngine.CreateItemList(k);
                ItemCreationEngine.CreateVillagerList(k);
                ItemCreationEngine.CreateBodyFabricColorPartsList(k);
            }

            ClassCreationEngine.CreateItemKind();
            ClassCreationEngine.CreateCustomColor();
            ClassCreationEngine.CreateRCP();
            ClassCreationEngine.CreateRCPC();
            ClassCreationEngine.CreateRemakeInfoData();
            ClassCreationEngine.CreateRemakeUtil();
            ClassCreationEngine.CreateRecipeUtil();
        }

        // Example functions
        static void ShowNMTType() // show the type of an nmt in the item parameter table
        {
            var table = TableProcessor.LoadTable(PathHelper.BCSVItemParamItem, (char)9, 69); //69

            DataRow nmt = table.Rows.Find("5851");
            Console.WriteLine(nmt[80] + " " + nmt[80].ToString().Length);
        }

        static void ShowNMTFile() // shows filename of model data
        {
            var table = TableProcessor.LoadTable(PathHelper.BCSVItemParamItem, (char)9, 69);

            DataRow nmt = table.Rows.Find("6426");
            string val = nmt[91].ToString(); // or 89
            Console.WriteLine(val);
        }

        static void ShowEngItemName() // search and show the name of an nmt using msbt parser. Note the padding chars
        {
            const string toLookFor = "05851";
            string rootPath = PathHelper.GetItemDirectory(PathHelper.Languages["en"]);
            List<MSBT> loadedItemsEng = new List<MSBT>(TableProcessor.LoadAllMSBTs(rootPath));
            List<string> foundItems = new List<string>();
            foreach (MSBT loaded in loadedItemsEng)
            {
                for (int i = 0; i < loaded.LBL1.Labels.Count; ++i)
                {
                    if (loaded.LBL1.Labels[i].ToString().Contains(toLookFor))
                        foundItems.Add(loaded.LBL1.Labels[i].ToString() + ": " 
                            + loaded.FileEncoding.GetString(loaded.LBL1.Labels[i].Value).Replace("\n", "\r\n").TrimEnd('\0').Replace("\0", @"\0") + "\0");
                }
            }

            // print loaded items
            foreach (string itm in foundItems)
                Console.WriteLine(toLookFor + " is " + itm);
        }
    }
}
