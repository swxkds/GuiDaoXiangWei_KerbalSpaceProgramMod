using System;
using System.IO;
using KSP.IO;
using UnityEngine;

// 变量命名:_z说明是字段;_w说明是字符串;_h说明是属性或函数;_c说明是函数的实参;英文命名使用大驼峰;区域: 位置与尺寸;
namespace meanran_xuexi_mods
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class 轨道相位调整计算器 : MonoBehaviour
    {
        private void OnDestroy()
        {
            配置读写器_h.SetValue("x", 窗口区域_z.x.ToString());
            配置读写器_h.SetValue("y", 窗口区域_z.y.ToString());
            配置读写器_h.SetValue("switch", 主窗口开关_z.ToString());
            配置读写器_h.save();
        }
        private void Start()
        {
            GameEvents.onHideUI.Add(() => 主窗口开关_z = false);  // 隐藏UI回调
            GameEvents.onShowUI.Add(() => 主窗口开关_z = true);  // 显示UI回调
            // GameEvents.onGamePause.Add(() => 活跃么 = false);   // 暂停游戏回调
            // GameEvents.onGameUnpause.Add(() => 活跃么 = true);  // 继续游戏回调

            配置读写器_h.load();
            var (x, y) = (float.Parse(配置读写器_h.GetValue<string>("x")), float.Parse(配置读写器_h.GetValue<string>("y")));
            if (x >= 0 && x < Screen.width && y >= 0 && y < Screen.height)
            {
                (窗口区域_z.x, 窗口区域_z.y) = (x, y);
            }
            else
            {
                (窗口区域_z.x, 窗口区域_z.y) = (20, 20);
            }
            主窗口开关_z = bool.Parse(配置读写器_h.GetValue<string>("switch"));

            主窗口ID_z = GetInstanceID();
            远近点下拉列表ID_z = $"{主窗口ID_z}: {远近点下拉列表ID_z}".GetHashCode();
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Home))
            {
                主窗口开关_z = !主窗口开关_z;
            }
        }
        private void OnGUI()
        {
            if (!主窗口开关_z) { return; }

            窗口区域_z = GUILayout.Window(
                     主窗口ID_z,
                     窗口区域_z,
                     内容,
                     "",
                    UI样式管理器.窗口样式_h
                 );

            if (远近点下拉列表开关_z)
            {
                GUI.BringWindowToFront(远近点下拉列表ID_z);

                GUILayout.Window(远近点下拉列表ID_z, 远近点下拉列表区域_z,
                (int windowID) =>
                {
                    GUILayout.BeginVertical(UI样式管理器.浅背景中对齐样式_h);
                    {
                        if (GUILayout.Button(面板类型.船只平面夹角相位调整.ToString(), UI样式管理器.按钮样式_h))
                        {
                            当前面板类型_z = 面板类型.船只平面夹角相位调整;
                            远近点下拉列表开关_z = false;
                        }
                        if (GUILayout.Button(面板类型.船只平近点角相位调整.ToString(), UI样式管理器.按钮样式_h))
                        {
                            当前面板类型_z = 面板类型.船只平近点角相位调整;
                            远近点下拉列表开关_z = false;
                        }
                        if (GUILayout.Button(面板类型.静止轨道投影经度调整.ToString(), UI样式管理器.按钮样式_h))
                        {
                            当前面板类型_z = 面板类型.静止轨道投影经度调整;
                            远近点下拉列表开关_z = false;
                        }
                    }
                    GUILayout.EndVertical();
                }, "", UI样式管理器.窗口样式_h);
            }
        }
        private void 内容(int windowID)
        {
            // GUILayout的绘制函数并不是直接绘制(所有带有自动布局功能的GUI都有类似的处理流程), 而是按照调用顺序生成 <位置/尺寸/内容/UI状态/UI事件处理函数回调> 的UI消息树
            // 注: 在开始本帧的绘制流程前, 会从消息队列中取出所有输入设备消息, 以上帧的消息树为基准, 从根节点向下递归转发消息给各级的UI事件处理函数回调, 然后开始UI消息树更新的三次流程
            // 第一次从UI消息树根节点向下递归, 根据最新内容计算出实际尺寸并更新
            // 第二次从UI消息树尾节点向上递归, 每遇到一个水平布局或者垂直布局就重排所有下级的位置(使用相对位置)
            // 元素位置 = 上一个子级元素位置 + 上一个子级元素尺寸    注: 首个子级元素位置 = 父级位置 + 0(这里的0在支持配置的布局中也叫内容区偏移)
            // 第二次递归流程完成后, 所有UI的位置与尺寸都确定了, 此时开始第三次递归正式绘制

            // 标题栏
            GUILayout.BeginHorizontal(UI样式管理器.浅背景中对齐样式_h);
            {
                GUILayout.Label("    轨道相位调整计算器", UI样式管理器.无背景中对齐样式_h);
                if (GUILayout.Button("×", UI样式管理器.按钮样式_h, GUILayout.Width(20)))
                {
                    主窗口开关_z = false;
                }
            }
            GUILayout.EndHorizontal();

            if (GUILayout.Button($"↓  {当前面板类型_z}  ↓", UI样式管理器.按钮样式_h))
            {
                远近点下拉列表开关_z = true;
            }

            // EventType.Repaint就是读取控件区域进行绘制的阶段了, 但是由于布局是相对坐标系
            // 因此在还没有绘制到下位列表前, 提供下拉列表的位置还来的及
            if (Event.current.type == EventType.Repaint)
            {
                // GUILayoutUtility.GetLastRect()指的是上一个控件的位置, 既控制远近点下拉列表开关_z的按钮
                远近点下拉列表区域_z = GUILayoutUtility.GetLastRect();
                远近点下拉列表区域_z.x += 窗口区域_z.x;
                远近点下拉列表区域_z.y += 窗口区域_z.y;
            }

            switch (当前面板类型_z)
            {
                case 面板类型.船只平面夹角相位调整:
                    {
                        渲染船只平面夹角相位调整面板();
                    }
                    break;
                case 面板类型.船只平近点角相位调整:
                    {
                        渲染船只平近点角相位调整面板();
                    }
                    break;
                case 面板类型.静止轨道投影经度调整:
                    {
                        渲染静止轨道投影经度调整面板();
                    }
                    break;
            }

            GUI.DragWindow();     // 此拖动窗口UI消息最后入消息树因此最先处理, 若存在鼠标拖动则以上帧的UI消息树为事件处理基准变更本帧的面板根节点位置
        }
        private void 渲染船只平面夹角相位调整面板()
        {
            Vessel 受控船只, 目标船只;
            double 世界时, 受控远点, 受控近点, 目标远点, 目标近点;
            Orbit 受控船只轨道, 目标船只轨道;
            初始化船只轨道(out 受控船只, out 目标船只, out 世界时, out 受控船只轨道, out 受控远点, out 受控近点, out 目标船只轨道, out 目标远点, out 目标近点);

            if (受控船只 && 目标船只)
            {
                渲染船只轨道(受控船只, 目标船只, 受控远点, 受控近点, 目标远点, 目标近点, 受控船只轨道, 目标船只轨道);

                if (受控船只轨道.referenceBody == 目标船只轨道.referenceBody)
                {
                    GUILayout.BeginVertical(UI样式管理器.浅背景中对齐样式_h);
                    {
                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.Label("设定理想相位角: ", UI样式管理器.输入框头样式_h);
                            目标相位角_w = GUILayout.TextField(目标相位角_w, UI样式管理器.输入框体样式_h);
                        }
                        GUILayout.EndHorizontal();

                        if (!double.TryParse(目标相位角_w, out var 理想相位角值))
                        {
                            GUILayout.Label("请输入有效数字！", UI样式管理器.无背景左对齐样式_h);
                        }
                        else
                        {
                            理想相位角值 = 扩展方法.循环取值范围_正360(理想相位角值);
                            GUILayout.Label($"输入结果: {理想相位角值}°", UI样式管理器.无背景左对齐样式_h);

                            var 到达近点世界时 = 受控船只轨道.timeToPe + 世界时;
                            var 到达近点相位角 = 受控船只轨道.计算目标相位角(目标船只轨道, 到达近点世界时);

                            var 到达远点世界时 = 受控船只轨道.timeToAp + 世界时;
                            var 到达远点相位角 = 受控船只轨道.计算目标相位角(目标船只轨道, 到达远点世界时);

                            if (GUILayout.Button("计算", UI样式管理器.按钮样式_h))
                            {
                                var (当前轨道真近点角, 目标轨道真近点角) = 当前轨道_z.计算远近点下点火处真近点角(最终轨道_z);

                                if (当前轨道真近点角 == -1 && 目标轨道真近点角 == -1 || 当前轨道真近点角 == 0 && Math.Abs(受控近点 - 目标近点) > 1 || 当前轨道真近点角 > 0 && Math.Abs(受控远点 - 目标远点) > 1)
                                {
                                    var 日志内容 = $"相位变更采用在原地等待一圈并圆轨的方式\n请将远点或者近点设置为参考船只的远点或者近点(误差小于1)\n[{当前轨道真近点角},{目标轨道真近点角}]";
                                    InstallChecker.弹出日志面板(日志内容);
                                }
                                else
                                {
                                    var ΔV = Vector3d.zero;
                                    if (当前轨道真近点角 == 0)
                                    {
                                        ΔV = 船只轨道计算.计算指定相位调整量的转移轨道所需速度(当前轨道_z, 最终轨道_z, 理想相位角值 - 到达近点相位角, 受控船只轨道, 到达近点世界时, 当前轨道真近点角);
                                        // InstallChecker.弹出报警日志面板($"近点点火转移轨道速度: {ΔV}");
                                        if (ΔV != Vector3d.zero)
                                        { var 节点 = 机动节点管理器.创建机动节点(受控船只, 受控船只轨道, ΔV, 到达近点世界时); }
                                    }
                                    else
                                    {
                                        ΔV = 船只轨道计算.计算指定相位调整量的转移轨道所需速度(当前轨道_z, 最终轨道_z, 理想相位角值 - 到达远点相位角, 受控船只轨道, 到达远点世界时, 当前轨道真近点角);
                                        // InstallChecker.弹出报警日志面板($"远点点火转移轨道速度: {ΔV}");
                                        if (ΔV != Vector3d.zero)
                                        { var 节点 = 机动节点管理器.创建机动节点(受控船只, 受控船只轨道, ΔV, 到达远点世界时); }
                                    }
                                }
                            }

                            GUILayout.Label($"实时目标相位角: {受控船只轨道.计算目标相位角(目标船只轨道, 世界时):F2}°", UI样式管理器.无背景左对齐样式_h);
                            GUILayout.Label($"到达近点时目标相位角: {到达近点相位角:F2}°", UI样式管理器.无背景左对齐样式_h);
                            GUILayout.Label($"到达远点时目标相位角: {到达远点相位角:F2}°", UI样式管理器.无背景左对齐样式_h);
                        }
                    }
                    GUILayout.EndVertical();
                }
                else
                {
                    GUILayout.Label($"报警日志: 船只与目标船只的轨道必须在同一中心天体", UI样式管理器.无背景左对齐样式_h);
                }
            }
            else if (受控船只 && !目标船只)
            {
                渲染船只轨道(受控船只, 目标船只, 受控远点, 受控近点, 目标远点, 目标近点, 受控船只轨道, 目标船只轨道);
            }
            else
            {
                GUILayout.Label($"受控船只: 无", UI样式管理器.无背景左对齐样式_h);
                GUILayout.Label($"目标船只: 无", UI样式管理器.无背景左对齐样式_h);
            }


        }
        private void 渲染船只平近点角相位调整面板()
        {
            Vessel 受控船只, 目标船只;
            double 世界时, 受控远点, 受控近点, 目标远点, 目标近点;
            Orbit 受控船只轨道, 目标船只轨道;
            初始化船只轨道(out 受控船只, out 目标船只, out 世界时, out 受控船只轨道, out 受控远点, out 受控近点, out 目标船只轨道, out 目标远点, out 目标近点);

            if (受控船只 && 目标船只)
            {
                渲染船只轨道(受控船只, 目标船只, 受控远点, 受控近点, 目标远点, 目标近点, 受控船只轨道, 目标船只轨道);

                if (受控船只轨道.referenceBody == 目标船只轨道.referenceBody)
                {
                    GUILayout.BeginVertical(UI样式管理器.浅背景中对齐样式_h);
                    {
                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.Label("设定理想平近点角相位: ", UI样式管理器.输入框头样式_h);
                            目标平近点角相位_w = GUILayout.TextField(目标平近点角相位_w, UI样式管理器.输入框体样式_h);
                        }
                        GUILayout.EndHorizontal();

                        if (!double.TryParse(目标平近点角相位_w, out var 理想平近点角相位值))
                        {
                            GUILayout.Label("请输入有效数字！", UI样式管理器.无背景左对齐样式_h);
                        }
                        else
                        {
                            理想平近点角相位值 = 扩展方法.循环取值范围_正360(理想平近点角相位值);
                            GUILayout.Label($"输入结果: {理想平近点角相位值}", UI样式管理器.无背景左对齐样式_h);

                            var 到达近点世界时 = 受控船只轨道.timeToPe + 世界时;
                            var 到达远点世界时 = 受控船只轨道.timeToAp + 世界时;

                            var 到达近点船只平近点角 = 受控船只轨道.计算指定时间下平近点角(到达近点世界时);
                            var 到达远点船只平近点角 = 受控船只轨道.计算指定时间下平近点角(到达远点世界时);

                            var 到达近点目标平近点角 = 目标船只轨道.计算指定时间下平近点角(到达近点世界时);
                            var 到达远点目标平近点角 = 目标船只轨道.计算指定时间下平近点角(到达远点世界时);

                            var 实时船只平近点角 = 受控船只轨道.计算指定时间下平近点角(世界时);
                            var 实时目标平近点角 = 目标船只轨道.计算指定时间下平近点角(世界时);

                            if (GUILayout.Button("计算", UI样式管理器.按钮样式_h))
                            {
                                var (当前轨道真近点角, 目标轨道真近点角) = 当前轨道_z.计算远近点下点火处真近点角(最终轨道_z);

                                if (当前轨道真近点角 == -1 && 目标轨道真近点角 == -1 || 当前轨道真近点角 == 0 && Math.Abs(受控近点 - 目标近点) > 1 || 当前轨道真近点角 > 0 && Math.Abs(受控远点 - 目标远点) > 1)
                                {
                                    var 日志内容 = $"相位变更采用在原地等待一圈并圆轨的方式\n请将远点或者近点设置为参考船只的远点或者近点(误差小于1)\n[{当前轨道真近点角},{目标轨道真近点角}]";
                                    InstallChecker.弹出日志面板(日志内容);
                                }
                                else
                                {
                                    var ΔV = 船只轨道计算.计算指定相位调整量的转移轨道所需速度(当前轨道_z, 最终轨道_z, -扩展方法.循环取值范围_正360(到达近点目标平近点角 + 理想平近点角相位值), 受控船只轨道, 到达近点世界时, 0);
                                    // InstallChecker.弹出报警日志面板($"近点点火转移轨道速度: {ΔV}");
                                    if (ΔV != Vector3d.zero)
                                    {
                                        var 节点 = 机动节点管理器.创建机动节点(受控船只, 受控船只轨道, ΔV, 到达近点世界时);
                                    }
                                }
                            }

                            GUILayout.Label($"实时船只平近点角: {实时船只平近点角:F2}°", UI样式管理器.无背景左对齐样式_h);
                            GUILayout.Label($"实时目标平近点角: {实时目标平近点角:F2}°", UI样式管理器.无背景左对齐样式_h);
                            GUILayout.Label($"实时平近点角相位: {实时船只平近点角 - 实时目标平近点角:F2}°", UI样式管理器.无背景左对齐样式_h);
                            var __近点 = 到达近点船只平近点角 - 到达近点目标平近点角;
                            GUILayout.Label($"到达近点时平近点角相位: {((__近点 == 360.0) ? 0 : __近点):F2}°", UI样式管理器.无背景左对齐样式_h);
                            var __远点 = 到达远点船只平近点角 - 到达远点目标平近点角;
                            GUILayout.Label($"到达远点时平近点角相位: {((__远点 == 360.0) ? 0 : __远点):F2}°", UI样式管理器.无背景左对齐样式_h);
                        }
                    }
                    GUILayout.EndVertical();
                }
                else
                {
                    GUILayout.Label($"报警日志: 船只与目标船只的轨道必须在同一中心天体", UI样式管理器.无背景左对齐样式_h);
                }
            }
            else if (受控船只 && !目标船只)
            {
                渲染船只轨道(受控船只, 目标船只, 受控远点, 受控近点, 目标远点, 目标近点, 受控船只轨道, 目标船只轨道);
            }
            else
            {
                GUILayout.Label($"受控船只: 无", UI样式管理器.无背景左对齐样式_h);
                GUILayout.Label($"目标船只: 无", UI样式管理器.无背景左对齐样式_h);
            }
        }
        private void 渲染静止轨道投影经度调整面板()
        {
            Vessel 受控船只, 目标船只;
            double 世界时, 受控远点, 受控近点, 目标远点, 目标近点;
            Orbit 受控船只轨道, 目标船只轨道;
            初始化船只轨道(out 受控船只, out 目标船只, out 世界时, out 受控船只轨道, out 受控远点, out 受控近点, out 目标船只轨道, out 目标远点, out 目标近点);
            目标船只 = null;
            var 静止轨道高度 = 轨道定义.计算静止轨道海拔高度(中心天体_z);
            最终轨道_z.初始化(静止轨道高度, 静止轨道高度, 中心天体_z);

            if (受控船只)
            {
                渲染船只轨道(受控船只, 目标船只, 受控远点, 受控近点, 目标远点, 目标近点, 受控船只轨道, 目标船只轨道);

                GUILayout.BeginVertical(UI样式管理器.浅背景中对齐样式_h);
                {
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Label("设定理想经度: ", UI样式管理器.输入框头样式_h);
                        目标经度_w = GUILayout.TextField(目标经度_w, UI样式管理器.输入框体样式_h);
                    }
                    GUILayout.EndHorizontal();

                    if (!double.TryParse(目标经度_w, out var 理想新经度值))
                    {
                        GUILayout.Label("请输入有效数字！", UI样式管理器.无背景左对齐样式_h);
                    }
                    else
                    {
                        理想新经度值 = 扩展方法.循环取值范围_正360(理想新经度值);
                        GUILayout.Label($"输入结果: {理想新经度值}", UI样式管理器.无背景左对齐样式_h);

                        var 到达近点世界时 = 受控船只轨道.timeToPe + 世界时;
                        var 到达远点世界时 = 受控船只轨道.timeToAp + 世界时;

                        var 实时投影经度 = 受控船只轨道.计算指定时间下船只投影经度(世界时, 世界时);
                        var 实时经度相位误差 = 理想新经度值 - 实时投影经度;

                        var 到达近点时投影经度 = 受控船只轨道.计算指定时间下船只投影经度(到达近点世界时, 世界时);
                        var 到达远点时投影经度 = 受控船只轨道.计算指定时间下船只投影经度(到达远点世界时, 世界时);

                        if (GUILayout.Button("计算", UI样式管理器.按钮样式_h))
                        {
                            var (当前轨道真近点角, 目标轨道真近点角) = 当前轨道_z.计算远近点下点火处真近点角(最终轨道_z);

                            if (当前轨道真近点角 == -1 && 目标轨道真近点角 == -1 || 当前轨道真近点角 == 0 && Math.Abs(受控近点 - 最终轨道_z.Pe海拔近点_z) > 1 || 当前轨道真近点角 > 0 && Math.Abs(受控远点 - 最终轨道_z.Ap海拔远点_z) > 1)
                            {
                                var 日志内容 = $"相位变更采用在原地等待一圈并圆轨的方式\n请将远点或者近点设置为参考船只的远点或者近点(误差小于1)\n[{当前轨道真近点角},{目标轨道真近点角}]";
                                InstallChecker.弹出日志面板(日志内容);
                            }
                            else
                            {
                                var ΔV = Vector3d.zero;
                                if (当前轨道真近点角 == 0)
                                {
                                    ΔV = 船只轨道计算.计算指定相位调整量的转移轨道所需速度(当前轨道_z, 最终轨道_z, 到达近点时投影经度 - 理想新经度值, 受控船只轨道, 到达近点世界时, 当前轨道真近点角);
                                    // InstallChecker.弹出报警日志面板($"近点点火转移轨道速度: {ΔV}");
                                    if (ΔV != Vector3d.zero)
                                    { var 节点 = 机动节点管理器.创建机动节点(受控船只, 受控船只轨道, ΔV, 到达近点世界时); }
                                }
                                else
                                {
                                    ΔV = 船只轨道计算.计算指定相位调整量的转移轨道所需速度(当前轨道_z, 最终轨道_z, 到达远点时投影经度 - 理想新经度值, 受控船只轨道, 到达远点世界时, 当前轨道真近点角);
                                    // InstallChecker.弹出报警日志面板($"远点点火转移轨道速度: {ΔV}");
                                    if (ΔV != Vector3d.zero)
                                    { var 节点 = 机动节点管理器.创建机动节点(受控船只, 受控船只轨道, ΔV, 到达远点世界时); }
                                }
                            }
                        }

                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.BeginVertical();
                            {
                                GUILayout.Label("实时投影经度:\t", UI样式管理器.无背景左对齐样式_h);
                                GUILayout.Label("实时经度相位误差:\t", UI样式管理器.无背景左对齐样式_h);
                                GUILayout.Label("到达近点时投影经度:\t", UI样式管理器.无背景左对齐样式_h);
                                GUILayout.Label("到达远点时投影经度:\t", UI样式管理器.无背景左对齐样式_h);
                            }
                            GUILayout.EndVertical();

                            GUILayout.BeginVertical();
                            {
                                GUILayout.Label($"{实时投影经度:F2}°", UI样式管理器.无背景右对齐样式_h);
                                GUILayout.Label($"{实时经度相位误差:F2}°", UI样式管理器.无背景右对齐样式_h);
                                GUILayout.Label($"{到达近点时投影经度:F2}°", UI样式管理器.无背景右对齐样式_h);
                                GUILayout.Label($"{到达远点时投影经度:F2}°", UI样式管理器.无背景右对齐样式_h);
                            }
                            GUILayout.EndVertical();

                            GUILayout.Space(4);

                            GUILayout.BeginVertical();
                            {
                                GUILayout.Label(扩展方法.换算_角度转经度(实时投影经度), UI样式管理器.无背景右对齐样式_h);
                                GUILayout.Label(扩展方法.换算_角度转经度(实时经度相位误差), UI样式管理器.无背景右对齐样式_h);
                                GUILayout.Label(扩展方法.换算_角度转经度(到达近点时投影经度), UI样式管理器.无背景右对齐样式_h);
                                GUILayout.Label(扩展方法.换算_角度转经度(到达远点时投影经度), UI样式管理器.无背景右对齐样式_h);
                            }
                            GUILayout.EndVertical();
                        }
                        GUILayout.EndHorizontal();
                    }
                }
                GUILayout.EndVertical();
            }
            else
            {
                GUILayout.Label($"受控船只: 无", UI样式管理器.无背景左对齐样式_h);
            }
        }
        static void 渲染船只轨道(Vessel 受控船只_c, Vessel 目标船只_c, double 受控远点_c, double 受控近点_c, double 目标远点_c, double 目标近点_c, Orbit 受控船只轨道_c, Orbit 目标船只轨道_c)
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.BeginHorizontal(UI样式管理器.浅背景中对齐样式_h);
                {
                    GUILayout.BeginVertical();
                    {
                        GUILayout.Label("受控船只:\t", UI样式管理器.无背景左对齐样式_h);
                        GUILayout.Label("远点:\t", UI样式管理器.无背景左对齐样式_h);
                        GUILayout.Label("近点:\t", UI样式管理器.无背景左对齐样式_h);
                        GUILayout.Label("轨道周期:\t", UI样式管理器.无背景左对齐样式_h);
                        GUILayout.Label("中心天体:\t", UI样式管理器.无背景左对齐样式_h);
                    }
                    GUILayout.EndVertical();

                    GUILayout.BeginVertical();
                    {
                        GUILayout.Label(受控船只_c.GetName(), UI样式管理器.无背景右对齐样式_h);
                        GUILayout.Label(扩展方法.换算_米转自适应单位(受控远点_c), UI样式管理器.无背景右对齐样式_h);
                        GUILayout.Label(扩展方法.换算_米转自适应单位(受控近点_c), UI样式管理器.无背景右对齐样式_h);
                        GUILayout.Label(扩展方法.换算_秒转日期(受控船只轨道_c.period), UI样式管理器.无背景右对齐样式_h);
                        GUILayout.Label(受控船只轨道_c.referenceBody.bodyName, UI样式管理器.无背景右对齐样式_h);
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndHorizontal();

                if (目标船只_c)
                {
                    GUILayout.BeginHorizontal(UI样式管理器.浅背景中对齐样式_h);
                    {
                        GUILayout.BeginVertical();
                        {
                            GUILayout.Label("目标船只:\t", UI样式管理器.无背景左对齐样式_h);
                            GUILayout.Label("远点:\t", UI样式管理器.无背景左对齐样式_h);
                            GUILayout.Label("近点:\t", UI样式管理器.无背景左对齐样式_h);
                            GUILayout.Label("轨道周期:\t", UI样式管理器.无背景左对齐样式_h);
                            GUILayout.Label("中心天体:\t", UI样式管理器.无背景左对齐样式_h);
                        }
                        GUILayout.EndVertical();

                        GUILayout.BeginVertical();
                        {
                            GUILayout.Label(目标船只_c.GetName(), UI样式管理器.无背景右对齐样式_h);
                            GUILayout.Label(扩展方法.换算_米转自适应单位(目标远点_c), UI样式管理器.无背景右对齐样式_h);
                            GUILayout.Label(扩展方法.换算_米转自适应单位(目标近点_c), UI样式管理器.无背景右对齐样式_h);
                            GUILayout.Label(扩展方法.换算_秒转日期(目标船只轨道_c.period), UI样式管理器.无背景右对齐样式_h);
                            GUILayout.Label(目标船只轨道_c.referenceBody.bodyName, UI样式管理器.无背景右对齐样式_h);
                        }
                        GUILayout.EndVertical();
                    }
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(UI样式管理器.浅背景中对齐样式_h);
            {
                GUILayout.BeginVertical();
                {
                    GUILayout.Label("静止轨道高度:\t", UI样式管理器.无背景左对齐样式_h);
                    GUILayout.Label("天体自转周期:\t", UI样式管理器.无背景左对齐样式_h);
                }
                GUILayout.EndVertical();

                GUILayout.BeginVertical();
                {
                    GUILayout.Label($"{轨道定义.计算静止轨道海拔高度(中心天体_z):N2}m", UI样式管理器.无背景右对齐样式_h);
                    GUILayout.Label(扩展方法.换算_秒转日期(中心天体_z.自转周期_z), UI样式管理器.无背景右对齐样式_h);
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();

        }
        static void 初始化船只轨道(out Vessel 受控船只_c, out Vessel 目标船只_c, out double 世界时_c, out Orbit 受控船只轨道_c, out double 受控远点_c, out double 受控近点_c, out Orbit 目标船只轨道_c, out double 目标远点_c, out double 目标近点_c)
        {
            受控船只_c = FlightGlobals.ActiveVessel;
            目标船只_c = null;
            世界时_c = 0.0;
            受控船只轨道_c = null;
            (受控远点_c, 受控近点_c) = (0.0, 0.0);
            目标船只轨道_c = null;
            (目标远点_c, 目标近点_c) = (0.0, 0.0);

            if (受控船只_c)
            {
                目标船只_c = 受控船只_c.targetObject as Vessel;
                世界时_c = Planetarium.GetUniversalTime();

                受控船只轨道_c = 受控船只_c.orbit;
                (受控远点_c, 受控近点_c) = (受控船只轨道_c.ApA, 受控船只轨道_c.PeA);

                中心天体_z.初始化(受控船只轨道_c.referenceBody);
                当前轨道_z.初始化(受控远点_c, 受控近点_c, 中心天体_z);
            }

            if (目标船只_c)
            {
                目标船只轨道_c = 目标船只_c.GetOrbit();
                (目标远点_c, 目标近点_c) = (目标船只轨道_c.ApA, 目标船只轨道_c.PeA);

                最终轨道_z.初始化(目标远点_c, 目标近点_c, 中心天体_z);
            }
        }
        enum 面板类型 { 未初始化, 船只平面夹角相位调整, 船只平近点角相位调整, 静止轨道投影经度调整 }
        public class 配置读写器类 : PluginConfiguration
        {
            public 配置读写器类(string pathToFile) : base(pathToFile) { }
        }
        static PluginConfiguration 硬盘上的配置文件读写器_z = null;
        static PluginConfiguration 配置读写器_h
        {
            get
            {
                if (硬盘上的配置文件读写器_z == null)
                {
                    var 路径 = Path.Combine(Path.GetFullPath(KSPUtil.ApplicationRootPath), "GameData/GuiDao/Config/config.xml").Replace('/', Path.DirectorySeparatorChar);
                    硬盘上的配置文件读写器_z = new 配置读写器类(路径);
                }

                return 硬盘上的配置文件读写器_z;
            }
        }
        static int 主窗口ID_z;
        static int 远近点下拉列表ID_z;
        static Rect 窗口区域_z = new();   // 窗口区域: 窗口的位置与尺寸
        static Rect 远近点下拉列表区域_z = new();
        static bool 主窗口开关_z = true;
        static bool 远近点下拉列表开关_z = false;
        static 面板类型 当前面板类型_z = 面板类型.船只平面夹角相位调整;
        static 轨道定义 当前轨道_z = new();
        static 天体定义 中心天体_z = new();
        static 轨道定义 最终轨道_z = new();
        static string 目标相位角_w = "";
        static string 目标平近点角相位_w = "";
        static string 目标经度_w = "";
    }
}


