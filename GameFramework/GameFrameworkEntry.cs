using System;
using System.Collections.Generic;

namespace GameFramework
{
    public static class GameFrameworkEntry
    {
        private const string GameFrameworkVersion = "1.0.0";
        private static readonly LinkedList<GameFrameworkModule> s_GameFrameworkModules = new LinkedList<GameFrameworkModule>();

        public static string Version
        {
            get
            {
                return GameFrameworkVersion;
            }
        }

        public static void Update(float elapseSeconds, float realElapseSeconds)
        {
            foreach (GameFrameworkModule module in s_GameFrameworkModules)
            {
                module.Update(elapseSeconds, realElapseSeconds);
            }
        }

        public static void Shutdown()
        {
            for (LinkedListNode<GameFrameworkModule> current = s_GameFrameworkModules.Last; current != null; current = current.Previous)
            {
                current.Value.Shutdown();
            }

            s_GameFrameworkModules.Clear();
            Log.SetLogHelper(null);
        }

        public static T GetModule<T>()where T : class
        {
            Type interfaceType = typeof(T);
            if (!interfaceType.IsInterface)
            {
                throw new GameFrameworkException(string.Format("You must get module by interface, but '{0}' is not.", interfaceType.FullName));
            }

            if (!interfaceType.FullName.StartsWith("GameFramework."))
            {
                throw new GameFrameworkException(string.Format("You must get a Game Framework module, but '{0}' is not.", interfaceType.FullName));
            }

            string moduleName = string.Format("{0}.{1}", interfaceType.Namespace, interfaceType.Name.Substring(1));
            Type moduleType = Type.GetType(moduleName);
            if (moduleType == null)
            {
                throw new GameFrameworkException(string.Format("Can not find Game Framework module type '{0}'.", moduleName));
            }

            return GetModule(moduleType) as T;
        }

        private static GameFrameworkModule GetModule(Type moduleType)
        {
            foreach (GameFrameworkModule module in s_GameFrameworkModules)
            {
                if (module.GetType() == moduleType)
                {
                    return module;
                }
            }
            return CreateModule(moduleType);
        }

        private static GameFrameworkModule CreateModule(Type moduleType)
        {
            GameFrameworkModule module = (GameFrameworkModule)Activator.CreateInstance(moduleType);
            if (module == null)
            {
                throw new GameFrameworkException(string.Format("Can not create module '{0}'.", module.GetType().FullName));
            }

            LinkedListNode<GameFrameworkModule> current = s_GameFrameworkModules.First;
            while(current != null)
            {
                if(module.Priority > current.Value.Priority)
                {
                    break;
                }

                current = current.Next;
            }

            if (current != null)
            {
                s_GameFrameworkModules.AddBefore(current, module);
            } else
            {
                s_GameFrameworkModules.AddLast(module);
            }

            return module;
        }
    }
}