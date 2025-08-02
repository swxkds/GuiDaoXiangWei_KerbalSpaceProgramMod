using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace meanran_xuexi_mods
{
    [KSPAddon(KSPAddon.Startup.Instantly, true)]
    internal class Startup : MonoBehaviour
    {
        private void Start()
        {
            // 在Properties/AssemblyInfo.cs中写入[assembly: AssemblyTitle("任意标题")], 编译器自动生成AssemblyTitleAttribute特性
            // 注: 程序集的特性实例只能在AssemblyInfo.cs中写入相关代码, 由编译器自动生成
            var 标题特性实例 = (AssemblyTitleAttribute)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyTitleAttribute), false);
            string title = 标题特性实例?.Title;
            if (title == null)
            {
                title = "程序集未设置标题";
            }

            string version = Assembly.GetExecutingAssembly().GetName().FullName;
            if (version == null)
            {
                version = "程序集未设置版本";
            }

            Debug.Log("[" + title + "] Version " + version);
        }
    }

    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    internal class InstallChecker : MonoBehaviour
    {
        internal const string 模组名称 = "轨道相位调整计算器";
        internal const string 模组目录名称 = "GuiDao";
        internal const string DLL路径 = 模组目录名称 + "/Plugins";
        public static StreamWriter m_日志写入 = null;
        public static StreamWriter 日志写入
        {
            get
            {
                if (m_日志写入 == null)
                {
                    var 日志路径 = Path.Combine(Path.GetFullPath(KSPUtil.ApplicationRootPath), "GameData/GuiDao/Log/Log.txt").Replace('/', Path.DirectorySeparatorChar);
                    Directory.CreateDirectory(Path.GetDirectoryName(日志路径));
                    m_日志写入 = new StreamWriter(日志路径, append: false) { AutoFlush = true };
                }

                return m_日志写入;
            }
        }
        protected void Start()
        {
            // a.url：只显示GameData目录中的相对路径  // a.path：包括盘符的完整路径
            // 搜索此模组的 DLL 文件是否存在于错误的位置。这也将检测到重复的副本，因为只有一个可以位于正确的位置。
            var 错误加载表 = AssemblyLoader.loadedAssemblies.Where(a => a.assembly.GetName().Name == Assembly.GetExecutingAssembly().GetName().Name).Where(a => a.url != DLL路径);
            if (错误加载表.Any())
            {
                // 将完整路径转换成Uri格式, 并去除坎巴拉根目录，只保留相对路径
                var 路径表 = 错误加载表.Select(a => a.path).Select(p => Uri.UnescapeDataString(new Uri(Path.GetFullPath(KSPUtil.ApplicationRootPath)).MakeRelativeUri(new Uri(p)).ToString().Replace('/', Path.DirectorySeparatorChar)));
                var 日志内容 = $"<{模组名称}>安装了错误的版本, 所有DLL文件应位于 KSP/GameData/{DLL路径} 目录下, 请勿移动该文件夹内的任何文件\n\n" + "请移除以下错误文件：\n" + String.Join("\n", 路径表.ToArray());
                弹出报警日志面板(日志内容);
            }
        }

        // 调用者相关实参由编译器在编译时自动生成,无需手动传入
        public static string 弹出报警日志面板(string 日志内容,
                   [CallerMemberName] string 调用者方法名 = "",
                   [CallerFilePath] string 调用者源文件路径 = "",
                   [CallerLineNumber] int 调用者源文件行号 = 0)
        {
            PopupDialog.SpawnPopupDialog
      (
          new Vector2(0.5f, 0.5f),
          new Vector2(0.5f, 0.5f),
          "报警日志",
          "报警日志",
          日志内容,
          "OK",
          false,
          HighLogic.UISkin
      );
            var 日志格式化 = $"#{DateTime.Now.ToString("HH:mm:ss.fff")}\n日志内容: {日志内容}\n调用者方法名: {调用者方法名}\n调用者源文件路径: {调用者源文件路径}\n调用者源文件行号: {调用者源文件行号}\n";
            日志写入.WriteLine(日志格式化);
            return 日志格式化;
        }
    }
}

// 原版游戏的相关事件
// GameEvents.onHideUI.Add(this.HideUI);
// GameEvents.onShowUI.Add(this.ShowUI);
// GameEvents.onGamePause.Add(this.HideUIWhenPaused);
// GameEvents.onGameUnpause.Add(this.ShowUIwhenUnpaused);

// 在近心点处创建圆轨机动节点
// {
//     double UT = Planetarium.GetUniversalTime();
//     UT += FlightGlobals.ActiveVessel.orbit.timeToPe;
//     var o = FlightGlobals.ActiveVessel.orbit;
//     Vector3d circularizeDv = VesselOrbitalCalc.CircularizeAtPE(FlightGlobals.ActiveVessel);
//     // 如果翻转, 则减去
//     if (flipOrbit)
//         circularizeDv -= OrbitCalc.dBurnDV * o.Horizontal(UT);
//     else
//         circularizeDv += OrbitCalc.dBurnDV * o.Horizontal(UT);
//     FlightGlobals.ActiveVessel.patchedConicSolver.maneuverNodes.Clear();
//     VesselOrbitalCalc.PlaceManeuverNode(FlightGlobals.ActiveVessel, FlightGlobals.ActiveVessel.orbit, circularizeDv, UT);
// }

// 在远心点处创建圆轨机动节点
// Vector3d circularizeDv = VesselOrbitalCalc.CircularizeAtAP(FlightGlobals.ActiveVessel);
// double UT = Planetarium.GetUniversalTime();
// UT += FlightGlobals.ActiveVessel.orbit.timeToAp;
// var o = FlightGlobals.ActiveVessel.orbit;
// // 如果翻转, 则减去
// if (flipOrbit)
//     circularizeDv -= OrbitCalc.dBurnDV * o.Horizontal(UT);
// else
//     circularizeDv += OrbitCalc.dBurnDV * o.Horizontal(UT);
// FlightGlobals.ActiveVessel.patchedConicSolver.maneuverNodes.Clear();
// VesselOrbitalCalc.PlaceManeuverNode(FlightGlobals.ActiveVessel, FlightGlobals.ActiveVessel.orbit, circularizeDv, UT);




// 删除机动节点
// FlightGlobals.ActiveVessel.patchedConicSolver.maneuverNodes.Clear(); // 清除所有机动节点
// for (int i = FlightGlobals.ActiveVessel.patchedConicSolver.maneuverNodes.Count - 1; i >= 0; i--)
// {
//     FlightGlobals.ActiveVessel.patchedConicSolver.maneuverNodes[i].RemoveSelf();
// }

// 将本地图片加载到内存的方法
// if (System.IO.File.Exists(filePath))
// {
//     Texture2D 贴图 = new Texture2D(2, 2); ;
//     byte[] 文件流 = System.IO.File.ReadAllBytes(filePath);
//     贴图.LoadImage(文件流); // 会自动调整贴图的尺寸
// }

// 修改内存位图的像素数组的方法
// {
//     const Color 颜色 = new Color(238f / 255f, 238f / 255f, 238f / 255f);

//     // 创建一个宽X*高Y的内存位图
//     Texture2D 贴图 = new Texture2D(WIDTH, HEIGHT, TextureFormat.RGB24, false, true);

//     // 创建一个与内存位图相同尺寸的像素数组
//     Color[] 像素数组 = new Color[WIDTH * HEIGHT];
//     for (int i = 0; i < 像素数组.Length; i++)
//     { 像素数组[i] = 颜色; }

//     // 用自定义的像素数组覆盖掉给内存位图的像素区域
//     贴图.SetPixels(像素数组);

//     // SetPixels只是创建了一个消息,从哪个像素数组采样/从何处开始绘制/共绘制多少行多少列像素....
//     // 调用Apply()方法让内存位图从消息队列中取出消息并执行绘制
//     贴图.Apply();
// }

// 保存配置config.xml到本地
// {
//     // config.xml内容见以下
//     <? xml version = "1.0" encoding = "utf-8" ?>
//     < config >
//         < string name = "x" > 100 </ string >
//         < string name = "y" > 100 </ string >
//     </ config >

//     // 配置内容转换成内存数值见下
//     var conf = PluginConfiguration.CreateForType<任意类型>();   // 任意类型: 模组名称或类名
//     conf.load();
//     var x = float.Parse(conf.GetValue<string>("x"));
//     var y = float.Parse(conf.GetValue<string>("y"));

//     // 内存数值转换成配置内容见下
//     var x = 100; var y = 200;
//     var conf = PluginConfiguration.CreateForType<任意类型>();
//     conf.SetValue("x", x.ToString());
//     conf.SetValue("y", y.ToString());
//     conf.save();
// }