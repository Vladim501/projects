using System;
using System.Collections.Generic;
using System.Threading;

namespace GZipCompressor
{
    /// <summary>
    /// Для каждого элемента указывается порядковый номер.
    /// Очередь гарантирует, что элементы будут извлечены в соответствии с порядковыми номерами элементов, 
    /// не важно, в каком порядке они добавлялись в очередь. 
    /// </summary>
    /// <typeparam name="T">Тип элементов</typeparam>
    public class RedyQueue<T>
    {
        private readonly Dictionary<Queue_Order, T> queueDictionary
            = new Dictionary<Queue_Order, T>();

        private readonly Dictionary<int, int> subOrderLimits
            = new Dictionary<int, int>();

        private int currentOrder;

        private int currentSubOrder;

        private readonly object innerLock = new object();

        /// <summary>
        /// Размер очереди
        /// </summary>
        public int Size
        {
            get { return queueDictionary.Count; }
        }

        /// <summary>
        /// Помещаем элемент в очередь
        /// </summary>
        /// <param name="order">Порядковый номер элемента</param>
        /// <param name="item">Добавляемый элемент</param>
        public void Enqueue(int order, T item)
        {
            Enqueue(order, 0, item, true);
        }

        /// <summary>
        /// Занести элемент в очередь
        /// </summary>
        /// <param name="order">Основной порядковый номер элемента</param>
        /// <param name="subOrder">Второстепенный порядковый номер</param>
        /// <param name="item">Добавляемый элемент</param>
        /// <param name="lastSubOrder">
        /// Указанный второстепенный порядковый номер является последним
        /// для основного порядкового номера
        /// </param>
        public void Enqueue(int order, int subOrder, T item, bool lastSubOrder = false)
        {
            lock (innerLock)
            {
                var queueOrder = new Queue_Order(order, subOrder);

                if (queueDictionary.ContainsKey(queueOrder))
                    throw new Exception("Элемент с таким же порядком уже существует в очереди.");

                if (order < currentOrder)
                    throw new Exception("Элемент с таким же порядком уже был в очереди и был удален из очереди.");

                if (order == currentOrder && subOrder < currentSubOrder)
                    throw new Exception("Элемент с таким же порядком уже был в очереди и был удален из очереди.");

                if (lastSubOrder && subOrderLimits.ContainsKey(order))
                    throw new Exception("Второстеменный порядковый номер.");

                if (lastSubOrder)
                    subOrderLimits[order] = subOrder;

                queueDictionary.Add(queueOrder, item);

                
            }
        }

        /// <summary>
        /// Получить элемент из очереди
        /// </summary>
        /// <param name="item">Полученный из очереди элемент</param>
        /// <returns>Признак успешного получения элемента</returns>
        public bool TryDequeue(out T item)
        {
            lock (innerLock)
            {
                var queueOrder = new Queue_Order(currentOrder, currentSubOrder);

                if (queueDictionary.TryGetValue(queueOrder, out item))
                {
                    queueDictionary.Remove(queueOrder);

                    if (subOrderLimits.ContainsKey(currentOrder) && currentSubOrder == subOrderLimits[currentOrder])
                    {
                        Interlocked.Increment(ref currentOrder);
                        currentSubOrder = 0;
                    }
                    else
                    {
                        Interlocked.Increment(ref currentSubOrder);
                    }

                   

                    return true;
                }
            }

            item = default(T);
            return false;
        }

        /// <summary>
        /// Очистка очереди
        /// </summary>
        public void Clear()
        {
            lock (innerLock)
            {
                currentOrder = 0;
                currentSubOrder = 0;

                subOrderLimits.Clear();
                queueDictionary.Clear();
            }
        }

        /// <summary>
        /// Составной порядковый номер элемента в очереди
        /// </summary>
        private struct Queue_Order
        {
            public Queue_Order(int order, int subOrder = 0)
            {
                Order = order;
                SubOrder = subOrder;
            }
            
            public int Order;
            public int SubOrder;


        }
    }
}