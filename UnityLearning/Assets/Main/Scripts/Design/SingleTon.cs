using System;

namespace TEN.DESIGNMODEL
{
    public abstract class Singleton<T> where T : class
    {
        // 静态只读变量用于存储单例实例
        private static T _instance = null;

        // 锁对象，确保线程安全
        private static readonly object _lock = new object();

        // 保护的构造函数，子类无法在外部实例化
        protected Singleton() { }

        // 公共属性，用于获取单例实例
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            // 使用反射创建实例
                            _instance = Activator.CreateInstance(typeof(T), true) as T;
                        }
                    }
                }
                return _instance;
            }
        }
    }
}

