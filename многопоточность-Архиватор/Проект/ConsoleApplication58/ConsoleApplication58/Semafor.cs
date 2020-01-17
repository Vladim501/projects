using System.Threading;

namespace ConsoleApplication58
{
   
   
        /// <summary>
        ///Реализация семафора.
        ///Не использую System.Threading.Semaphore для ускорения производительности.
        /// </summary>
        public class Semaphor
        {
            private const int SleepIntervaI = 5;

            private readonly object Wait_Lock = new object();

            /// <summary>
            /// Максимальное количество вхождений
            /// </summary>
            private readonly int maximumCount;

            /// <summary>
            /// Счетчик текущих вхождений вхождений
            /// </summary>
            private int counter;

            public Semaphor(int maximumCount)
            {
                this.maximumCount = maximumCount;
            }

            public int Wait()
            {
                lock (Wait_Lock)
                {
                    while (counter >= maximumCount)
                    {
                        Thread.Sleep(SleepIntervaI);
                    }

                    return Interlocked.Increment(ref counter);
                }
            }

            /// <summary>
            /// Уменьшить количество вхождений
            /// </summary>
            /// <returns>Значение счетчика текущих вхождений</returns>
            public int Release()
            {
                return Interlocked.Decrement(ref counter);
            }

            /// <summary>
            /// Сборсить количество текущих значений
            /// </summary>
            public void ReleaseAll()
            {
                counter = 0;
            }
        }




    }

