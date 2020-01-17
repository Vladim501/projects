using System;
using System.IO;
using System.IO.Compression;


namespace ConsoleApplication58
{
   public class Compressor:MainModul 
    {


 //  конструктор
        public Compressor(long blockSize = MinBlockSize, int maxQueueSize = QueueMaxSize)
            : base(blockSize,  maxQueueSize)
        {
        }

       protected override void ReadInputStream()
        {
            
            
            var inputFileInfo = new FileInfo(inputPath);
            inputStreamLength = inputFileInfo.Length;
            totalBuffersCount = (int)Math.Ceiling((double)inputFileInfo.Length / BlockSize);
            
            
            for (int i = 0; i < totalBuffersCount; i++)
            {
                if (cancellationPending)
                    break;
                
                
                int blockIndex = i; // номер блока
                long currentPosition = i * BlockSize; // позиция в потоке
                long blockLength = Math.Min((long)BlockSize, inputStreamLength - currentPosition);// длинна блока

                // отправляем задачу в очередь задач
            q.Enqueue(() => { COmpress(currentPosition, (int)blockLength, blockIndex); });
            

            }



        }

       


        private void COmpress(long streamStartPosition, int blockLength, int blockOrder)
        {

          //читаем часть из файла
            byte[] Buffer = new byte[blockLength]; // размер буфера
            using (var FS = File.OpenRead(inputPath))
            {
                FS.Seek(streamStartPosition, SeekOrigin.Begin);// устанавливаем позицию в потоке со смещением
                FS.Read(Buffer, 0, blockLength);
            }

            // сжимаем эту часть

            byte[] compresBuffer;

            using (var ms=new MemoryStream())
            {
                using (var GZ=new GZipStream(ms,CompressionMode.Compress,true))
                {
                    GZ.Write(Buffer,0,Buffer.Length);
                }

                compresBuffer = ms.ToArray();
            } 

            // отправляем на запись в очередь
           Semafor.Wait();
           buffer_Queue.Enqueue(blockOrder,compresBuffer);
           Console.Write("*"); // визуально показывает что операция идет
            
            

        }


        
    }
}
