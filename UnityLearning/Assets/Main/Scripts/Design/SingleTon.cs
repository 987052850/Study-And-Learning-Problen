using System;

namespace TEN.DESIGNMODEL
{
    public abstract class Singleton<T> where T : class
    {
        // ��ֻ̬���������ڴ洢����ʵ��
        private static T _instance = null;

        // ������ȷ���̰߳�ȫ
        private static readonly object _lock = new object();

        // �����Ĺ��캯���������޷����ⲿʵ����
        protected Singleton() { }

        // �������ԣ����ڻ�ȡ����ʵ��
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
                            // ʹ�÷��䴴��ʵ��
                            _instance = Activator.CreateInstance(typeof(T), true) as T;
                        }
                    }
                }
                return _instance;
            }
        }
    }
}

