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
        public static void CreateItemKind()
        {
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
                    extract = extract + ' ';
                if (!kinds.Contains(extract))
                    kinds.Add(extract);
            }

            kinds.Sort();
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
    }
}
