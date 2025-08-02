using System;
using System.Reflection;

namespace meanran_xuexi_mods
{
    public class 扫描MJ模组自动执行节点功能
    {
        System.Type CoreType;
        bool m_Mj启用标志 = false;
        public bool Mj启用标志
        {
            get { if (!初始化标志) { m_Mj启用标志 = init(); } return m_Mj启用标志; }
            set { m_Mj启用标志 = value; }
        }
        public bool 初始化标志 = false;
        public static Vessel 受控船只 { get { return FlightGlobals.ActiveVessel; } }
        PartModule Mj引用 = null;
        bool GetCore()
        {
            // PartModule: 代码组件
            foreach (Part p in 受控船只.parts)
            {
                foreach (PartModule module in p.Modules)
                {
                    if (module.GetType() == CoreType)
                    {
                        Mj引用 = module;
                        return true;
                    }
                }

            }
            return false;
        }

        System.Type FindMechJebModule(string module)
        {
            // Log.Info("FindMechJebModule");
            Type type = null;
            AssemblyLoader.loadedAssemblies.TypeOperation(t =>
            {
                if (t.FullName == module)
                {
                    type = t;
                }
            });

            return type;
        }
        public bool init()
        {
            // Log.Info("MechjebWrapper.init");
            if (初始化标志) { return Mj启用标志; }

            CoreType = FindMechJebModule("MuMech.MechJebCore");

            if (CoreType == null)
            {
                // Log.Info("MechJeb assembly not found");
                Mj启用标志 = false;
                return Mj启用标志;
            }
            if (!GetCore())
            {
                // Log.Info("MechJeb core not found");
                Mj启用标志 = false;
                return Mj启用标志;
            }

            // Log.Info("Found MechJeb core");
            初始化标志 = true;
            Mj启用标志 = true;
            return Mj启用标志;
        }

        // Mj模组的自动执行节点功能
        public void 自动执行一个节点()
        {
            var coreNodeInfo = CoreType.GetField("node");
            var coreNode = coreNodeInfo.GetValue(Mj引用);
            var NodeExecute = coreNode.GetType().GetMethod("ExecuteOneNode", BindingFlags.Public | BindingFlags.Instance);
            NodeExecute.Invoke(coreNode, new object[] { this });
        }



    }
}
