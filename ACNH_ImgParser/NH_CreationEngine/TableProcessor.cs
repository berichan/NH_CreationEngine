using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using MsbtLite;

// helps to load csvs and msbts
namespace NH_CreationEngine
{
    public static class TableProcessor
    {
        public static DataTable LoadTable(string path, char splitter, string key)
        {
            DataTable dt = new DataTable();
            string[] rawValList = File.ReadAllLines(path);

            rawValList.Take(1)
                .SelectMany(x => x.Split(new[] { splitter }, StringSplitOptions.RemoveEmptyEntries))
                .ToList()
                .ForEach(x => dt.Columns.Add(x.Trim()));

            rawValList.Skip(1)
                .Select(x => x.Split(splitter))
                .ToList()
                .ForEach(line => dt.Rows.Add(line));

            if (dt.Columns.Contains(key))
                dt.PrimaryKey = new DataColumn[1] { dt.Columns[key] };

            return dt;
        }

        public static DataTable LoadTable(string path, char splitter, int key)
        {
            DataTable dt = new DataTable();
            string[] rawValList = File.ReadAllLines(path);

            rawValList.Take(1)
                .SelectMany(x => x.Split(new[] { splitter }, StringSplitOptions.RemoveEmptyEntries))
                .ToList()
                .ForEach(x => dt.Columns.Add(x.Trim()));

            rawValList.Skip(1)
                .Select(x => x.Split(splitter))
                .ToList()
                .ForEach(line => dt.Rows.Add(line));

            if (dt.Columns.Count > key-1)
                dt.PrimaryKey = new DataColumn[1] { dt.Columns[key] };

            return dt;
        }

        public static Dictionary<string, MSBT> LoadAllMSBTs_GiveNames(string rootPath)
        {
            string[] items = Directory.GetFiles(rootPath);
            string[] msbtItems = items.Where(x => x.EndsWith(".msbt") || x.EndsWith(".msbt".ToUpper())).ToArray();
            MSBT[] msbts = new MSBT[msbtItems.Length];

            for (int i = 0; i < msbtItems.Length; ++i)
            {
                msbts[i] = MSBTHelper.AttemptLoadFile(msbtItems[i]);
            }

            Dictionary<string, MSBT> toReturn = new Dictionary<string, MSBT>();

            for (int i = 0; i < msbts.Length; ++i)
            {
                toReturn.Add(Path.GetFileName(msbtItems[i]), msbts[i]);
            }

            return toReturn;
        }

        public static MSBT[] LoadAllMSBTs(string rootPath)
        {
            return LoadAllMSBTs_GiveNames(rootPath).Values.ToArray();
        }
    }
}
