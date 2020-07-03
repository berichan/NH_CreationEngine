using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using NHSE.Injection;
using System.Linq;

namespace NH_Sysbot_Tools
{
    public static class HashmapDumper
    {
        public static SysBot sb = new SysBot();
        public static ulong offsetMainIcon = 0x3920C00;
        public static ulong offsetSizeMap = offsetMainIcon + 0xF4D0;

        static object _sync = new object();

        public static Dictionary<string, int> GetMainInventoryIconHashmap(ulong offset)
        {
            Dictionary<string, int> builtDic = new Dictionary<string, int>();

            lock (_sync)
            {
                sb.Connect(sb.IP, sb.Port);

                Thread.Sleep(1000);

                ulong nsoBaseAddress = getMainNsoBase(); // get offset of main
                ulong mainIconAddress = nsoBaseAddress + offset; // get where the hashmap is stored in the main executable

                // check this is the start of the hashmap
                KeyValuePair<string, int> startVal = PullKVP(mainIconAddress);
                if (!isHashMapBoundary(startVal))
                    throw new Exception("This isn't the start of a hashmap, your offset is incorrect or you have other cheats/plugins active that is shifting something in RAM. Try restarting the game pressing L when starting it up.");

                mainIconAddress += 8;

                uint itemsRead = 0;

                while (true)
                {
                    KeyValuePair<string, int> newVal = PullKVP(mainIconAddress);
                    if (!isHashMapBoundary(newVal))
                        builtDic.Add(newVal.Key, newVal.Value);
                    else
                        break;

                    Thread.Sleep(100);
                    mainIconAddress += 8;
                    itemsRead++;
                    Console.WriteLine("Items read: " + itemsRead.ToString());
                }
            }

            Console.WriteLine("Reached end of HashMap.");
            return builtDic;
        }

        private static KeyValuePair<string, int> PullKVP(ulong ramAddress)
        {
            sb.SendRawBytes(SwitchCommand.PeekAbsolute(ramAddress, 8));
            byte[] received = sb.ReadRawBytes(8); // this returns chars
            string strToWrite = "";
            foreach (byte rec in received)
                strToWrite += (char)rec;
            strToWrite = strToWrite.Replace("\n", string.Empty);
            Console.WriteLine(strToWrite);

            string half1 = strToWrite.Substring(0, 8);
            string half2 = strToWrite.Substring(8, 8);
            string half1Sorted = string.Format("0x{0}", new string(fix32BitEndian(half1.ToCharArray())));
            string half2Sorted = new string(fix32BitEndian(half2.ToCharArray()));
            Console.WriteLine(half1Sorted + " is " + half2Sorted);

            return new KeyValuePair<string, int>(half1Sorted, int.Parse(half2Sorted, System.Globalization.NumberStyles.HexNumber));
        }

        private static ulong getMainNsoBase()
        {
            sb.SendRawBytes(SwitchCommand.Encode("getMainNsoBase"));
            byte[] received = sb.ReadRawBytes(16);
            List<char> chars = new List<char>();
            foreach (byte b in received)
                chars.Add((char)b);
            string val = new string(chars.ToArray()).Replace("\0", string.Empty).Replace("\n", string.Empty);
            Console.WriteLine("Main at " + val);
            return ulong.Parse(val, System.Globalization.NumberStyles.HexNumber);
        }

        private static char[] fix32BitEndian(char[] eigthChars)
        {
            List<char[]> twoByteList = new List<char[]>();
            for (int i = 0; i < eigthChars.Length; i += 2) // filter into 2 char blocks
            {
                char[] toAdd = new char[2] { eigthChars[i], eigthChars[i + 1] };
                twoByteList.Add(toAdd);
            }
            twoByteList.Reverse();
            char[][] twoDArray = twoByteList.ToArray();

            List<char> oneDArray = new List<char>();
            foreach (char[] arrayOfChars in twoDArray)
                foreach (char wowChar in arrayOfChars)
                    oneDArray.Add(wowChar);

            return oneDArray.ToArray();
        }

        public static void ReadItemDump(FakeItem item, uint offset, uint offsetItem)
        {
            FakeItem itemToWrite = item;
            sb.WriteBytes(itemToWrite.GetAsBytes5(), offset);

            sb.SendRawBytes(SwitchCommand.Click(SwitchButton.DRIGHT));

            Thread.Sleep(1500);

            sb.SendRawBytes(SwitchCommand.Click(SwitchButton.B));

            byte[] received = sb.ReadBytes(offsetItem, 4196);
            string txt = System.Text.Encoding.UTF8.GetString(received);

            Console.WriteLine(txt);
            Console.WriteLine("******************************************************************************************************************");
        }

        private static bool isHashMapBoundary(KeyValuePair<string, int> toCheck)
        {
            return toCheck.Key == "0x00000001" && toCheck.Value == 0;
        }
    }
}
