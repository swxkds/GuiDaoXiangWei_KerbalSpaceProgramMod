using System;
using System.Runtime.CompilerServices;
using Smooth.Delegates;
using UnityEngine;

namespace meanran_xuexi_mods
{
    public static class 扩展方法
    {
        public static string 日志内容_GUIStyle(this GUIStyle 样式实例_c)
        {
            return
            $"样式实例.name: {样式实例_c.name}\n样式实例.padding: {样式实例_c.padding}\n样式实例.border: {样式实例_c.border}\n" +
            $"样式实例.contentOffset: {样式实例_c.contentOffset}\n样式实例.stretchHeight: {样式实例_c.stretchHeight}\n" +
            $"样式实例.stretchWidth: {样式实例_c.stretchWidth}\n样式实例.alignment: {样式实例_c.alignment}\n" +
            $"样式实例.clipping: {样式实例_c.clipping}\n样式实例.font: {样式实例_c.font}\n样式实例.fontSize: {样式实例_c.fontSize}\n" +
            $"样式实例.fontStyle: {样式实例_c.fontStyle}\n样式实例.richText: {样式实例_c.richText}\n" +
            $"样式实例.wordWrap: {样式实例_c.wordWrap}\n样式实例.imagePosition: {样式实例_c.imagePosition}\n" +
            $"样式实例.fixedHeight: {样式实例_c.fixedHeight}\n样式实例.fixedWidth: {样式实例_c.fixedWidth}\n" +
            $"样式实例.margin: {样式实例_c.margin}\n样式实例.overflow: {样式实例_c.overflow}\n" +
            $"样式实例.normal.background: {样式实例_c.normal.background}\n" +
            $"样式实例.hover.background: {样式实例_c.hover.background}\n" +
            $"样式实例.active.background: {样式实例_c.active.background}\n" +
            $"样式实例.focused.background: {样式实例_c.focused.background}\n" +
            $"样式实例.onNormal.background: {样式实例_c.onNormal.background}\n" +
            $"样式实例.onHover.background: {样式实例_c.onHover.background}\n" +
            $"样式实例.onActive.background: {样式实例_c.onActive.background}\n" +
            $"样式实例.onFocused.background: {样式实例_c.onFocused.background}\n";
        }
        public static void 初始化像素数组(this Color[] 像素数组_c, int 列数_c, int 行数_c, int 边框宽度_c, Color 主体颜色_c, Color 边框颜色_c)
        {
            if (列数_c * 行数_c != 像素数组_c.Length)
            { throw new Exception(InstallChecker.弹出日志面板($"数组溢出: 访问下标=> {列数_c * 行数_c} , 实际下标=> {像素数组_c.Length}")); }

            int 最大边框宽度 = Math.Min(列数_c, 行数_c) / 2;
            if (边框宽度_c < 0 || 边框宽度_c > 最大边框宽度)
            { throw new Exception(InstallChecker.弹出日志面板($"边框宽度是负数或者大于最大边框宽度{最大边框宽度}")); }

            for (var 行 = 0; 行 < 行数_c; 行++)
            {
                for (var 列 = 0; 列 < 列数_c; 列++)
                {
                    if (列 < 边框宽度_c || 列 >= 列数_c - 边框宽度_c || 行 < 边框宽度_c || 行 >= 行数_c - 边框宽度_c)
                    { 像素数组_c[行 * 列数_c + 列] = 边框颜色_c; }
                    else { 像素数组_c[行 * 列数_c + 列] = 主体颜色_c; }
                }
            }
        }
        public static void 初始化GUIStyle(this GUIStyle 样式实例_c, string 样式名称_c, int 边框宽度_c, int 内容区内缩宽度_c, int 排版间距_c, Texture2D 正常贴图_c, Texture2D 悬停贴图_c, Texture2D 按下贴图_c, Texture2D 键盘焦点贴图_c)
        {
            // 对于传入的样式实例, 不同的UI控件只会选择性的读取自己会使用到的样式参数
            // 可以给垂直布局组(GUILayout.BeginVertical(样式实例);)或者水平布局组(GUILayout.BeginHorizontal(样式实例);)传入样式实例,    
            // 布局组会读取样式实例中的 样式实例.normal.background 的贴图作为区域背景并渲染
            样式实例_c.name = 样式名称_c;                                                             // GUIStyle的名称(用于根据名称获取它们)
            样式实例_c.padding = new(内容区内缩宽度_c, 内容区内缩宽度_c, 内容区内缩宽度_c, 内容区内缩宽度_c);   // 贴图区域收缩N后的区域是文本区
            样式实例_c.border = new(边框宽度_c, 边框宽度_c, 边框宽度_c, 边框宽度_c);                          // 贴图缩放时固定的边框宽度(原理见九宫格纹理)
            样式实例_c.contentOffset = Vector2.zero;                                               // 文本区左上角坐标偏移
            样式实例_c.stretchHeight = false;                                                      // 整体区域 < 窗口时是否缩放
            样式实例_c.stretchWidth = true;                                                       // 整体区域 < 窗口时是否缩放
            样式实例_c.alignment = TextAnchor.MiddleCenter;                                         // 文本对齐方式
            样式实例_c.clipping = TextClipping.Clip;                                               // 文本内容超出区域时的截断方式
            样式实例_c.font = GUI.skin.window.font;                                                // 文本字体
            样式实例_c.fontSize = GUI.skin.window.fontSize;                                        // 文本字体尺寸
            样式实例_c.fontStyle = GUI.skin.window.fontStyle;                                      // 文本字体加粗/斜体等变种
            样式实例_c.richText = true;                                                            // 文本富文本开关
            样式实例_c.wordWrap = false;                                                           // 文本自动换行开关
            // ImageLeft:图像在左,文本在右; ImageAbove:图像在上,文本在下; ImageOnly:只显示图像; TextOnly:只显示文本
            样式实例_c.imagePosition = ImagePosition.TextOnly;                                     // 既有文本又有贴图时如何显示
            样式实例_c.fixedHeight = 0;                                                            // 强制区域尺寸(=0时由布局计算区域)
            样式实例_c.fixedWidth = 0;                                                             // 强制区域尺寸(=0时由布局计算区域)
            样式实例_c.margin = new(排版间距_c, 排版间距_c, 排版间距_c, 排版间距_c);                          // GUI元素之间的间距
            样式实例_c.overflow = new(0, 0, 0, 0);                // 贴图四边阴影宽度(只采样和渲染,不参与布局尺寸的部分)

            // 鼠标无触碰UI控件显示的贴图/鼠标触碰UI控件显示的贴图/鼠标触碰UI控件+鼠标按下显示的贴图/鼠标触碰输入框+鼠标按下显示的贴图
            样式实例_c.normal.background = 正常贴图_c;
            样式实例_c.hover.background = 悬停贴图_c;
            样式实例_c.active.background = 按下贴图_c;
            样式实例_c.focused.background = 键盘焦点贴图_c;
            // 状态类UI控件在切换状态时切换贴图, 例: 开关控件从按下状态切换到正常状态需要显示不同的贴图
            样式实例_c.onNormal.background = 正常贴图_c;
            样式实例_c.onHover.background = 悬停贴图_c;
            样式实例_c.onActive.background = 按下贴图_c;
            样式实例_c.onFocused.background = 键盘焦点贴图_c;

            样式实例_c.normal.textColor = Color.white;
            样式实例_c.onNormal.textColor = Color.white;
            样式实例_c.hover.textColor = Color.white;
            样式实例_c.onHover.textColor = Color.white;
            样式实例_c.active.textColor = Color.white;
            样式实例_c.onActive.textColor = Color.white;
            样式实例_c.focused.textColor = Color.white;
            样式实例_c.onFocused.textColor = Color.white;
        }
        public static GUIStyle 创建GUIStyle((string 名称, int 宽, int 高, int 边框宽度, int 内容区内缩宽度, int 排版间距, Color 主体颜色, Color 正常边框颜色, Color 悬停边框颜色, Color 按下边框颜色, Color 键盘焦点边框颜色) 样式_c, GUIStyle 模板_c = null)
        {
            var 像素数组 = new Color[样式_c.宽 * 样式_c.高];        // 临时变量会自动释放内存

            Texture2D 正常贴图 = null;
            Texture2D 悬停贴图 = null;
            Texture2D 按下贴图 = null;
            Texture2D 键盘焦点贴图 = null;

            GUIStyle 样式实例 = null;

            if (样式_c.正常边框颜色 != Color.clear)
            {
                正常贴图 = new Texture2D(样式_c.宽, 样式_c.高, TextureFormat.RGBA32, false, true);  // 引用被样式实例持有, 不需要释放内存
                像素数组.初始化像素数组(样式_c.宽, 样式_c.高, 样式_c.边框宽度, 样式_c.主体颜色, 样式_c.正常边框颜色);
                正常贴图.SetPixels(像素数组);
                正常贴图.Apply();
            }

            if (样式_c.悬停边框颜色 != Color.clear)
            {

                悬停贴图 = new Texture2D(样式_c.宽, 样式_c.高, TextureFormat.RGBA32, false, true);  // 引用被样式实例持有, 不需要释放内存
                像素数组.初始化像素数组(样式_c.宽, 样式_c.高, 样式_c.边框宽度, 样式_c.主体颜色, 样式_c.悬停边框颜色);
                悬停贴图.SetPixels(像素数组);
                悬停贴图.Apply();
            }

            if (样式_c.按下边框颜色 != Color.clear)
            {
                按下贴图 = new Texture2D(样式_c.宽, 样式_c.高, TextureFormat.RGBA32, false, true);  // 引用被样式实例持有, 不需要释放内存
                像素数组.初始化像素数组(样式_c.宽, 样式_c.高, 样式_c.边框宽度, 样式_c.主体颜色, 样式_c.按下边框颜色);
                按下贴图.SetPixels(像素数组);
                按下贴图.Apply();
            }

            if (样式_c.键盘焦点边框颜色 != Color.clear)
            {
                键盘焦点贴图 = new Texture2D(样式_c.宽, 样式_c.高, TextureFormat.RGBA32, false, true);  // 引用被样式实例持有, 不需要释放内存
                像素数组.初始化像素数组(样式_c.宽, 样式_c.高, 样式_c.边框宽度, 样式_c.主体颜色, 样式_c.键盘焦点边框颜色);
                键盘焦点贴图.SetPixels(像素数组);
                键盘焦点贴图.Apply();
            }

            if (模板_c == null) { 样式实例 = new GUIStyle(); }                // 引用被上级调用者持有, 不需要释放内存
            else { 样式实例 = new GUIStyle(模板_c); }
            样式实例.初始化GUIStyle(样式_c.名称, 样式_c.边框宽度, 样式_c.内容区内缩宽度, 样式_c.排版间距, 正常贴图, 悬停贴图, 按下贴图, 键盘焦点贴图);

            return 样式实例;
        }
        public static void Swap<T>(ref T a_c, ref T b_c)
        {
            T temp = a_c;
            a_c = b_c;
            b_c = temp;
        }

        public static double 换算_弧度转角度(this double 弧度_c)
        {
            return 弧度_c * 57.29577951308232;
        }

        public enum 长度单位 { m, km, Mm, Gm };
        public static string 换算_米转自适应单位(double value_c, 长度单位 单位_c = 长度单位.m, string format_c = "N2")
        {
            switch (单位_c)
            {
                case 长度单位.m:
                    return value_c.ToString(format_c) + "m";
                case 长度单位.km:
                    return (value_c / 1000).ToString(format_c) + "km";
                case 长度单位.Mm:
                    return (value_c / 1000000).ToString(format_c) + "Mm";
                default:
                    return (value_c / 1000000000).ToString(format_c) + "Gm";
            }
        }
        public static string 换算_秒转日期(double 秒数_c, int 日期精确到小数几位_c = 2)
        {
            if (double.IsInfinity(秒数_c) || double.IsNaN(秒数_c)) return "Inf";

            string 结果 = "";
            bool 高精标志 = 日期精确到小数几位_c > 0;

            try
            {
                string[] 单位表 = { "y", "d", "h", "m", "s" };
                long[] 换算表 = { KSPUtil.dateTimeFormatter.Year, KSPUtil.dateTimeFormatter.Day, KSPUtil.dateTimeFormatter.Hour, KSPUtil.dateTimeFormatter.Minute, 1 };

                if (秒数_c < 0)
                {
                    结果 += "-";
                    秒数_c *= -1;
                }

                for (int i = 0; i < 单位表.Length; i++)
                {
                    long n = (long)(秒数_c / 换算表[i]);
                    bool first = 结果.Length < 2;
                    if (!first || n != 0 || (i == 单位表.Length - 1 && 结果 == ""))
                    {
                        // 当字数小于2时,即年时，不需要用空格分割
                        if (!first) 结果 += " ";

                        // 其它单位全部换算后，当前单位是秒时，根据高精标志判断是否输出小数点的秒
                        if (高精标志 && 秒数_c < 60 && i == 单位表.Length - 1)
                            结果 += 秒数_c.ToString("00." + new string('0', 日期精确到小数几位_c));
                        else if (first) 结果 += n.ToString();               // 单位年是多少就写多少
                        else 结果 += n.ToString(i == 1 ? "000" : "00");     // 单位天占位3格，时、分和无小数的秒占位两格

                        结果 += 单位表[i];
                    }

                    秒数_c -= n * 换算表[i];
                }
            }
            catch (Exception)
            {
                return "NaN";
            }

            return 结果;
        }

        public static double 循环取值范围_正360(double value_c)
        {
            value_c = value_c % 360.0;
            if (value_c < 0) { return value_c + 360.0; }
            return value_c;
        }

        public static double 循环取值范围_正负180(double value_c)
        {
            value_c = 循环取值范围_正360(value_c);
            if (value_c > 180) { value_c -= 360; }
            return value_c;
        }
        public static string 换算_角度转角分秒(double angle_c)
        {
            int 角度 = (int)Math.Floor(Math.Abs(angle_c));
            int 角分 = (int)Math.Floor(60 * (Math.Abs(angle_c) - 角度));
            int 角秒 = (int)Math.Floor(3600 * (Math.Abs(angle_c) - 角度 - 角分 / 60.0));

            return $"{角度:0}° {角分:00}' {角秒:00}\"";
        }
        public static string 换算_角度转经度(double angle_c)
        {
            var 经度 = 循环取值范围_正负180(angle_c);
            return $"{换算_角度转角分秒(经度)}{((经度 > 0.0) ? " E东经" : " W西经")}";
        }
    }
}