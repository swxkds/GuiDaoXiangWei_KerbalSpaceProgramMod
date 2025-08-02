using System;

namespace meanran_xuexi_mods
{
    public class 天体定义
    {
        public CelestialBody 天体;
        public double 半径;
        public double 质量;
        public double 自转周期;
        public double 大气层海拔高度;
        public double SOI;  //  SOI是以天体质心为基准的一个球形区域半径
        public const double G = 6.67408E-11;    // G = 牛顿万有引力常数 Newtons Gravitational Constant     
        public void 初始化(CelestialBody 天体)
        {
            this.天体 = 天体;
            this.半径 = 天体.Radius;
            this.质量 = 天体.Mass;
            this.自转周期 = 天体.rotationPeriod;
            this.大气层海拔高度 = 天体.atmosphereDepth;
            this.SOI = 天体.sphereOfInfluence;
        }
        public double μ => this.质量 * G;   // 质心引力常数
        public double SOI海拔高度 => this.SOI - this.半径; // 希尔球的海拔半径(游戏中的近点和远点使用海拔高度)

    }
    public class 轨道定义
    {
        // 游戏中的轨道近点和轨道远点使用海拔高度
        public double Ap海拔远点;
        public double Pe海拔近点;
        public 天体定义 中心天体;
        public const double 三分之一 = 1.0 / 3.0;
        public void 初始化(double Ap海拔远点, double Pe海拔近点, 天体定义 中心天体)
        {
            this.Ap海拔远点 = Ap海拔远点;
            this.Pe海拔近点 = Pe海拔近点;
            this.中心天体 = 中心天体;
        }
        public double μ { get { return this.中心天体.μ; } }
        public double 天体半径 { get { return this.中心天体.半径; } }
        public double 长轴 { get { return this.Ap海拔远点 + this.Pe海拔近点 + 2 * this.天体半径; } }
        public double 半长轴 { get { return this.长轴 / 2; } }
        public double Ap质心远点 { get { return this.Ap海拔远点 + this.天体半径; } }
        public double Pe质心近点 { get { return this.Pe海拔近点 + this.天体半径; } }

        // 活力公式变形, r = 质心高度, 质心近点和质心远点就是质心高度
        public double PeV质心近点速度 { get { return Math.Sqrt((2 * this.Ap质心远点 * this.μ) / (this.长轴 * this.Pe质心近点)); } }

        // 活力公式变形, r = 质心高度, 质心近点和质心远点就是质心高度
        public double ApV质心远点速度 { get { return Math.Sqrt((2 * this.Pe质心近点 * this.μ) / (this.长轴 * this.Ap质心远点)); } }

        // 活力公式变形(半长轴与速度的关系)后代入远近点偏心率公式, 然后化简
        public double e偏心率 { get { return (this.Pe质心近点 * Math.Pow(this.PeV质心近点速度, 2) / this.μ) - 1; } }

        // 远近点偏心率公式
        public double e偏心率_旧 { get { return (this.Ap质心远点 - this.Pe质心近点) / this.长轴; } }
        public double 短轴 { get { return this.长轴 * Math.Sqrt(1 - (Math.Pow(this.e偏心率, 2))); } }
        public double 半短轴 { get { return this.短轴 / 2; } }
        public double F半焦距 { get { return Math.Sqrt(Math.Pow(this.半长轴, 2) - Math.Pow(this.半短轴, 2)); } }

        // 开普勒第三定律轨道周期T与半长轴a的关系公式
        public double T轨道周期 { get { return 2 * Math.PI * Math.Sqrt(Math.Pow(this.半长轴, 3) / this.μ); } }
        public string T轨道周期_日期 { get { return 扩展方法.换算_秒转日期(T轨道周期, 3); } }
        public void SetAp海拔远点(double value)
        {
            this.Ap海拔远点 = value;
            if (this.Ap海拔远点 < this.Pe海拔近点)
            {
                扩展方法.Swap(ref this.Ap海拔远点, ref this.Pe海拔近点);
            }
        }
        public void SetPe海拔近点(double value)
        {
            this.Pe海拔近点 = value;
            if (this.Pe海拔近点 > this.Ap海拔远点)
            {
                扩展方法.Swap(ref this.Ap海拔远点, ref this.Pe海拔近点);
            }
        }
        public static double Cbrt(double value) { return Math.Pow(value, 三分之一); }   // 立方根计算
        public void 设置指定长轴下的Ap海拔远点(double 新长轴) { this.SetAp海拔远点(this.Ap海拔远点 + (新长轴 - this.长轴)); }   // 将轨道周期增量带来的长轴变化应用到远点或近点
        public void 设置指定长轴下的Pe海拔近点(double 新长轴) { this.SetPe海拔近点(this.Pe海拔近点 + (新长轴 - this.长轴)); }
        public static double 计算指定轨道周期下的半长轴_旧(天体定义 中心天体, double T)
        {
            // 开普勒第三定律轨道周期T与半长轴a的关系公式的变形
            var 半长轴 = Cbrt(Math.Pow(T / (2 * Math.PI), 2) * 中心天体.μ);
            return 半长轴;
        }
        public static double 计算指定轨道周期下的半长轴(天体定义 中心天体, double T)
        {
            // 开普勒第三定律轨道周期T与半长轴a的关系公式的变形
            var 半长轴 = Cbrt((中心天体.μ * Math.Pow(T, 2)) / 39.4784176);  // 39.4784176 = 4π^2
            return 半长轴;
        }
        public static double 计算指定轨道周期下的长轴(天体定义 中心天体, double T)
        {
            var 长轴 = 2 * 计算指定轨道周期下的半长轴(中心天体, T);
            return 长轴;
        }
        public static double 计算静止轨道海拔高度(天体定义 中心天体)
        {
            if (中心天体.自转周期 != 0)
            {
                var 半长轴 = 计算指定轨道周期下的半长轴(中心天体, 中心天体.自转周期);
                var 海拔高度 = 半长轴 - 中心天体.半径;
                return (海拔高度 >= 中心天体.SOI海拔高度) ? 0 : 海拔高度;
            }
            else { return 0; }
        }
        public static double 计算LOS最小海拔高度(天体定义 中心天体, int 共轨卫星数量)
        {
            // 请画一个正多边形, 然后画内切圆, 圆心到任一顶点的连线就是轨道质心高度, 然后作直角三角形, 轨道半径就是临边的长度
            if (共轨卫星数量 >= 3)
            {
                var 三角形顶点质心高度 = 中心天体.半径 / (Math.Cos(0.5 * (2.0 * Math.PI / 共轨卫星数量)));
                var LOS最小海拔高度 = 三角形顶点质心高度 - 中心天体.半径;
                return (LOS最小海拔高度 <= 中心天体.大气层海拔高度) ? 中心天体.大气层海拔高度 + 100 : LOS最小海拔高度; // 最小LOS高度不能低于大气层高度
            }
            else { return 0; }
        }

        public (double, double) 计算远近点下点火处真近点角(轨道定义 b)
        {
            // 180° = π radians; 轨道升降到超过原来的远近点时, 远近点会交换位置, 真近点角同时逆转180度

            double 当前轨道真近点角 = -1;
            double 目标轨道真近点角 = -1;

            if (Math.Abs(this.Ap海拔远点 - b.Ap海拔远点) <= 5)       // 老远点 = 新远点 , 目标真近点角 = 180
            {
                当前轨道真近点角 = Math.PI;
                目标轨道真近点角 = Math.PI;
            }
            else if (Math.Abs(this.Pe海拔近点 - b.Ap海拔远点) <= 5)    // 老近点 = 新远点 , 目标真近点角 = 180
            {
                当前轨道真近点角 = 0;
                目标轨道真近点角 = Math.PI;
            }
            else if (Math.Abs(this.Pe海拔近点 - b.Pe海拔近点) <= 5)    // 老近点 = 新近点  , 目标真近点角 = 0
            {
                当前轨道真近点角 = 0;
                目标轨道真近点角 = 0;
            }
            else if (Math.Abs(this.Ap海拔远点 - b.Pe海拔近点) <= 5)     // 老远点 = 新近点 , 目标真近点角 = 0
            {
                当前轨道真近点角 = Math.PI;
                目标轨道真近点角 = 0;
            }

            return (当前轨道真近点角, 目标轨道真近点角);
        }
    }
}
