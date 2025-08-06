using UnityEngine;

namespace meanran_xuexi_mods
{
    public class UI样式管理器
    {
        public static GUIStyle 窗口样式_z = null;
        public static GUIStyle 窗口样式_h
        {
            get
            {
                if (窗口样式_z == null)
                {
                    var 主体颜色 = new Color(0.3f, 0.3f, 0.3f, 0.8f);  // 此临时变量会自动销毁
                    var 正常边框颜色 = Color.black;
                    var 悬停边框颜色 = Color.clear;
                    var 按下边框颜色 = new Color(1f, 1f, 1f, 0.8f);     // 此临时变量会自动销毁
                    var 键盘焦点边框颜色 = Color.clear;
                    窗口样式_z = 扩展方法.创建GUIStyle(("窗口样式", 宽: 64, 高: 64, 边框宽度: 1, 内容区内缩宽度: 4, 排版间距: 2, 主体颜色: 主体颜色, 正常边框颜色, 悬停边框颜色, 按下边框颜色, 键盘焦点边框颜色));
                    窗口样式_z.alignment = TextAnchor.MiddleCenter;
                    窗口样式_z.imagePosition = ImagePosition.ImageOnly;
                    // 窗口只有两种状态=>1.正常渲染的Normal贴图 2.点击获取焦点后的onNormal贴图
                    窗口样式_z.onNormal.background = 窗口样式_z.active.background;    // 资源转移, 不需要释放内存
                    窗口样式_z.active.background = null;
                    窗口样式_z.onActive.background = null;
                }
                return 窗口样式_z;
            }
        }
        public static GUIStyle 输入框体样式_z = null;
        public static GUIStyle 输入框体样式_h
        {
            get
            {
                if (输入框体样式_z == null)
                {
                    var 主体颜色 = new Color(0.3f, 0.3f, 0.3f, 0.8f);  // 此临时变量会自动销毁
                    var 正常边框颜色 = Color.black;
                    var 悬停边框颜色 = Color.white;
                    var 按下边框颜色 = Color.black;
                    var 键盘焦点边框颜色 = Color.white;
                    输入框体样式_z = 扩展方法.创建GUIStyle(("输入框体样式", 宽: 64, 高: 64, 边框宽度: 1, 内容区内缩宽度: 4, 排版间距: 2, 主体颜色: 主体颜色, 正常边框颜色, 悬停边框颜色, 按下边框颜色, 键盘焦点边框颜色));
                    输入框体样式_z.alignment = TextAnchor.MiddleLeft;
                }
                return 输入框体样式_z;
            }
        }
        public static GUIStyle 输入框头样式_z = null;
        public static GUIStyle 输入框头样式_h
        {
            get
            {
                if (输入框头样式_z == null)
                {
                    var 主体颜色 = Color.clear;
                    var 正常边框颜色 = Color.clear;
                    var 悬停边框颜色 = Color.clear;
                    var 按下边框颜色 = Color.clear;
                    var 键盘焦点边框颜色 = Color.clear;
                    输入框头样式_z = 扩展方法.创建GUIStyle(("输入框头样式", 宽: 64, 高: 64, 边框宽度: 1, 内容区内缩宽度: 4, 排版间距: 2, 主体颜色: 主体颜色, 正常边框颜色, 悬停边框颜色, 按下边框颜色, 键盘焦点边框颜色));
                    输入框头样式_z.alignment = TextAnchor.MiddleLeft;
                    输入框头样式_z.stretchWidth = false;
                }
                return 输入框头样式_z;
            }
        }
        public static GUIStyle 按钮样式_z = null;
        public static GUIStyle 按钮样式_h
        {
            get
            {
                if (按钮样式_z == null)
                {
                    var 主体颜色 = new Color(0.3f, 0.3f, 0.3f, 0.8f);  // 此临时变量会自动销毁
                    var 正常边框颜色 = Color.black;
                    var 悬停边框颜色 = Color.white;
                    var 按下边框颜色 = Color.black;
                    var 键盘焦点边框颜色 = Color.white;
                    按钮样式_z = 扩展方法.创建GUIStyle(("按钮样式", 宽: 64, 高: 64, 边框宽度: 1, 内容区内缩宽度: 4, 排版间距: 2, 主体颜色: 主体颜色, 正常边框颜色, 悬停边框颜色, 按下边框颜色, 键盘焦点边框颜色));
                    按钮样式_z.alignment = TextAnchor.MiddleCenter;
                }
                return 按钮样式_z;
            }
        }
        public static GUIStyle 浅背景中对齐样式_z = null;
        public static GUIStyle 浅背景中对齐样式_h
        {
            get
            {
                if (浅背景中对齐样式_z == null)
                {
                    var 主体颜色 = new Color(0.4f, 0.4f, 0.4f, 0.8f);  // 此临时变量会自动销毁
                    var 正常边框颜色 = Color.black;
                    var 悬停边框颜色 = Color.clear;
                    var 按下边框颜色 = Color.clear;
                    var 键盘焦点边框颜色 = Color.clear;
                    浅背景中对齐样式_z = 扩展方法.创建GUIStyle(("浅背景样式", 宽: 64, 高: 64, 边框宽度: 1, 内容区内缩宽度: 4, 排版间距: 2, 主体颜色: 主体颜色, 正常边框颜色, 悬停边框颜色, 按下边框颜色, 键盘焦点边框颜色));
                    浅背景中对齐样式_z.alignment = TextAnchor.MiddleCenter;
                }
                return 浅背景中对齐样式_z;
            }
        }
        public static GUIStyle 无背景左对齐样式_z = null;
        public static GUIStyle 无背景左对齐样式_h
        {
            get
            {
                if (无背景左对齐样式_z == null)
                {
                    var 主体颜色 = Color.clear;
                    var 正常边框颜色 = Color.clear;
                    var 悬停边框颜色 = Color.clear;
                    var 按下边框颜色 = Color.clear;
                    var 键盘焦点边框颜色 = Color.clear;
                    无背景左对齐样式_z = 扩展方法.创建GUIStyle(("无背景样式", 宽: 64, 高: 64, 边框宽度: 1, 内容区内缩宽度: 4, 排版间距: 2, 主体颜色: 主体颜色, 正常边框颜色, 悬停边框颜色, 按下边框颜色, 键盘焦点边框颜色));
                    无背景左对齐样式_z.alignment = TextAnchor.MiddleLeft;
                }
                return 无背景左对齐样式_z;
            }
        }
        public static GUIStyle 无背景中对齐样式_z = null;
        public static GUIStyle 无背景中对齐样式_h
        {
            get
            {
                if (无背景中对齐样式_z == null)
                {
                    无背景中对齐样式_z = new(无背景左对齐样式_h);
                    无背景中对齐样式_z.alignment = TextAnchor.MiddleCenter;
                }
                return 无背景中对齐样式_z;
            }
        }
        public static GUIStyle 无背景右对齐样式_z = null;
        public static GUIStyle 无背景右对齐样式_h
        {
            get
            {
                if (无背景右对齐样式_z == null)
                {
                    无背景右对齐样式_z = new(无背景左对齐样式_h);
                    无背景右对齐样式_z.alignment = TextAnchor.MiddleRight;
                }
                return 无背景右对齐样式_z;
            }
        }
    }
}