using System;
using UnityEngine;

namespace meanran_xuexi_mods
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class 轨道相位调整计算器 : MonoBehaviour
    {
        private Rect 窗口尺寸 = new Rect(20, 20, 300, 80);
        private bool 活跃么 = true;
        private string 相位角输入 = "";
        private 轨道定义 当前轨道 = new 轨道定义();
        private 轨道定义 最终轨道 = new 轨道定义();
        private 天体定义 中心天体 = new 天体定义();
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Home))
            {
                活跃么 = !活跃么;
            }
        }
        private void OnGUI()
        {
            if (!活跃么) { return; }

            窗口尺寸 = GUILayout.Window(
                      GetInstanceID(),
                      窗口尺寸,
                      内容,
                      "轨道相位调整计算器",
                      GUI.skin.window
                  );
        }
        private void 内容(int windowID)
        {
            // OnGui的绘制函数并不是直接绘制, 而是创建UI消息树, 然后从下至上递归并排序(因为需要先计算出文字(图片)的宽高)
            // 如果遇到上一级有垂直布局或水平布局, 会对下一级重新排序(使用相对位置, 因此不需要对下下一级排序)
            // 比如第一个子元素的位置 = 相对0, 第二个子元素的位置 = 子元素1位置 +子元素1宽高, 依次类推
            // 得到一个排序完了的 位置/宽高/采样图/UI状态Union/UI类型 的UI消息树后, 从鼠标键盘消息队列中取出消息, 并递归传递给UI消息树用于处理事件

            var 受控船只 = FlightGlobals.ActiveVessel;
            var 目标船只 = 受控船只.targetObject as Vessel;

            if (受控船只 == null || 目标船只 == null) { return; }

            var 受控船只轨道 = 受控船只.orbit;
            var 目标船只轨道 = 目标船只.GetOrbit();

            var 世界时 = Planetarium.GetUniversalTime();

            var 到达近点世界时 = 受控船只轨道.timeToPe + 世界时;
            var 到达近点相位角 = 受控船只轨道.计算目标相位角(目标船只轨道, 到达近点世界时);

            var 到达远点世界时 = 受控船只轨道.timeToAp + 世界时;
            var 到达远点相位角 = 受控船只轨道.计算目标相位角(目标船只轨道, 到达远点世界时);

            var 受控远点 = 受控船只轨道.ApA;
            var 受控近点 = 受控船只轨道.PeA;

            var 目标远点 = 目标船只轨道.ApA;
            var 目标近点 = 目标船只轨道.PeA;

            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical();
            GUILayout.Label($"受控船只: {受控船只.GetName()}");
            GUILayout.Label($"远点: {扩展方法.换算_米转自适应单位(受控远点)}");
            GUILayout.Label($"近点: {扩展方法.换算_米转自适应单位(受控近点)}");
            GUILayout.Label($"轨道周期: {受控船只轨道.period:N3}s");
            GUILayout.Label($"轨道周期: {扩展方法.换算_秒转日期(受控船只轨道.period, 3)}");
            GUILayout.Label($"中心天体: {受控船只轨道.referenceBody.bodyName}");
            GUILayout.EndVertical();

            GUILayout.Space(5);

            GUILayout.BeginVertical();
            GUILayout.Label($"目标船只: {目标船只.GetName()}");
            GUILayout.Label($"远点: {扩展方法.换算_米转自适应单位(目标远点)}");
            GUILayout.Label($"近点: {扩展方法.换算_米转自适应单位(目标近点)}");
            GUILayout.Label($"轨道周期: {目标船只轨道.period:N3}s");
            GUILayout.Label($"轨道周期: {扩展方法.换算_秒转日期(目标船只轨道.period, 3)}");
            GUILayout.Label($"中心天体: {目标船只轨道.referenceBody.bodyName}");
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            GUILayout.BeginVertical();
            GUILayout.Space(5);
            GUILayout.Label("设定理想相位角:");
            相位角输入 = GUILayout.TextField(相位角输入, GUILayout.Width(300));

            if (double.TryParse(相位角输入, out double 理想相位角))
            {
                理想相位角 %= 360;
                GUILayout.Label($"输入结果: {理想相位角}");
            }
            else
            {
                GUILayout.Label("请输入有效数字！");
            }

            中心天体.初始化(受控船只轨道.referenceBody);
            当前轨道.初始化(受控远点, 受控近点, 中心天体);
            最终轨道.初始化(目标远点, 目标近点, 中心天体);

            if (GUILayout.Button("计算"))
            {
                var (当前轨道真近点角, 目标轨道真近点角) = 当前轨道.计算远近点下点火处真近点角(最终轨道);

                if (当前轨道真近点角 == -1 && 目标轨道真近点角 == -1 || 当前轨道真近点角 == 0 && Math.Abs(受控近点 - 目标近点) > 1 || 当前轨道真近点角 > 0 && Math.Abs(受控远点 - 目标远点) > 1)
                {
                    var 日志内容 = $"相位变更采用在原地等待一圈并圆轨的方式\n请将远点或者近点设置为参考船只的远点或者近点(误差小于1)\n{当前轨道真近点角}  {目标轨道真近点角}";
                    InstallChecker.弹出报警日志面板(日志内容);
                }
                else
                {
                    if (当前轨道真近点角 == 0)
                    {
                        var ΔV = 船只轨道计算.计算指定相位调整量的转移轨道所需速度(当前轨道, 最终轨道, (理想相位角 - 到达近点相位角) % 360, 受控船只轨道, 到达近点世界时, 当前轨道真近点角);
                        // InstallChecker.弹出报警日志面板($"近点点火转移轨道速度: {ΔV}");
                        if (ΔV != Vector3d.zero)
                        { var 节点 = 机动节点.创建机动节点(受控船只, 受控船只轨道, ΔV, 到达近点世界时); }
                    }
                    else
                    {
                        var ΔV = 船只轨道计算.计算指定相位调整量的转移轨道所需速度(当前轨道, 最终轨道, (理想相位角 - 到达远点相位角) % 360, 受控船只轨道, 到达远点世界时, 当前轨道真近点角);
                        // InstallChecker.弹出报警日志面板($"远点点火转移轨道速度: {ΔV}");
                        if (ΔV != Vector3d.zero)
                        { var 节点 = 机动节点.创建机动节点(受控船只, 受控船只轨道, ΔV, 到达远点世界时); }
                    }
                }

            }

            GUILayout.Label($"目标相位角: {受控船只轨道.计算目标相位角(目标船只轨道, 世界时):F2}º");
            GUILayout.Label($"到达近点时目标相位角: {到达近点相位角:F2}º");
            GUILayout.Label($"到达远点时目标相位角: {到达远点相位角:F2}º");
            GUILayout.Label($"静止轨道高度: {轨道定义.计算静止轨道海拔高度(中心天体):N3}");
            GUILayout.Label($"天体自转周期: {扩展方法.换算_秒转日期(中心天体.自转周期, 3)}");

            // GUILayout.Label($"远点速度: {当前轨道.ApV质心远点速度}");
            // GUILayout.Label($"近点速度: {当前轨道.PeV质心近点速度}");
            // GUILayout.Label($"偏心率: {当前轨道.e偏心率}  ,  {当前轨道.e偏心率_旧}");
            // GUILayout.Label($"半短轴: {当前轨道.半短轴}");

            // GUILayout.Label($"牛顿万有引力常数: {中心天体.天体.gravParameter / 中心天体.天体.Mass}");
            // GUILayout.Label($"天体质心引力常数_这是μ么: {中心天体.天体.gravParameter}");
            // GUILayout.Label($"天体质心引力常数_实时计算: {中心天体.μ}");

            GUILayout.EndVertical();

            // 允许拖动窗口
            GUI.DragWindow();
        }
    }


}