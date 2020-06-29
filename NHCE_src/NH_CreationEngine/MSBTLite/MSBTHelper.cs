using System;
using System.Collections.Generic;
using System.Text;

namespace MsbtLite
{
    public static class MSBTHelper
    {
        public static MSBT AttemptLoadFile(string path)
        {
			MSBT _msbt = null;
			if (path != string.Empty)
			{
				_msbt = new MSBT(path);
			}
			return _msbt;
		}

		public static Dictionary<string, string> MSBTtoDic(this MSBT ms)
        {
			Dictionary<string, string> toRet = new Dictionary<string, string>();
			for (int i = 0; i < ms.TXT2.NumberOfStrings; i++)
			{
				if (ms.HasLabels)
				{
					toRet.Add(ms.LBL1.Labels[i].ToString(), ms.TXT2.Strings[i].ToString());
				}
				else
				{
					toRet.Add(ms.TXT2.Strings[i].ToString(), ms.TXT2.Strings[i].ToString());
				}
			}
			return toRet;
		}
    }
}
