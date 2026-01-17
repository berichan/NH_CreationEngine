using System;
using System.Collections.Generic;
using System.Text;

namespace NH_CreationEngine.API
{
    [Serializable]
    public class ItemListObject
    {
        public int count;
        public ItemDigest[] items;
    }

    [Serializable]
    public class ItemDigest
    {
        public ushort item_id;
        public string url;
        public int var_count;

        public string name_en;
        public string name_jp;
        public string name_es;
        public string name_fr;
        public string name_it;
        public string name_ko;
        public string name_zhs;
        public string name_zht;
        public string name_de;
    }
}
