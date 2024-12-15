using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TEN.MANAGER
{
    /// <summary>
    ///项目 : TEN
    ///日期：2024/12/14 14:58:12 
    ///创建者：Michael Corleone
    ///类用途：对象池
    /// </summary>
    public class ObjectPool<T , DataType>
        where T : Component
        where DataType : GLOBAL.STRUCT.SInterface
    {
        public int MaxSize { get; set; } = 10;
        public int Count { get; set; } = 0;
        public delegate GameObject CreateObject(DataType vIn_Data) ;
        public CreateObject OnCreate;

        private Queue<T> _objectPool;
        private object _lock = new object();
        public ObjectPool()
        {
            _objectPool = new Queue<T>();
        }
        ~ObjectPool()
        {
            if (_objectPool.Count >= 0)
            {
                Reset();
            }
            _objectPool = null;
        }
        public void Init(int vIn_MaxSize , List<DataType> pIn_DataTypes)
        {
            MaxSize = vIn_MaxSize;
            int dataCount = pIn_DataTypes.Count;
            for (int i = 0; i < vIn_MaxSize; i++)
            {
                if (i>= dataCount)
                {
                    return;
                }
                GameObject gameObject = OnCreate?.Invoke(pIn_DataTypes[i]);
                Enqueue(gameObject.GetComponent<T>());
            }
        }
        public void Reset()
        {
            for (int i = 0; i < _objectPool.Count; i++)
            {
                UnityEngine.Object temp = _objectPool.Dequeue();
                GameObject.Destroy(temp);
            }
        }
        public void Enqueue(T vIn_Object)
        {
            if (Count >= MaxSize)
            {
                GameObject.Destroy(vIn_Object.gameObject);
                return;
            }
            lock (_lock)
            {
                _objectPool.Enqueue(vIn_Object);
                Count++;
            }
        }
        public T Dequeue(DataType vIn_Data)
        {
            if (Count <= 0)
            {

            }
            lock (_lock)
            {
                Count--;
                return _objectPool.Dequeue();
            }
        }
    }
}
