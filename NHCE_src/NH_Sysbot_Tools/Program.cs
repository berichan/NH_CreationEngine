using NHSE.Injection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.AccessControl;
using System.Security.Permissions;
using System.Threading;

namespace NH_Sysbot_Tools
{
    class Program
    {
        /* Program for dumping certain hashmaps from RAM. The functionality here requires at least sys-botbase v1.5. 
         * Verify that sys-botbase works correctly using something else before using this function, as you may get garbage data if your offsets are wrong */

        // Don't edit these
        const string mainIconMapName = "mainIcon.berimap";
        const string sizeMapName = "size.berimap";

        // Edit these if you'd like. You'll need to put these into NH_CreationEngine/Dumpfiles. The functionality will print the map path at the end;
        static string outputMainPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

        // Edit this with your switch's IP and Socket
        public static string IP = "192.168.0.107";
        public static int Port = 6000;

        public static void Main(string[] args)
        {
            if (!checkWriteAccess())
                return;

            HashmapDumper.sb.IP = IP; HashmapDumper.sb.Port = Port;

            var dictionaryMainIcon = HashmapDumper.GetMainInventoryIconHashmap(HashmapDumper.offsetMainIcon);
            //var dictionarySize = HashmapDumper.GetMainInventoryIconHashmap(HashmapDumper.offsetSizeMap);

            string outputPathFull = outputMainPath + Path.DirectorySeparatorChar + mainIconMapName;
            writeoutDictionary(dictionaryMainIcon, outputPathFull);

            //string outputPathSize = outputMainPath + Path.DirectorySeparatorChar + sizeMapName;
            //writeoutDictionary(dictionarySize, outputPathSize);
        }

        static void writeoutDictionary<TKey, TValue>(Dictionary<TKey, TValue> dic, string outputPath)
        {
            
            using (StreamWriter file = new StreamWriter(outputPath, false))
                foreach (var entry in dic)
                    file.WriteLine("{0},{1}", entry.Key, entry.Value);

            Console.WriteLine("Wrote: " + outputPath);
        }

        static bool checkWriteAccess()
        {
            if (!Directory.Exists(outputMainPath))
                throw new Exception(string.Format("Please create {0} first, I won't do that for you so as to not destroy your filesystem.", outputMainPath));

            // check write access
            FileIOPermission f = new FileIOPermission(PermissionState.None);
            f.AllFiles = FileIOPermissionAccess.Write;
            try
            {
                f.Demand();
            }
            catch (SecurityException s)
            {
                Console.WriteLine(s);
                return false;
            }

            return true;
        }

        
    }

    public class FakeItem
    {
        ushort itemId;
        byte count;

        public FakeItem(ushort id, byte ct) { itemId = id; count = ct; }

        public byte[] GetAsBytes5()
        {
            byte[] toRet = new byte[5];
            byte[] val = BitConverter.GetBytes(itemId);
            toRet[0] = val[0];
            toRet[1] = val[1];
            toRet[2] = 0; toRet[3] = 0;
            toRet[4] = count;
            return (toRet);
        }
    }
}
