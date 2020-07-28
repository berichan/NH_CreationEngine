using System;
using System.Collections.Generic;
using System.Text;

namespace NH_CreationEngine.API
{
    [Serializable]
    public class ItemObject
    {
        public ushort item_id;

        public bool is_sold;
        public int sell_price;

        public int var_count;
        public Variation[] variations;

        public ItemInfo info_en;
        public ItemInfo info_jp;
        public ItemInfo info_es;
        public ItemInfo info_fr;
        public ItemInfo info_it;
        public ItemInfo info_ko;
        public ItemInfo info_zhs;
        public ItemInfo info_zht;
        public ItemInfo info_de;
    }

    [Serializable]
    public class Variation
    {
        public byte body;
        public byte fabric;
        public string img_url;
    }

    [Serializable]
    public class ItemInfo
    {
        public string lang; // 3 bytes!
        public string item_name;
        public string body_name;
        public string fabric_name;
    }
}
