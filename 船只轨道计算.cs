using System;

namespace meanran_xuexi_mods
{
    public static class 船只轨道计算
    {
        public static Vector3d Horizontal(this Orbit 船只轨道, double 世界时)
        {
            return Vector3d.Exclude(船只轨道.Up(世界时), 船只轨道.Prograde(世界时)).normalized;
        }
        public static Vector3d Prograde(this Orbit 船只轨道, double 世界时)
        {
            return 船只轨道.SwappedOrbitalVelocityAtUT(世界时).normalized;
        }
        public static Vector3d Up(this Orbit 船只轨道, double 世界时)
        {
            return 船只轨道.SwappedRelativePositionAtUT(世界时).normalized;
        }
        public static Vector3d RadialPlus(this Orbit 船只轨道, double 世界时)
        {
            return Vector3d.Exclude(船只轨道.Prograde(世界时), 船只轨道.Up(世界时)).normalized;
        }
        public static Vector3d SwappedOrbitNormal(this Orbit 船只轨道)
        {
            return -SwapYZ(船只轨道.GetOrbitNormal()).normalized;
        }
        public static Vector3d NormalPlus(this Orbit 船只轨道, double 世界时)
        {
            return 船只轨道.SwappedOrbitNormal();
        }
        public static Vector3d Reorder(this Vector3d vector, int order)
        {
            switch (order)
            {
                case 123:
                    return new Vector3d(vector.x, vector.y, vector.z);
                case 132:
                    return new Vector3d(vector.x, vector.z, vector.y);
                case 213:
                    return new Vector3d(vector.y, vector.x, vector.z);
                case 231:
                    return new Vector3d(vector.y, vector.z, vector.x);
                case 312:
                    return new Vector3d(vector.z, vector.x, vector.y);
                case 321:
                    return new Vector3d(vector.z, vector.y, vector.x);
            }
            throw new ArgumentException("Invalid order", "order");
        }
        public static Vector3d SwapYZ(Vector3d value)
        {
            return value.Reorder(132);
        }

        // 获取指定时间下的轨道速度(轨道坐标和世界坐标对于X/Y/Z保存到Vector3d的顺序并不一致,需要转换)
        public static Vector3d SwappedOrbitalVelocityAtUT(this Orbit 船只轨道, double 世界时)
        {
            return SwapYZ(船只轨道.getOrbitalVelocityAtUT(世界时));
        }

        // 获取指定时间点下船只相对于天体质心的坐标(轨道坐标和世界坐标对于X/Y/Z保存到Vector3d的顺序并不一致,需要转换)
        public static Vector3d SwappedRelativePositionAtUT(this Orbit 船只轨道, double 世界时)
        {
            return SwapYZ(船只轨道.getRelativePositionAtUT(世界时));
        }

        // 获取指定时间点下船只与天体质心之间的长度, 即轨道海拔高度
        public static double Radius(this Orbit 船只轨道, double 世界时)
        {
            return 船只轨道.SwappedRelativePositionAtUT(世界时).magnitude;
        }

        // 计算在指定轨道质心高度下正圆轨道的轨道速度 v = sqrt(GM/r)   注: 轨道半径 = 质心高度
        public static double 计算指定质心高度下正圆轨道所需速度(CelestialBody 中心天体, double 轨道半径)
        {
            return Math.Sqrt(中心天体.gravParameter / 轨道半径);
        }

        public static Vector3d 计算指定时间下正圆轨道所需速度(Orbit 船只轨道, double 世界时)
        {
            // 模长(所需速度) * 单位方向向量 = 速度向量
            Vector3d 需求速度 = 计算指定质心高度下正圆轨道所需速度(船只轨道.referenceBody, 船只轨道.Radius(世界时)) * Horizontal(船只轨道, 世界时);
            Vector3d 实际速度 = 船只轨道.SwappedOrbitalVelocityAtUT(世界时);
            return 需求速度 - 实际速度;
        }
        public static Vector3d 计算在海拔远点时圆轨所需速度(Vessel 船只)
        {
            var 到达远点世界时 = Planetarium.GetUniversalTime() + 船只.orbit.timeToAp;
            var ΔV = 计算指定时间下正圆轨道所需速度(船只.orbit, 到达远点世界时);
            return ΔV;
        }
        public static Vector3d 计算在海拔近点时圆轨所需速度(Vessel 船只)
        {
            var 到达近点世界时 = Planetarium.GetUniversalTime() + 船只.orbit.timeToPe;
            var ΔV = 计算指定时间下正圆轨道所需速度(船只.orbit, 到达近点世界时);
            return ΔV;
        }

        // 圆锥曲线的极坐标方程     注: 轨道半径 = 质心高度     注: 此方程使用真近点角定位椭圆上的某点, 并计算该点与天体质心之间的高度
        public static double 计算指定真近点角下的椭圆轨道半径(轨道定义 轨道, double 真近点角)
        {
            var a = 轨道.半长轴; var e = 轨道.e偏心率; var θ = 真近点角;
            var 轨道半径 = a * (1 - Math.Pow(e, 2)) / (1 + (e * Math.Cos(θ)));
            return 轨道半径;
        }

        // 活力公式  注: 轨道半径 = 质心高度
        public static double 计算指定质心高度下椭圆轨道所需速度(轨道定义 轨道, double 轨道半径)
        {
            var μ = 轨道.μ; var r = 轨道半径; var a = 轨道.半长轴;
            var 轨道速度 = Math.Sqrt(μ * ((2 / r) - (1 / a)));
            return 轨道速度;
        }
        public static double 计算目标轨道所需速度(轨道定义 当前轨道, 轨道定义 目标轨道, 天体定义 中心天体, double 点火处_当前轨道真近点角, double 点火处_目标轨道真近点角)
        {
            // 目标轨道是在当前轨道的某点上进行点火加减速后的轨道, 因此两处真近点角其实是指向同一个位置(但是两者近点的位置不一定一样, 比如目标远点低于近点, 原来的近点变成远点)

            var r1 = 计算指定真近点角下的椭圆轨道半径(当前轨道, 点火处_当前轨道真近点角);
            var v1 = 计算指定质心高度下椭圆轨道所需速度(当前轨道, r1);

            var r2 = 计算指定真近点角下的椭圆轨道半径(目标轨道, 点火处_目标轨道真近点角);
            var v2 = 计算指定质心高度下椭圆轨道所需速度(目标轨道, r2);

            var ΔV = v2 - v1;

            InstallChecker.弹出报警日志面板($"[当前ΔV: {v1}  ,  目标ΔV: {v2}  ,  {目标轨道.Ap海拔远点:N3}  ,  {目标轨道.Pe海拔近点:N3}  ,  目标周期: {扩展方法.换算_秒转日期(目标轨道.T轨道周期, 3)}]");

            return ΔV;
        }
        public static Vector3d 计算指定相位调整量的转移轨道所需速度(轨道定义 当前轨道, 轨道定义 最终轨道, double 相位调整量, Orbit 船只轨道, double 世界时, double 点火时间真近点角)
        {
            var T = 最终轨道.T轨道周期;
            var 转移轨道周期 = 相位调整量 / (360.0 / T) + T;

            var 转移轨道 = new 轨道定义();
            转移轨道.初始化(当前轨道.Ap海拔远点, 当前轨道.Pe海拔近点, 当前轨道.中心天体);
            var 新长轴 = 轨道定义.计算指定轨道周期下的长轴(转移轨道.中心天体, 转移轨道周期);

            if (点火时间真近点角 == 0) { 转移轨道.设置指定长轴下的Ap海拔远点(新长轴); }
            else { 转移轨道.设置指定长轴下的Pe海拔近点(新长轴); }

            if (转移轨道.Ap海拔远点 >= 转移轨道.中心天体.SOI海拔高度)
            {
                InstallChecker.弹出报警日志面板($"转移轨道远点超出SOI高度");
                return Vector3d.zero;
            }
            else if (转移轨道.Pe海拔近点 <= 0)
            {
                InstallChecker.弹出报警日志面板($"转移轨道近点在天体内部");
                return Vector3d.zero;
            }
            else if (转移轨道.中心天体.大气层海拔高度 > 0)
            {
                if (转移轨道.Pe海拔近点 <= 转移轨道.中心天体.大气层海拔高度)
                {
                    InstallChecker.弹出报警日志面板($"转移轨道近点在大气层内部");
                    return Vector3d.zero;
                }
            }

            var (当前轨道真近点角, 目标轨道真近点角) = 当前轨道.计算远近点下点火处真近点角(转移轨道);
            if (当前轨道真近点角 == -1 && 目标轨道真近点角 == -1)
            {
                var 日志内容 = $"转移轨道计算失败\n{当前轨道真近点角}  {目标轨道真近点角}";
                InstallChecker.弹出报警日志面板(日志内容);
                return Vector3d.zero;
            }

            // InstallChecker.弹出报警日志面板($"[{转移轨道.Ap海拔远点:N3}  ,  {转移轨道.Pe海拔近点:N3}  ,  长轴周期: {扩展方法.换算_秒转日期(转移轨道.T轨道周期,3)}  ,  预测周期: {扩展方法.换算_秒转日期(转移轨道周期,3)}]");

            return 计算目标轨道所需速度(当前轨道, 转移轨道, 当前轨道.中心天体, 当前轨道真近点角, 目标轨道真近点角) * Horizontal(船只轨道, 世界时);
        }

        public static double 计算目标相位角(this Orbit a, Orbit b, double 世界时)
        {
            // a的轨道平面法线向量
            Vector3d vector3d = a.SwappedOrbitNormal();

            // a指定世界下的相对于天体的位置矢量
            Vector3d vector3d2 = a.SwappedRelativePositionAtUT(世界时);

            // 从b的位置矢量中移除法线方向的分量, 得到b位置在a轨道平面上的投影向量
            Vector3d vector3d3 = Vector3d.Exclude(vector3d, b.SwappedRelativePositionAtUT(世界时));

            // 计算两向量夹角
            double num = Vector3d.Angle(vector3d2, vector3d3);

            // 叉积 (法线 × a位置) 得到a轨道的切向基准方向
            // 与投影向量点积判断投影在基准的哪一侧
            // 点积<0说明投影在基准的"负侧", 需要转换为360度补角(判断夹角方向（顺时针/逆时针）)
            if (Vector3d.Dot(Vector3d.Cross(vector3d, vector3d2), vector3d3) < 0.0)
            {
                num = 360.0 - num;
            }

            return num;
        }
    }
}