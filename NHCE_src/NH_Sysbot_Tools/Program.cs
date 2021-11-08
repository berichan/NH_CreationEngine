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
    /// HidKeyboardKey
    public enum HidKeyboardKey {
        HidKeyboardKey_A = 4,
        HidKeyboardKey_B = 5,
        HidKeyboardKey_C = 6,
        HidKeyboardKey_D = 7,
        HidKeyboardKey_E = 8,
        HidKeyboardKey_F = 9,
        HidKeyboardKey_G = 10,
        HidKeyboardKey_H = 11,
        HidKeyboardKey_I = 12,
        HidKeyboardKey_J = 13,
        HidKeyboardKey_K = 14,
        HidKeyboardKey_L = 15,
        HidKeyboardKey_M = 16,
        HidKeyboardKey_N = 17,
        HidKeyboardKey_O = 18,
        HidKeyboardKey_P = 19,
        HidKeyboardKey_Q = 20,
        HidKeyboardKey_R = 21,
        HidKeyboardKey_S = 22,
        HidKeyboardKey_T = 23,
        HidKeyboardKey_U = 24,
        HidKeyboardKey_V = 25,
        HidKeyboardKey_W = 26,
        HidKeyboardKey_X = 27,
        HidKeyboardKey_Y = 28,
        HidKeyboardKey_Z = 29,
        HidKeyboardKey_D1 = 30,
        HidKeyboardKey_D2 = 31,
        HidKeyboardKey_D3 = 32,
        HidKeyboardKey_D4 = 33,
        HidKeyboardKey_D5 = 34,
        HidKeyboardKey_D6 = 35,
        HidKeyboardKey_D7 = 36,
        HidKeyboardKey_D8 = 37,
        HidKeyboardKey_D9 = 38,
        HidKeyboardKey_D0 = 39,
        HidKeyboardKey_Return = 40,
        HidKeyboardKey_Escape = 41,
        HidKeyboardKey_Backspace = 42,
        HidKeyboardKey_Tab = 43,
        HidKeyboardKey_Space = 44,
        HidKeyboardKey_Minus = 45,
        HidKeyboardKey_Plus = 46,
        HidKeyboardKey_OpenBracket = 47,
        HidKeyboardKey_CloseBracket = 48,
        HidKeyboardKey_Pipe = 49,
        HidKeyboardKey_Tilde = 50,
        HidKeyboardKey_Semicolon = 51,
        HidKeyboardKey_Quote = 52,
        HidKeyboardKey_Backquote = 53,
        HidKeyboardKey_Comma = 54,
        HidKeyboardKey_Period = 55,
        HidKeyboardKey_Slash = 56,
        HidKeyboardKey_CapsLock = 57,
        HidKeyboardKey_F1 = 58,
        HidKeyboardKey_F2 = 59,
        HidKeyboardKey_F3 = 60,
        HidKeyboardKey_F4 = 61,
        HidKeyboardKey_F5 = 62,
        HidKeyboardKey_F6 = 63,
        HidKeyboardKey_F7 = 64,
        HidKeyboardKey_F8 = 65,
        HidKeyboardKey_F9 = 66,
        HidKeyboardKey_F10 = 67,
        HidKeyboardKey_F11 = 68,
        HidKeyboardKey_F12 = 69,
        HidKeyboardKey_PrintScreen = 70,
        HidKeyboardKey_ScrollLock = 71,
        HidKeyboardKey_Pause = 72,
        HidKeyboardKey_Insert = 73,
        HidKeyboardKey_Home = 74,
        HidKeyboardKey_PageUp = 75,
        HidKeyboardKey_Delete = 76,
        HidKeyboardKey_End = 77,
        HidKeyboardKey_PageDown = 78,
        HidKeyboardKey_RightArrow = 79,
        HidKeyboardKey_LeftArrow = 80,
        HidKeyboardKey_DownArrow = 81,
        HidKeyboardKey_UpArrow = 82,
        HidKeyboardKey_NumLock = 83,
        HidKeyboardKey_NumPadDivide = 84,
        HidKeyboardKey_NumPadMultiply = 85,
        HidKeyboardKey_NumPadSubtract = 86,
        HidKeyboardKey_NumPadAdd = 87,
        HidKeyboardKey_NumPadEnter = 88,
        HidKeyboardKey_NumPad1 = 89,
        HidKeyboardKey_NumPad2 = 90,
        HidKeyboardKey_NumPad3 = 91,
        HidKeyboardKey_NumPad4 = 92,
        HidKeyboardKey_NumPad5 = 93,
        HidKeyboardKey_NumPad6 = 94,
        HidKeyboardKey_NumPad7 = 95,
        HidKeyboardKey_NumPad8 = 96,
        HidKeyboardKey_NumPad9 = 97,
        HidKeyboardKey_NumPad0 = 98,
        HidKeyboardKey_NumPadDot = 99,
        HidKeyboardKey_Backslash = 100,
        HidKeyboardKey_Application = 101,
        HidKeyboardKey_Power = 102,
        HidKeyboardKey_NumPadEquals = 103,
        HidKeyboardKey_F13 = 104,
        HidKeyboardKey_F14 = 105,
        HidKeyboardKey_F15 = 106,
        HidKeyboardKey_F16 = 107,
        HidKeyboardKey_F17 = 108,
        HidKeyboardKey_F18 = 109,
        HidKeyboardKey_F19 = 110,
        HidKeyboardKey_F20 = 111,
        HidKeyboardKey_F21 = 112,
        HidKeyboardKey_F22 = 113,
        HidKeyboardKey_F23 = 114,
        HidKeyboardKey_F24 = 115,
        HidKeyboardKey_NumPadComma = 133,
        HidKeyboardKey_Ro = 135,
        HidKeyboardKey_KatakanaHiragana = 136,
        HidKeyboardKey_Yen = 137,
        HidKeyboardKey_Henkan = 138,
        HidKeyboardKey_Muhenkan = 139,
        HidKeyboardKey_NumPadCommaPc98 = 140,
        HidKeyboardKey_HangulEnglish = 144,
        HidKeyboardKey_Hanja = 145,
        HidKeyboardKey_Katakana = 146,
        HidKeyboardKey_Hiragana = 147,
        HidKeyboardKey_ZenkakuHankaku = 148,
        HidKeyboardKey_LeftControl = 224,
        HidKeyboardKey_LeftShift = 225,
        HidKeyboardKey_LeftAlt = 226,
        HidKeyboardKey_LeftGui = 227,
        HidKeyboardKey_RightControl = 228,
        HidKeyboardKey_RightShift = 229,
        HidKeyboardKey_RightAlt = 230,
        HidKeyboardKey_RightGui = 231,
    }
    class Program
    {
        /* Program for dumping certain hashmaps from RAM. The functionality here requires at least sys-botbase v1.5. 
         * Verify that sys-botbase works correctly using something else before using this function, as you may get garbage data if your offsets are wrong */

        // Don't edit these
        const string mainIconMapName = "mainIcon.berimap";
        const string sizeMapName = "size.berimap";
        const string unitIconMapName = "unitIcon.berimap";

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

            //var dictionaryMainIcon = HashmapDumper.GetMainInventoryIconHashmap(HashmapDumper.offsetMainIcon);
            //string outputPathFull = outputMainPath + Path.DirectorySeparatorChar + mainIconMapName;
            //writeoutDictionary(dictionaryMainIcon, outputPathFull);

            //HashmapDumper.PrintMainPlusOffset(0x396F5A0, 8);
            //HashmapDumper.SendRawEncoded($"configure pollRate 34");
            //HashmapDumper.SendRawEncoded($"configure keySleepTime 50");
            //SendKeyCommand(true, HidKeyboardKey.HidKeyboardKey_T, HidKeyboardKey.HidKeyboardKey_E, HidKeyboardKey.HidKeyboardKey_S, HidKeyboardKey.HidKeyboardKey_T);
            /*SendKeyCommand(true, HidKeyboardKey.HidKeyboardKey_K, HidKeyboardKey.HidKeyboardKey_E, HidKeyboardKey.HidKeyboardKey_Y, HidKeyboardKey.HidKeyboardKey_B,
                HidKeyboardKey.HidKeyboardKey_O, HidKeyboardKey.HidKeyboardKey_A, HidKeyboardKey.HidKeyboardKey_R, HidKeyboardKey.HidKeyboardKey_D, HidKeyboardKey.HidKeyboardKey_Space);
            Thread.Sleep(50);
            SendKeyCommand(true, HidKeyboardKey.HidKeyboardKey_K, HidKeyboardKey.HidKeyboardKey_E, HidKeyboardKey.HidKeyboardKey_Y, HidKeyboardKey.HidKeyboardKey_B,
                HidKeyboardKey.HidKeyboardKey_O, HidKeyboardKey.HidKeyboardKey_A, HidKeyboardKey.HidKeyboardKey_R, HidKeyboardKey.HidKeyboardKey_D, HidKeyboardKey.HidKeyboardKey_Space);*/
            //HashmapDumper.SendRawEncoded($"keyMod 4 8 5 4 5 4");

            //HashmapDumper.SendRawEncoded("debugKeys");
            /*HashmapDumper.SendRawEncoded($"tap 450 420");
            HashmapDumper.SendRawEncoded($"tap 450 420", false, false);
            HashmapDumper.SendRawEncoded($"tap 450 420", false, false);
            HashmapDumper.SendRawEncoded($"tap 450 420", false, false);
            HashmapDumper.SendRawEncoded($"tap 450 540", false, false);
            HashmapDumper.SendRawEncoded($"tap 450 480", false, false);
            HashmapDumper.SendRawEncoded($"tap 450 540", false, false);
            HashmapDumper.SendRawEncoded($"tap 450 540", false, false);*/
            /*HashmapDumper.sb.IP = "192.168.0.132";
            HashmapDumper.SendRawEncoded($"click a", false, true);

            while (true)
            {
                HashmapDumper.SendRawEncoded($"click a", false, false);
                Thread.Sleep(300);
            }*/

            var dictionaryUnitIcon = HashmapDumper.GetMainInventoryIconHashmap(@"D:\Switch\ACNH\Unpackv2\patched_acnh_2_0_0\RawDumps\UnitIcon.Dmp");
            string outputPathUnitFull = outputMainPath + Path.DirectorySeparatorChar + unitIconMapName;
            writeoutDictionary(dictionaryUnitIcon, outputPathUnitFull);

            dictionaryUnitIcon = HashmapDumper.GetMainInventoryIconHashmap(@"D:\Switch\ACNH\Unpackv2\patched_acnh_2_0_0\RawDumps\MenuIcon.Dmp");
            outputPathUnitFull = outputMainPath + Path.DirectorySeparatorChar + mainIconMapName;
            writeoutDictionary(dictionaryUnitIcon, outputPathUnitFull);
        }

        public static void SendKeyCommand(bool connect, params HidKeyboardKey[] keys)
        {
            HashmapDumper.SendRawEncoded($"key{string.Concat(keys.Select(z => $" {(int)z}"))}", false, connect);
        }

        public static void drawcircle(int x0, int y0, int radius)
        {
            HashmapDumper.SendRawEncoded($"touch 0 0");
            int x = radius;
            int y = 0;
            int err = 0;

            while (x >= y)
            {
                setPixel(x0 + x, y0 + y, 7);
                setPixel(x0 + y, y0 + x, 7);
                setPixel(x0 - y, y0 + x, 7);
                setPixel(x0 - x, y0 + y, 7);
                setPixel(x0 - x, y0 - y, 7);
                setPixel(x0 - y, y0 - x, 7);
                setPixel(x0 + y, y0 - x, 7);
                setPixel(x0 + x, y0 - y, 7);

                if (err <= 0)
                {
                    y += 1;
                    err += 2 * y + 1;
                }

                if (err > 0)
                {
                    x -= 1;
                    err -= 2 * x + 1;
                }
            }
        }

        static void setPixel(int x, int y, int depth)
        {
            HashmapDumper.SendRawEncoded($"touch {x} {y}", false, false);
            Thread.Sleep(15);
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
