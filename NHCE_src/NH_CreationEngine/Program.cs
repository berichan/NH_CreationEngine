using System;
using System.Collections.Generic;
using System.Data;
using NHSE.Core;
using MsbtLite;
using System.IO;

namespace NH_CreationEngine
{
    class Program
    {
        //edit this
        public const string dumpPath = @"D:\Switch\ACNH\Unpackv2\patched_acnh_3_0_0\"; // should have your romBCSVfs and romSARCfs in it. Use https://github.com/berichan/ACNH_Dumper

        // Requires you to dump your own menu icon map using sysbot on your switch using the other project in this solution (NH_Sysbot_Tools) and move the berimap file it creates into /NH_CreationEngine/Dumpfiles/mainIcon.berimap 
        // once you get your items filtered you may use my fork of switch toolbox to batch convert textures, this preserves filenames (untick both top options, one of them is creating new folder which you don't need) https://github.com/berichan/Switch-Toolbox
        static void Main(string[] args)
        {
            //ShowNMTFile();
            //Util.SearchAllBCSVFor("FencePegRope");
            //Util.PrintBigFolders(PathHelper.ModelPath);

            //SpriteCreationEngine.GenerateMenuIconList();
            //SpriteParser.DumpImagesToSingleFile(@"D:\Switch\ACNH\Unpackv2\patched_acnh_2_0_0\Output\Sprites\Spritesbfres_menu\proc", @"D:\Switch\ACNH\Unpackv2\patched_acnh_2_0_0\Output\Sprites\imagedump_menu.dmp");

            SpriteCreationEngine.DoItemSearch();
            //SpriteParser.DumpImagesToSingleFile(@"D:\Switch\ACNH\Unpackv2\Images_Master", @"D:\Switch\ACNH\Unpackv2\patched_acnh_3_0_0\Output\Sprites\imagedump.dmp");
            //SpriteParser.DumpImagesToSingleFile(@"C:\Users\Strawberry\Documents\clean\NHSE\NHSE.Sprites\Resources\Villagers", @"D:\Switch\ACNH\Unpackv2\patched_acnh_2_0_0\Output\Sprites\villagerdump.dmp");

            //ModelCreationEngine.DoItemSearchUnitIcon();

            //PointerCreationEngine.GenerateUnitIconPointer();
            //SpriteParser.DumpImagesToSingleFile(@"D:\Switch\ACNH\Unpackv2\Images_UnitIcon_Master", @"D:\Switch\ACNH\Unpackv2\patched_acnh_1_4_0\Output\Sprites\imagedump_manual.dmp");

            //ClassCreationEngine.CreateReaction();

            //var watch = System.Diagnostics.Stopwatch.StartNew();
            //BCSVHelper.RedumpBCSV();
            //BCSVHelper.CreateMenuIconParam(@"D:\Switch\ACNH\Unpackv2\patched_acnh_2_0_0\romBCSVfs\Bcsv\text\menuicon.txt");
            //DoEverything();
            //watch.Stop();
            //Console.WriteLine(string.Format("All files written in {0}ms", watch.ElapsedMilliseconds));
            //FindUntakeableDIYs();

            //ConvertVHouses(@"C:\Users\Strawberry\Downloads\NHSE-4cad6f589437a687c854f5e91dfabfd399687e49\NHSE-4cad6f589437a687c854f5e91dfabfd399687e49\NHSE.Villagers\Resources\Houses", @"C:\Users\Strawberry\Downloads\NHSE-4cad6f589437a687c854f5e91dfabfd399687e49\NHSE-4cad6f589437a687c854f5e91dfabfd399687e49\NHSE.Villagers\Resources\Houses\fix");
        }

        static void DoEverything()
        {
            foreach (string k in PathHelper.Languages.Keys)
            {
                ItemCreationEngine.CreateItemList(k);
                ItemCreationEngine.CreateVillagerList(k);
                ItemCreationEngine.CreateBodyFabricColorPartsList(k);
                ItemCreationEngine.CreateVillagerPhraseList(k);
            }

            ClassCreationEngine.CreateItemKind();
            ClassCreationEngine.CreateCustomColor();
            ClassCreationEngine.CreateRCP();
            ClassCreationEngine.CreateRCPC();
            ClassCreationEngine.CreateRemakeInfoData();
            ClassCreationEngine.CreateRemakeUtil();
            ClassCreationEngine.CreateRecipeUtil();
            ClassCreationEngine.CreateMenuIcon();
        }

        static void FindUntakeableDIYs()
        {
            var table = TableProcessor.LoadTable(PathHelper.BCSVRecipeItem, (char)9, 21);
            string lines = string.Empty;
            var recipeItems = new List<Item>();
            foreach (DataRow row in table.Rows)
            {
                string extract = row["0x3CCCA419"].ToString().Replace("\0", string.Empty);
                if (extract == "0")
                    continue;
                string extractID = row["0x54706054"].ToString();
                ushort recipeID = ushort.Parse(extractID);
                var recipe = new Item(Item.DIYRecipe);
                recipe.Count = recipeID;
                recipeItems.Add(recipe);

                var recipeName = GameInfo.Strings.GetItemName(recipe).Replace("(DIY recipe) - ", string.Empty);
                lines += recipeName + "\r\n";
            }

            ItemArrayEditor<Item> its = new ItemArrayEditor<Item>(recipeItems);
            File.WriteAllText(PathHelper.OutputPath + Path.DirectorySeparatorChar + "BadDIYs.txt", lines);
            File.WriteAllBytes(PathHelper.OutputPath + Path.DirectorySeparatorChar + "BadDIYs.nhi", its.Write());
        }

        static void ConvertVillagers(string inPath, string outPath)
        {
            if (!Directory.Exists(outPath))
                Directory.CreateDirectory(outPath);

            DirectoryInfo di = new DirectoryInfo(inPath);
            FileInfo[] files = di.GetFiles();

            foreach (var file in files)
            {
                if (file.Name.EndsWith("V.bytes"))
                {
                    var villager = File.ReadAllBytes(file.FullName);
                    Villager1 v1 = new Villager1(villager);
                    Villager2 v2 = new Villager2(VillagerConverter.Convert12(villager));
                    File.WriteAllBytes(Path.Combine(outPath, file.Name), v2.Data);
                }
            }
        }

        static void ConvertVHouses(string inPath, string outPath, string ext = ".bin")
        {
            if (!Directory.Exists(outPath))
                Directory.CreateDirectory(outPath);

            DirectoryInfo di = new DirectoryInfo(inPath);
            FileInfo[] files = di.GetFiles();

            int expect = VillagerHouse2.SIZE;

            foreach (var file in files)
            {
                var house = File.ReadAllBytes(file.FullName);
                VillagerHouse1 vh1 = new VillagerHouse1(house);
                VillagerHouse2 vh2 = new VillagerHouse2(VillagerHouseConverter.GetCompatible(vh1.Data, expect));
                File.WriteAllBytes(Path.Combine(outPath, Path.GetFileNameWithoutExtension(file.Name) + ext), vh2.Data);
            }
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

            DataRow nmt = table.Rows.Find("5851");
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

        class ItemData
        {
            public string name, color, hex, diy;
        }

        static void CreateMarkdownTable(string outputPath)
        {
            var itemList = NHSE.Core.GameInfo.Strings.itemlist;
            List<ItemData> items = new List<ItemData>();

            foreach (var it in itemList)
            {
                if (!string.IsNullOrWhiteSpace(it))
                {
                    ItemData i = new ItemData();
                    if (it.Contains('(') && (!it.StartsWith('(')))
                    {

                    }
                }
            }
        }
    }
}
