using System;
using System.IO;
using System.IO.Compression;


namespace ConsoleApplication58
{
    public class Decompressor:MainModul
    {
         /// <summary>
        /// Заголовок файла сжатого GZStream соответсвует формату GZip. 
        /// Беру описание заголовка GZip (https://www.ietf.org/rfc/rfc1952.txt).
        /// </summary>
        private readonly byte[] gzipHeader = new byte[] { 31, 139, 8, 0, 0, 0, 0, 0, 4, 0 };

        private const int ReadBufferSize = 1024 * 1024;

     


        //  конструктор
        public Decompressor(long blockSize = MinBlockSize, int maxQueueSize = QueueMaxSize)
            : base(blockSize,  maxQueueSize)
        {
        }

        protected override void ReadInputStream()
        {
           
                using (var inputStream = File.OpenRead(inputPath))
                {
                    inputStreamLength = inputStream.Length;

                    // Файл не начинается с заголовка Gzip, значит архив был создан в другой программе.
                    // Выполняем распаковку архива в одном потоке.
                    if (!inputStream.Start_With(gzipHeader))
                    {
                       
                        DecompressBlock(0, 0);
                       
                    }
                    else
                    {
                        int blockOrder = 0;
                        while (inputStream.Position < inputStream.Length)
                        {
                            if (cancellationPending)
                                break;

                            var nextBlockIndex = inputStream.GetBufferIndex(gzipHeader, ReadBufferSize);
                            if (nextBlockIndex == -1)
                            {
                               
                                break;
                            }

                           

                            var localBlockOrder = blockOrder;
                            q.Enqueue(() => DecompressBlock(nextBlockIndex, localBlockOrder));

                            blockOrder += 1;
                            Console.Write("*"); // визуально показывает что операция идет
                            
                            GC.Collect();
                        }
                    }
                }
            
        }

        /// <summary>
        /// Распаковываю  отдельный блок данных из исходного файла
        /// </summary>
        /// <param name="Position_in_stream">Смещение блока данных от начала файла</param>
        /// <param name="blockOrder">Порядок блока</param>
        private void DecompressBlock(long Position_in_stream, int blockOrder)
        {
           
                using (var inputStream = File.OpenRead(inputPath))
                {
                    inputStream.Seek(Position_in_stream, SeekOrigin.Begin);

                    using (var compressStream = new GZipStream(inputStream, CompressionMode.Decompress, true))
                    {
                        int bufferNumber = 0;

                        byte[] buffer = new byte[BlockSize];
                        int bytesRead = compressStream.Read(buffer, 0, buffer.Length);
                        if (bytesRead < BlockSize)
                            Array.Resize(ref buffer, bytesRead);

                        byte[] nextBuffer = new byte[BlockSize];
                        while (bytesRead > 0)
                        {
                            

                            bytesRead = compressStream.Read(nextBuffer, 0, nextBuffer.Length);

                            if (bytesRead < BlockSize)
                                Array.Resize(ref nextBuffer, bytesRead);

                            Semafor.Wait();
                            buffer_Queue.Enqueue(blockOrder, bufferNumber, buffer, nextBuffer.Length == 0);

                            buffer = nextBuffer;
                            nextBuffer = new byte[BlockSize];

                            bufferNumber++;
                           

                           
                        }
                    }
                }
            
           
        }

  

     
     
    }
}
    

