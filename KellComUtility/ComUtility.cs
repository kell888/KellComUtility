using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Configuration;

namespace KellComUtility
{
    public static class ComUtility
    {
        public static string GetHex(byte data, bool format=false)
        {
            string fo = string.Empty;
            if (format)
                fo = "0x";
            return fo + data.ToString("X2");
        }

        public static string Format(string hex, bool format)
        {
            string fo = string.Empty;
            string[] fs = hex.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            if (fs.Length > 1)
            {
                StringBuilder sb = new StringBuilder();
                foreach (string f in fs)
                {
                    if (format)
                    {
                        if (!f.StartsWith("0x"))
                            sb.Append("0x" + f);
                        else
                            sb.Append(f);
                    }
                    else
                    {
                        if (f.StartsWith("0x"))
                            sb.Append(f.Substring(2));
                        else
                            sb.Append(f);
                    }
                    sb.Append(" ");
                }
                fo = sb.ToString().TrimEnd(' ');
            }
            else
            {
                fo = hex;
                if (format)
                {
                    if (!hex.StartsWith("0x"))
                        fo = "0x" + hex;
                }
                else
                {
                    if (hex.StartsWith("0x"))
                        fo = hex.Substring(2);
                }
            }
            return fo;
        }

        public static string GetHex(byte[] data, bool format = false)
        {
            string fo = string.Empty;
            if (format)
                fo = "0x";
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sb.Append(fo + data[i].ToString("X2") + " ");
            }
            return sb.ToString();
        }

        public static byte[] StrHexToBin(string StrHex)
        {
            StrHex = StrHex.Trim();
            string[] temp = StrHex.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            byte[] buf = new byte[temp.Length];
            for (int i = 0; i < temp.Length; i++)
            {
                buf[i] = System.Convert.ToByte(temp[i], 16);
            }
            return buf;
        }

        public static int[] GetDefaultIndexs(int length)
        {
            int[] indexs = new int[length];
            for (int i = 0; i < length; i++)
            {
                indexs[i] = i;
            }
            return indexs;
        }

        public static byte[] MergeData(byte[] data1, byte[] data2)
        {
            int len1=0,len2=0;
            if (data1 != null) len1 = data1.Length;
            if (data2 != null) len2 = data2.Length;
            byte[] data = new byte[len1 + len2];
            if (data.Length > 0)
            {
                for (int i = 0; i < len1; i++)
                {
                    data[i] = data1[i];
                }
                for (int i = 0; i < len2; i++)
                {
                    data[len1 + i] = data2[i];
                }
            }
            return data;
        }

        public static object[] Convert(byte[] data)
        {
            object[] result = null;
            if (data != null)
            {
                result = new object[data.Length];
                data.CopyTo(result, 0);
            }
            return result;
        }

        public static object Parity(object[] data, int[] indexs)
        {
            string ParityBinFile = "Parity.dll";
            string parity = ConfigurationManager.AppSettings["ParityBinFile"];
            if (!string.IsNullOrEmpty(parity))
                ParityBinFile = parity;
            Assembly ass = Assembly.LoadFrom(ParityBinFile);
            if (ass != null)
            {
                string ParityType = "CRC";
                string typ = ConfigurationManager.AppSettings["ParityType"];
                if (!string.IsNullOrEmpty(typ))
                    ParityType = typ;
                Type t = ass.GetType(ParityType, false, true);
                if (t != null)
                {
                    string ParityMethod = "CRC16";
                    string meth = ConfigurationManager.AppSettings["ParityMethod"];
                    if (!string.IsNullOrEmpty(meth))
                        ParityMethod = meth;
                    MethodInfo m = t.GetMethod(ParityMethod, BindingFlags.Public | BindingFlags.Static);
                    if (m != null)
                    {
                        object[] paras = null;
                        if (indexs != null)
                        {
                            paras = new object[indexs.Length];
                            for (int i = 0; i < indexs.Length; i++)
                            {
                                int ind = indexs[i];
                                if (ind < 0 || ind > data.Length - 1)
                                    return null;
                                paras[i] = data[ind];
                            }
                        }
                        return m.Invoke(null, paras);
                    }
                }
            }
            return null;
        }

        public static bool PassParity(object raw, object parity)
        {
            string ParityBinFile = "Parity.dll";
            string parit = ConfigurationManager.AppSettings["ParityBinFile"];
            if (!string.IsNullOrEmpty(parit))
                ParityBinFile = parit;
            Assembly ass = Assembly.LoadFrom(ParityBinFile);
            if (ass != null)
            {
                string CheckParityType = "Check";
                string typ = ConfigurationManager.AppSettings["CheckParityType"];
                if (!string.IsNullOrEmpty(typ))
                    CheckParityType = typ;
                Type t = ass.GetType(CheckParityType, false, true);
                if (t != null)
                {
                    string CheckParityMethod = "CheckParity";
                    string meth = ConfigurationManager.AppSettings["CheckParityMethod"];
                    if (!string.IsNullOrEmpty(meth))
                        CheckParityMethod = meth;
                    MethodInfo m = t.GetMethod(CheckParityMethod, BindingFlags.Public | BindingFlags.Static);
                    if (m != null)
                    {
                        object[] paras = new object[] { raw, parity };
                        return (bool)m.Invoke(null, paras);
                    }
                }
            }
            return false;
        }

        public static void GetBytes(ushort pari, out byte pari1, out byte pari2)
        {
            byte[] data = System.BitConverter.GetBytes(pari);
            pari1 = data[0];
            pari2 = data[1];
        }
    }
}
