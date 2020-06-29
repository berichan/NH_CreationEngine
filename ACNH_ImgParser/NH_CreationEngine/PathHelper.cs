﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NH_CreationEngine
{
    public static class PathHelper
    {
        // no need to edit these unless the format has changed. Correct as of 1.2.1

        public static Dictionary<string, string> Languages = new Dictionary<string, string>()
        {
            { "de" , "EUde" },
            { "en" , "USen" },
            { "es" , "EUes" },
            { "fr" , "EUfr" },
            { "it" , "EUit" },
            { "jp" , "JPja" },
            { "ko" , "KRko" },
            { "zhs" , "CNzh" },
            { "zht" , "TWzh" }
        };

        public static Dictionary<string, string> ForgeryNames = new Dictionary<string, string>()
        {
            { "de" , "fälschung" },
            { "en" , "forgery" },
            { "es" , "falsificación" },
            { "fr" , "falsification" },
            { "it" , "falso" },
            { "jp" , "偽造" },
            { "ko" , "forgery" },
            { "zhs" , "forgery" },
            { "zht" , "forgery" }
        };

        public static Dictionary<string, string> NoneNames = new Dictionary<string, string>()
        {
            { "de" , "Leer" },
            { "en" , "None" },
            { "es" , "Nada" },
            { "fr" , "Aucun" },
            { "it" , "Niente" },
            { "jp" , "無し" },
            { "ko" , "None" },
            { "zhs" , "无" },
            { "zht" , "None" }
        };

        // start chars to discard
        public static Dictionary<string, int> LangPadAmount = new Dictionary<string, int>()
        {
            { "de" , 6 }, 
            { "en" , 6 }, // char 0xCDXX?
            { "es" , 6 },
            { "fr" , 6 },
            { "it" , 6 },
            { "jp" , 0 },
            { "ko" , 0 },
            { "zhs" , 0 },
            { "zht" , 0 }
        };

        public static string OutputPath = Program.dumpPath + Path.DirectorySeparatorChar + "Output";

        /*** BCSV ***/

        // root of all bcsv
        public static string BCSVPath = Program.dumpPath + Path.DirectorySeparatorChar + "romBCSVfs" + Path.DirectorySeparatorChar + "Bcsv";

        // bcsvs of note, remember these are parsed using acnh dumper and have the csv extension
        public static string BCSVItemParamItem = BCSVPath + Path.DirectorySeparatorChar + "ItemParam.csv";
        public static string BCSVItemParamRemakeItem = BCSVPath + Path.DirectorySeparatorChar + "ItemRemake.csv";

        /*** SARCS ***/

        // root of all processed SARCs
        public static string SARCPath = Program.dumpPath + Path.DirectorySeparatorChar + "romSARCfs";

        // Message (strings) root
        public static string MsgPath = SARCPath + Path.DirectorySeparatorChar + "Message";
        // Function to get root directory of item
        public static string GetItemDirectory(string lang, string subDir = "Item", string itemType = "String") 
        { return string.Format(MsgPath + Path.DirectorySeparatorChar + 
                                    @"{2}_{0}.sarc.zs" + Path.DirectorySeparatorChar + 
                                    @"{2}_{0}.sarc" + Path.DirectorySeparatorChar + 
                                    "{1}", 
                                    lang, subDir, itemType); }
        public static string GetItemRemakeDirectory(string lang) => GetItemDirectory(lang, "Remake");
        public static string GetOutfitColorDirectory(string lang) => GetItemDirectory(lang, "Outfit") + Path.DirectorySeparatorChar + "GroupColor";
        public static string GetOutfiteNameDirectory(string lang) => GetItemDirectory(lang, "Outfit") + Path.DirectorySeparatorChar + "GroupName";
    }
}