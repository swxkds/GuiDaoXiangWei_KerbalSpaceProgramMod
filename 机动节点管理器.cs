using System;

namespace meanran_xuexi_mods
{
    public class 机动节点管理器
    {
        public static Vector3d 换算_ΔV转机动节点向量(Orbit 船只轨道_c, double 世界时_c, Vector3d ΔV_c)
        {
            // 将世界坐标系中的DeltaV转换成机动节点上的径向、法向、切向值
            return new Vector3d(Vector3d.Dot(船只轨道计算.RadialPlus(船只轨道_c, 世界时_c), ΔV_c),
                                Vector3d.Dot(-船只轨道计算.NormalPlus(船只轨道_c, 世界时_c), ΔV_c),
                                Vector3d.Dot(船只轨道计算.Prograde(船只轨道_c, 世界时_c), ΔV_c));
        }

        // 几个模式分别是不显示任何轨道线，仅显示行星围绕恒星的轨道线，显示行星和船只的轨道线，以及显示机动节点的轨道线
        public static bool 查询追踪站是否解锁机动节点(Vessel 船只_c)
        {
            return GameVariables.Instance.GetOrbitDisplayMode(ScenarioUpgradeableFacilities.GetFacilityLevel(SpaceCenterFacility.TrackingStation)) == GameVariables.OrbitDisplayMode.PatchedConics;
        }
        public static ManeuverNode 创建机动节点(Vessel 船只_c, Orbit 船只轨道_c, Vector3d ΔV_c, double 世界时_c)
        {
            // 创建机动节点前需要提前计算好在指定时间下的ΔV向量
            if (查询追踪站是否解锁机动节点(船只_c))
            {
                // 检查ΔV是否是有效值(没有因为浮点数溢出而变成NaN或Infinity)
                for (int i = 0; i < 3; i++)
                {
                    if (double.IsNaN(ΔV_c[i]) || double.IsInfinity(ΔV_c[i]))
                    {
                        throw new Exception(InstallChecker.弹出日志面板($"ΔV值无法解析: {ΔV_c}"));
                    }
                }

                // 检查世界时是否是有效值(没有因为浮点数溢出而变成NaN或Infinity)
                if (double.IsNaN(世界时_c) || double.IsInfinity(世界时_c))
                {
                    throw new Exception(InstallChecker.弹出日志面板($"UT值无法解析: {世界时_c}"));
                }

                // 似乎有时在过去放置一个机动节点会导致游戏出现异常, 限制时间不得小于实际宇宙时间
                世界时_c = Math.Max(世界时_c, Planetarium.GetUniversalTime());

                // 机动节点坐标系 (x, y, z) = (径向+, 法向-, 顺向)
                Vector3d nodeΔV = 换算_ΔV转机动节点向量(船只轨道_c, 世界时_c, ΔV_c);

                // 这只是添加一个机动节点到飞行计划中，不会处理旧的机动节点
                var 机动节点 = 船只_c.patchedConicSolver.AddManeuverNode(世界时_c);
                机动节点.DeltaV = nodeΔV;

                // 必须更新飞行计划，否则机动节点在地图上不会同步更新
                船只_c.patchedConicSolver.UpdateFlightPlan();
                return 机动节点;
            }
            else
                return null;
        }
    }
}