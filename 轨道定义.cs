using System;

namespace meanran_xuexi_mods
{
    public class 天体定义
    {
        public CelestialBody 天体_z;
        public double 半径_z;
        public double 质量_z;
        public double 自转周期_z;
        public double 大气层海拔高度_z;
        public double SOI_z;  //  SOI是以天体质心为基准的一个球形区域半径
        public const double G_z = 6.67408E-11;    // G = 牛顿万有引力常数 Newtons Gravitational Constant     
        public void 初始化(CelestialBody 天体_c)
        {
            this.天体_z = 天体_c;
            this.半径_z = 天体_c.Radius;
            this.质量_z = 天体_c.Mass;
            this.自转周期_z = 天体_c.rotationPeriod;
            this.大气层海拔高度_z = 天体_c.atmosphereDepth;
            this.SOI_z = 天体_c.sphereOfInfluence;
        }
        public double μ_h => this.质量_z * G_z;   // 质心引力常数
        public double SOI海拔高度_h => this.SOI_z - this.半径_z; // 希尔球的海拔半径(游戏中的近点和远点使用海拔高度)
    }
    public class 轨道定义
    {
        // 游戏中的轨道近点和轨道远点使用海拔高度
        public double Ap海拔远点_z;
        public double Pe海拔近点_z;
        public 天体定义 中心天体_z;
        public const double 三分之一_z = 1.0 / 3.0;
        public void 初始化(double Ap海拔远点_c, double Pe海拔近点_c, 天体定义 中心天体_c)
        {
            this.Ap海拔远点_z = Ap海拔远点_c;
            this.Pe海拔近点_z = Pe海拔近点_c;
            this.中心天体_z = 中心天体_c;
        }
        public double μ_h { get { return this.中心天体_z.μ_h; } }
        public double 天体半径_h { get { return this.中心天体_z.半径_z; } }
        public double 长轴_h { get { return this.Ap海拔远点_z + this.Pe海拔近点_z + 2 * this.天体半径_h; } }
        public double 半长轴_h { get { return this.长轴_h / 2; } }
        public double Ap质心远点_h { get { return this.Ap海拔远点_z + this.天体半径_h; } }
        public double Pe质心近点_h { get { return this.Pe海拔近点_z + this.天体半径_h; } }

        // 活力公式变形, r = 质心高度, 质心近点和质心远点就是质心高度
        public double PeV质心近点速度_h { get { return Math.Sqrt((2 * this.Ap质心远点_h * this.μ_h) / (this.长轴_h * this.Pe质心近点_h)); } }

        // 活力公式变形, r = 质心高度, 质心近点和质心远点就是质心高度
        public double ApV质心远点速度_h { get { return Math.Sqrt((2 * this.Pe质心近点_h * this.μ_h) / (this.长轴_h * this.Ap质心远点_h)); } }

        // 活力公式变形(半长轴与速度的关系)后代入远近点偏心率公式, 然后化简
        public double e偏心率_h { get { return (this.Pe质心近点_h * Math.Pow(this.PeV质心近点速度_h, 2) / this.μ_h) - 1; } }

        // 远近点偏心率公式
        public double e偏心率_旧_h { get { return (this.Ap质心远点_h - this.Pe质心近点_h) / this.长轴_h; } }
        public double 短轴_h { get { return this.长轴_h * Math.Sqrt(1 - (Math.Pow(this.e偏心率_h, 2))); } }
        public double 半短轴_h { get { return this.短轴_h / 2; } }
        public double F半焦距_h { get { return Math.Sqrt(Math.Pow(this.半长轴_h, 2) - Math.Pow(this.半短轴_h, 2)); } }

        // 开普勒第三定律轨道周期T与半长轴a的关系公式
        public double T轨道周期_h { get { return 2 * Math.PI * Math.Sqrt(Math.Pow(this.半长轴_h, 3) / this.μ_h); } }
        public string T轨道周期_日期_h { get { return 扩展方法.换算_秒转日期(T轨道周期_h, 3); } }
        public void SetAp海拔远点(double value_c)
        {
            this.Ap海拔远点_z = value_c;
            if (this.Ap海拔远点_z < this.Pe海拔近点_z)
            {
                扩展方法.Swap(ref this.Ap海拔远点_z, ref this.Pe海拔近点_z);
            }
        }
        public void SetPe海拔近点(double value_c)
        {
            this.Pe海拔近点_z = value_c;
            if (this.Pe海拔近点_z > this.Ap海拔远点_z)
            {
                扩展方法.Swap(ref this.Ap海拔远点_z, ref this.Pe海拔近点_z);
            }
        }
        public static double Cbrt(double value_c) { return Math.Pow(value_c, 三分之一_z); }   // 立方根计算
        public void 设置指定长轴下的Ap海拔远点(double 新长轴_c) { this.SetAp海拔远点(this.Ap海拔远点_z + (新长轴_c - this.长轴_h)); }   // 将轨道周期增量带来的长轴变化应用到远点或近点
        public void 设置指定长轴下的Pe海拔近点(double 新长轴_c) { this.SetPe海拔近点(this.Pe海拔近点_z + (新长轴_c - this.长轴_h)); }
        public static double 计算指定轨道周期下的半长轴_旧(天体定义 中心天体_c, double T_c)
        {
            // 开普勒第三定律轨道周期T与半长轴a的关系公式的变形
            var 半长轴 = Cbrt(Math.Pow(T_c / (2 * Math.PI), 2) * 中心天体_c.μ_h);
            return 半长轴;
        }
        public static double 计算指定轨道周期下的半长轴(天体定义 中心天体_c, double T_c)
        {
            // 开普勒第三定律轨道周期T与半长轴a的关系公式的变形
            var 半长轴 = Cbrt((中心天体_c.μ_h * Math.Pow(T_c, 2)) / 39.4784176);  // 39.4784176 = 4π^2
            return 半长轴;
        }
        public static double 计算指定轨道周期下的长轴(天体定义 中心天体_c, double T_c)
        {
            var 长轴 = 2 * 计算指定轨道周期下的半长轴(中心天体_c, T_c);
            return 长轴;
        }
        public static double 计算静止轨道海拔高度(天体定义 中心天体_c)
        {
            if (中心天体_c.自转周期_z != 0)
            {
                var 半长轴 = 计算指定轨道周期下的半长轴(中心天体_c, 中心天体_c.自转周期_z);
                var 海拔高度 = 半长轴 - 中心天体_c.半径_z;
                return (海拔高度 >= 中心天体_c.SOI海拔高度_h) ? 0 : 海拔高度;
            }
            else { return 0; }
        }
        public static double 计算LOS最小海拔高度(天体定义 中心天体_c, int 共轨卫星数量_c)
        {
            // 请画一个正多边形, 然后画内切圆, 圆心到任一顶点的连线就是轨道质心高度, 然后作直角三角形, 轨道半径就是临边的长度
            if (共轨卫星数量_c >= 3)
            {
                var 三角形顶点质心高度 = 中心天体_c.半径_z / (Math.Cos(0.5 * (2.0 * Math.PI / 共轨卫星数量_c)));
                var LOS最小海拔高度 = 三角形顶点质心高度 - 中心天体_c.半径_z;
                return (LOS最小海拔高度 <= 中心天体_c.大气层海拔高度_z) ? 中心天体_c.大气层海拔高度_z + 100 : LOS最小海拔高度; // 最小LOS高度不能低于大气层高度
            }
            else { return 0; }
        }

        public (double, double) 计算远近点下点火处真近点角(轨道定义 b_c)
        {
            // 180° = π radians; 轨道升降到超过原来的远近点时, 远近点会交换位置, 真近点角同时逆转180度

            double 当前轨道真近点角 = -1;
            double 目标轨道真近点角 = -1;

            if (Math.Abs(this.Ap海拔远点_z - b_c.Ap海拔远点_z) <= 5)       // 老远点 = 新远点 , 目标真近点角 = 180
            {
                当前轨道真近点角 = Math.PI;
                目标轨道真近点角 = Math.PI;
            }
            else if (Math.Abs(this.Pe海拔近点_z - b_c.Ap海拔远点_z) <= 5)    // 老近点 = 新远点 , 目标真近点角 = 180
            {
                当前轨道真近点角 = 0;
                目标轨道真近点角 = Math.PI;
            }
            else if (Math.Abs(this.Pe海拔近点_z - b_c.Pe海拔近点_z) <= 5)    // 老近点 = 新近点  , 目标真近点角 = 0
            {
                当前轨道真近点角 = 0;
                目标轨道真近点角 = 0;
            }
            else if (Math.Abs(this.Ap海拔远点_z - b_c.Pe海拔近点_z) <= 5)     // 老远点 = 新近点 , 目标真近点角 = 0
            {
                当前轨道真近点角 = Math.PI;
                目标轨道真近点角 = 0;
            }

            return (当前轨道真近点角, 目标轨道真近点角);
        }
    }
}
