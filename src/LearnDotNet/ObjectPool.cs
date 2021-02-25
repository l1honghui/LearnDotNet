using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    /// <summary>
    /// 对象池
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DefaultObjectPool<T>
    {
        private readonly ConcurrentBag<T> _objects;
        private readonly Func<T> _objectGenerator;

        public DefaultObjectPool(Func<T> objectGenerator)
        {
            _objects = new ConcurrentBag<T>();
            _objectGenerator = objectGenerator ?? throw new ArgumentNullException("_objectGenerator is null");
        }

        /// <summary>
        /// 获取对象
        /// </summary>
        /// <returns></returns>
        public T GetObject()
        {
            if (_objects.TryTake(out var item)) return item;
            return _objectGenerator();
        }

        /// <summary>
        /// 添加对象
        /// </summary>
        /// <param name="item"></param>
        public void PutObject(T item)
        {
            _objects.Add(item);
        }
    }
}
