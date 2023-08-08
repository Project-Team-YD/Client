using System;
using System.Threading;

namespace HSMLibrary.Generics
{
    public class Singleton<T> where T : class
    {
        private static readonly Lazy<T>
            instance = new Lazy<T>(CreateInstanceOf, LazyThreadSafetyMode.ExecutionAndPublication);

        private static T CreateInstanceOf()
        {
            return Activator.CreateInstance(typeof(T), true) as T;
        }

        public static T getInstance { get { return instance.Value; } }

        protected Singleton()
        {
        }

        ~Singleton()
        {
        }
    }
}