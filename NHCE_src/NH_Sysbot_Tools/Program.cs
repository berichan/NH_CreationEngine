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
        /* The functionality here requires at least sys-botbase v1.5. 
         * Verify that sys-botbase works correctly using something else before using this function, as you may get garbage data if your offsets are wrong */

        // Don't edit these
        const string mainIconMapName = "mainIcon.berimap";

        // Edit these if you'd like. You'll need to put these into NH_CreationEngine/Resources
        static string outputMainIconMap = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

        public static void Main(string[] args)
        {
            if (!checkWriteAccess())
                return;

            var dictionaryMainIcon = HashmapDumper.GetMainInventoryIconHashmap();

            string outputPathFull = outputMainIconMap + Path.DirectorySeparatorChar + mainIconMapName;
            using (StreamWriter file = new StreamWriter(outputPathFull, false))
                foreach (var entry in dictionaryMainIcon)
                    file.WriteLine("{0},{1}", entry.Key, entry.Value);

            Console.WriteLine("Wrote: " + outputPathFull);
        }

        static bool checkWriteAccess()
        {
            if (!Directory.Exists(outputMainIconMap))
                throw new Exception(string.Format("Please create {0} first, I won't do that for you so as to not destroy your filesystem.", outputMainIconMap));

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
