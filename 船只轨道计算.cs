using System;

namespace meanran_xuexi_mods
{
    public static class 船只轨道计算
    {
        public static Vector3d Horizontal(this Orbit 船只轨道_c, double 世界时_c)
        {
            return Vector3d.Exclude(船只轨道_c.Up(世界时_c), 船只轨道_c.Prograde(世界时_c)).normalized;
        }
        public static Vector3d Prograde(this Orbit 船只轨道_c, double 世界时_c)
        {
            return 船只轨道_c.SwappedOrbitalVelocityAtUT(世界时_c).normalized;
        }
        public static Vector3d Up(this Orbit 船只轨道_c, double 世界时_c)
        {
            return 船只轨道_c.SwappedRelativePositionAtUT(世界时_c).normalized;
        }
        public static Vector3d RadialPlus(this Orbit 船只轨道_c, double 世界时_c)
        {
            return Vector3d.Exclude(船只轨道_c.Prograde(世界时_c), 船只轨道_c.Up(世界时_c)).normalized;
        }
        public static Vector3d SwappedOrbitNormal(this Orbit 船只轨道_c)
        {
            return -SwapYZ(船只轨道_c.GetOrbitNormal()).normalized;
        }
        public static Vector3d NormalPlus(this Orbit 船只轨道_c, double 世界时_c)
        {
            return 船只轨道_c.SwappedOrbitNormal();
        }
        public static Vector3d Reorder(this Vector3d vector_c, int order_c)
        {
            switch (order_c)
            {
                case 123:
                    return new Vector3d(vector_c.x, vector_c.y, vector_c.z);
                case 132:
                    return new Vector3d(vector_c.x, vector_c.z, vector_c.y);
                case 213:
                    return new Vector3d(vector_c.y, vector_c.x, vector_c.z);
                case 231:
                    return new Vector3d(vector_c.y, vector_c.z, vector_c.x);
                case 312:
                    return new Vector3d(vector_c.z, vector_c.x, vector_c.y);
                case 321:
                    return new Vector3d(vector_c.z, vector_c.y, vector_c.x);
            }
            throw new ArgumentException("Invalid order", "order");
        }
        public static Vector3d SwapYZ(Vector3d value_c)
        {
            return value_c.Reorder(132);
        }

        // 获取指定时间下的轨道速度(轨道坐标和世界坐标对于X/Y/Z保存到Vector3d的顺序并不一致,需要转换)
        public static Vector3d SwappedOrbitalVelocityAtUT(this Orbit 船只轨道_c, double 世界时_c)
        {
            return SwapYZ(船只轨道_c.getOrbitalVelocityAtUT(世界时_c));
        }

        // 获取指定时间点下船只相对于天体质心的坐标(轨道坐标和世界坐标对于X/Y/Z保存到Vector3d的顺序并不一致,需要转换)
        public static Vector3d SwappedRelativePositionAtUT(this Orbit 船只轨道_c, double 世界时_c)
        {
            return SwapYZ(船只轨道_c.getRelativePositionAtUT(世界时_c));
        }

        // 获取指定时间点下船只与天体质心之间的长度, 即轨道海拔高度
        public static double Radius(this Orbit 船只轨道_c, double 世界时_c)
        {
            return 船只轨道_c.SwappedRelativePositionAtUT(世界时_c).magnitude;
        }

        // 计算在指定轨道质心高度下正圆轨道的轨道速度 v = sqrt(GM/r)   注: 轨道半径 = 质心高度
        public static double 计算指定质心高度下正圆轨道所需速度(CelestialBody 中心天体_c, double 轨道半径_c)
        {
            return Math.Sqrt(中心天体_c.gravParameter / 轨道半径_c);
        }

        public static Vector3d 计算指定时间下正圆轨道所需速度(Orbit 船只轨道_c, double 世界时_c)
        {
            // 模长(所需速度) * 单位方向向量 = 速度向量
            Vector3d 需求速度 = 计算指定质心高度下正圆轨道所需速度(船只轨道_c.referenceBody, 船只轨道_c.Radius(世界时_c)) * Horizontal(船只轨道_c, 世界时_c);
            Vector3d 实际速度 = 船只轨道_c.SwappedOrbitalVelocityAtUT(世界时_c);
            return 需求速度 - 实际速度;
        }
        public static Vector3d 计算在海拔远点时圆轨所需速度(Vessel 船只_c)
        {
            var 到达远点世界时 = Planetarium.GetUniversalTime() + 船只_c.orbit.timeToAp;
            var ΔV = 计算指定时间下正圆轨道所需速度(船只_c.orbit, 到达远点世界时);
            return ΔV;
        }
        public static Vector3d 计算在海拔近点时圆轨所需速度(Vessel 船只_c)
        {
            var 到达近点世界时 = Planetarium.GetUniversalTime() + 船只_c.orbit.timeToPe;
            var ΔV = 计算指定时间下正圆轨道所需速度(船只_c.orbit, 到达近点世界时);
            return ΔV;
        }

        // 圆锥曲线的极坐标方程     注: 轨道半径 = 质心高度     注: 此方程使用真近点角定位椭圆上的某点, 并计算该点与天体质心之间的高度
        public static double 计算指定真近点角下的椭圆轨道半径(轨道定义 轨道_c, double 真近点角_c)
        {
            var a = 轨道_c.半长轴_h; var e = 轨道_c.e偏心率_h; var θ = 真近点角_c;
            var 轨道半径 = a * (1 - Math.Pow(e, 2)) / (1 + (e * Math.Cos(θ)));
            return 轨道半径;
        }

        // 活力公式  注: 轨道半径 = 质心高度
        public static double 计算指定质心高度下椭圆轨道所需速度(轨道定义 轨道_c, double 轨道半径_c)
        {
            var μ = 轨道_c.μ_h; var r = 轨道半径_c; var a = 轨道_c.半长轴_h;
            var 轨道速度 = Math.Sqrt(μ * ((2 / r) - (1 / a)));
            return 轨道速度;
        }
        public static double 计算目标轨道所需速度(轨道定义 当前轨道_c, 轨道定义 目标轨道_c, double 点火处_当前轨道真近点角_c, double 点火处_目标轨道真近点角_c)
        {
            // 目标轨道是在当前轨道的某点上进行点火加减速后的轨道, 因此两处真近点角其实是指向同一个位置(但是两者近点的位置不一定一样, 比如目标远点低于近点, 原来的近点变成远点)

            var r1 = 计算指定真近点角下的椭圆轨道半径(当前轨道_c, 点火处_当前轨道真近点角_c);
            var v1 = 计算指定质心高度下椭圆轨道所需速度(当前轨道_c, r1);

            var r2 = 计算指定真近点角下的椭圆轨道半径(目标轨道_c, 点火处_目标轨道真近点角_c);
            var v2 = 计算指定质心高度下椭圆轨道所需速度(目标轨道_c, r2);

            var ΔV = v2 - v1;

            InstallChecker.弹出日志面板($"[当前ΔV: {v1}\n目标ΔV: {v2}\n目标远近点: [{目标轨道_c.Ap海拔远点_z:N2} , {目标轨道_c.Pe海拔近点_z:N2}]\n目标周期: {扩展方法.换算_秒转日期(目标轨道_c.T轨道周期_h)}]", "机动节点日志");

            return ΔV;
        }
        public static Vector3d 计算指定相位调整量的转移轨道所需速度(轨道定义 当前轨道_c, 轨道定义 最终轨道_c, double 相位调整量_c, Orbit 船只轨道_c, double 世界时_c, double 点火时间真近点角_c)
        {
            var T = 最终轨道_c.T轨道周期_h;
            var 转移轨道周期 = 相位调整量_c / (360.0 / T) + T;

            var 转移轨道 = new 轨道定义();
            转移轨道.初始化(当前轨道_c.Ap海拔远点_z, 当前轨道_c.Pe海拔近点_z, 当前轨道_c.中心天体_z);
            var 新长轴 = 轨道定义.计算指定轨道周期下的长轴(转移轨道.中心天体_z, 转移轨道周期);

            if (点火时间真近点角_c == 0) { 转移轨道.设置指定长轴下的Ap海拔远点(新长轴); }
            else { 转移轨道.设置指定长轴下的Pe海拔近点(新长轴); }

            if (转移轨道.Ap海拔远点_z >= 转移轨道.中心天体_z.SOI海拔高度_h)
            {
                InstallChecker.弹出日志面板($"转移轨道远点超出SOI高度");
                return Vector3d.zero;
            }
            else if (转移轨道.Pe海拔近点_z <= 0)
            {
                InstallChecker.弹出日志面板($"转移轨道近点在天体内部");
                return Vector3d.zero;
            }
            else if (转移轨道.中心天体_z.大气层海拔高度_z > 0)
            {
                if (转移轨道.Pe海拔近点_z <= 转移轨道.中心天体_z.大气层海拔高度_z)
                {
                    InstallChecker.弹出日志面板($"转移轨道近点在大气层内部");
                    return Vector3d.zero;
                }
            }

            var (当前轨道真近点角, 目标轨道真近点角) = 当前轨道_c.计算远近点下点火处真近点角(转移轨道);
            if (当前轨道真近点角 == -1 && 目标轨道真近点角 == -1)
            {
                var 日志内容 = $"转移轨道计算失败\n{当前轨道真近点角}  {目标轨道真近点角}";
                InstallChecker.弹出日志面板(日志内容);
                return Vector3d.zero;
            }

            // InstallChecker.弹出报警日志面板($"目标远近点: [{转移轨道.Ap海拔远点:N2} , {转移轨道.Pe海拔近点:N2} , 目标周期: [{扩展方法.换算_秒转日期(转移轨道.T轨道周期)} , {扩展方法.换算_秒转日期(转移轨道周期)}]");

            return 计算目标轨道所需速度(当前轨道_c, 转移轨道, 当前轨道真近点角, 目标轨道真近点角) * Horizontal(船只轨道_c, 世界时_c);
        }

        public static double 计算目标相位角(this Orbit a_c, Orbit b_c, double 世界时_c)
        {
            // a的轨道平面法线向量
            Vector3d vector3d = a_c.SwappedOrbitNormal();

            // a指定世界下的相对于天体的位置矢量
            Vector3d vector3d2 = a_c.SwappedRelativePositionAtUT(世界时_c);

            // 从b的位置矢量中移除法线方向的分量, 得到b位置在a轨道平面上的投影向量
            Vector3d vector3d3 = Vector3d.Exclude(vector3d, b_c.SwappedRelativePositionAtUT(世界时_c));

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
        public static double 计算指定时间下平近点角(this Orbit a_c, double 世界时_c)
        {
            // 返回值范围<0-360>, 不需要再额外修饰
            var 平近点角 = a_c.getMeanAnomalyAtUT(世界时_c).换算_弧度转角度();
            return 平近点角;
        }
        public static double 计算指定时间下船只投影经度(this Orbit 船只轨道_c, double 指定世界时_c, double 当前世界时_c)
        {
            // 计算船只在指定时间下的位置与当前时间下星球子午线之间的夹角, 即投影经度
            var 经度 = 船只轨道_c.referenceBody.GetLongitude(船只轨道_c.getPositionAtUT(指定世界时_c), false);  // 返回值不限制在360度范围内

            // 计算(指定时间-当前时间)下星球自转带来的星球子午线偏移量, 修正投影经度
            if (船只轨道_c.referenceBody.rotationPeriod != 0)
            {
                double 自转角速度 = 360.0 / 船只轨道_c.referenceBody.rotationPeriod;
                double 时间差 = 指定世界时_c - 当前世界时_c;
                double 自转偏移 = 自转角速度 * 时间差;
                经度 -= 自转偏移;
            }

            return 扩展方法.循环取值范围_正360(经度);
        }
    }
}