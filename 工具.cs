using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace meanran_xuexi_mods
{
    public static class 扩展方法
    {
        public static void Swap<T>(ref T a, ref T b)
        {
            T temp = a;
            a = b;
            b = temp;
        }

        public enum 长度单位 { m, km, Mm, Gm };
        public static string 换算_米转自适应单位(double value, 长度单位 单位 = 长度单位.m, string format = "N3")
        {
            switch (单位)
            {
                case 长度单位.m:
                    return value.ToString(format) + " m";
                case 长度单位.km:
                    return (value / 1000).ToString(format) + " km";
                case 长度单位.Mm:
                    return (value / 1000000).ToString(format) + " Mm";
                default:
                    return (value / 1000000000).ToString(format) + " Gm";
            }
        }
        public static string 换算_秒转日期(double 秒数, int 日期精确到小数几位 = 0)
        {
            if (double.IsInfinity(秒数) || double.IsNaN(秒数)) return "Inf";

            string 结果 = "";
            bool 高精标志 = 日期精确到小数几位 > 0;

            try
            {
                string[] 单位表 = { "y", "d", "h", "m", "s" };
                long[] 换算表 = { KSPUtil.dateTimeFormatter.Year, KSPUtil.dateTimeFormatter.Day, KSPUtil.dateTimeFormatter.Hour, KSPUtil.dateTimeFormatter.Minute, 1 };

                if (秒数 < 0)
                {
                    结果 += "-";
                    秒数 *= -1;
                }

                for (int i = 0; i < 单位表.Length; i++)
                {
                    long n = (long)(秒数 / 换算表[i]);
                    bool first = 结果.Length < 2;
                    if (!first || n != 0 || (i == 单位表.Length - 1 && 结果 == ""))
                    {
                        // 当字数小于2时,即年时，不需要用空格分割
                        if (!first) 结果 += " ";

                        // 其它单位全部换算后，当前单位是秒时，根据高精标志判断是否输出小数点的秒
                        if (高精标志 && 秒数 < 60 && i == 单位表.Length - 1)
                            结果 += 秒数.ToString("00." + new string('0', 日期精确到小数几位));
                        else if (first) 结果 += n.ToString();               // 单位年是多少就写多少
                        else 结果 += n.ToString(i == 1 ? "000" : "00");     // 单位天占位3格，时、分和无小数的秒占位两格

                        结果 += 单位表[i];
                    }

                    秒数 -= n * 换算表[i];
                }
            }
            catch (Exception)
            {
                return "NaN";
            }

            return 结果;
        }
    }
}