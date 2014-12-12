////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Apache License 2.0 (Apache)
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Reflection;
using System.Threading;
using System.Runtime.CompilerServices;

namespace System
{

    public sealed class AppDomain : MarshalByRefObject
    {
        [System.Reflection.FieldNoReflection]
        private object m_appDomain;
        private string m_friendlyName;

        private AppDomain()
        {
            throw new Exception();
        }

        
        public extern static AppDomain CreateDomain(String friendlyName);

        public Object CreateInstanceAndUnwrap(String assemblyName, String typeName)
        {
            Assembly assembly = Assembly.Load(assemblyName);
            Type type = assembly.GetType(typeName);

            ConstructorInfo ci = type.GetConstructor(new Type[0]);
            object obj = ci.Invoke(null);

            return obj;
        }

        public static AppDomain CurrentDomain
        {
            get { return Thread.GetDomain(); }
        }

        public String FriendlyName
        {
            get
            {
                return m_friendlyName;
            }
        }

        public Assembly Load(String assemblyString)
        {
            bool fVersion = false;
            int[] ver = new int[4];

            string name = Assembly.ParseAssemblyName(assemblyString, ref fVersion, ref ver);

            return LoadInternal(name, fVersion, ver[0], ver[1], ver[2], ver[3]);

        }

        
        public Assembly[] GetAssemblies()
        {
            throw new NotImplementedException();
        }

        
        private Assembly LoadInternal(String assemblyString, bool fVersion, int maj, int min, int build, int rev)
        {
            throw new NotImplementedException();
        }

        
        public static extern void Unload(AppDomain domain);
    }
}


