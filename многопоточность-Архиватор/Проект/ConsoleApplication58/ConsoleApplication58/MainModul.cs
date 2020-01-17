using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using GZipCompressor;

namespace ConsoleApplication58
{
    public abstract class MainModul : IJust_Do_It
    {
        
      
        protected const int QueueMaxSize = 30; // ограничение очереди по количеству элементов
        protected const long MinBlockSize = 10*1024*1024; // размер блока данных для обработки
        private const int ThreadSleep = 2; // интервал, когда поток остановлен

        protected string inputPath; 
        private string outputPath;
        protected long inputStreamLength;
        protected int totalBuffersCount;
        private long writtenBytesCount;
        private int writtenBuffersCount;
        protected volatile bool cancellationPending;

        private readonly Thread initialThread;// поток для чтения файла и создания задач
        private readonly Thread writeOutputStreamThread; // поток для записи выходного файла
        protected readonly RedyQueue<byte[]> buffer_Queue; // очередь для блоков данных
        protected readonly Semaphor Semafor; // Семафор для очереди
        private readonly List<Exception> Exceptionss; // храню исключения
        protected readonly PCQueue q; // имплементация Producer-Cansumer


    
        //  конструктор с параметрами
        protected MainModul(long blockSize = MinBlockSize, int maxQueueSize = QueueMaxSize)
        {
            BlockSize = blockSize;
            ThreadsCount = Environment.ProcessorCount;// количество ядер процессора
            MaxQueueSize = maxQueueSize;
            initialThread = new Thread(ReadInputStream);
            writeOutputStreamThread = new Thread(WriteOutputStream);
            q = new PCQueue(ThreadsCount);
            
            buffer_Queue = new RedyQueue<byte[]>(); // очередь для хранения
            Semafor = new Semaphor(maxQueueSize);
            Exceptionss = new List<Exception>();

        }



        /// Событие завершения операции
        /// </summary>
        public event EventHandler<Event> Completed;

        /// <summary>
        /// Размер блока данных, который преобразуется потоке
        /// </summary>
        public long BlockSize { get; private set; }

        /// <summary>
        /// Количество потоков для преобразования данных
        /// </summary>
        public int ThreadsCount { get; private set; }

        /// <summary>
        /// Максимальное количество блоков данных, в очереди на запись в файл
        /// </summary>
        public int MaxQueueSize { get; private set; }


        public void Execute(string inputPath, string outputPath)
        {
     
            this.inputPath = inputPath;
            this.outputPath = outputPath;
            this.cancellationPending = false;
            

            initialThread.Start();
            writeOutputStreamThread.Start();
          
        }

       

       


        protected abstract void ReadInputStream();

        private void WriteOutputStream()
        {

            try
            {
                using (var outputStream = File.OpenWrite(outputPath))
                {
                    // Завершаем запись, если завершился основной поток и не выполняются отдельные потоки 
                    // по преобразованию блоков. Данные отсутсвуют в очереди на запись
                    while (initialThread.IsAlive || q.CurrentThreadsCount > 0 || buffer_Queue.Size > 0)
                    {
                        if (cancellationPending)
                            break;

                        if (buffer_Queue.Size > 0)
                        {
                            byte[] buffer;
                            if (buffer_Queue.TryDequeue(out buffer))
                            {
                                Semafor.Release();
                                // Пишем массив байтов, полученный из очереди, в файл
                                outputStream.Write(buffer, 0, buffer.Length);

                                outputStream.Flush();

                                Interlocked.Increment(ref writtenBuffersCount);
                                Interlocked.Add(ref writtenBytesCount, buffer.Length);
                                
                                // Очищаю память
                                GC.Collect();
                            }
                        }

                        Thread.Sleep(ThreadSleep);
                    }
                }




            }
            catch (Exception e)
            {
               
                Exceptionss.Add(e);
                Cancel();

            }

            finally
            {
                // Завершении операции
            Completion();

                
            }



            
            







        }


        private void Completion()
        {
            var completedHandler = Completed;
            if (completedHandler != null)
            {
                Event eventArgs = null;

                if (Exceptionss.Count > 0)
                {
                    eventArgs = Event.Fault(Exceptionss);
                }

                else if (cancellationPending)
                {
                    eventArgs = Event.Cancell();
                }
                
                else
                {
                    eventArgs = Event.Success(inputStreamLength, writtenBytesCount);
                }

                // Освобождаем данные для последующих операций
                Exceptionss.Clear();
                buffer_Queue.Clear();
                Semafor.ReleaseAll();
                completedHandler.Invoke(this, eventArgs);
                
            }
              
            
            
            
            
            
            

                
            
        }

        public void Cancel()
        {
            cancellationPending = true;
        }
       






    }
}
