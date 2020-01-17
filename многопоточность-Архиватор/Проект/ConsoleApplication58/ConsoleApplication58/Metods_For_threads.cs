using System.Collections.Generic;
using System.IO;


namespace ConsoleApplication58
{
    /// <summary>
    /// Несколько методов для потоков
    /// </summary>
    public static class Metods_For_threads
    {
        /// <summary>
        /// Размер блока для чтения потока байтов
        /// </summary>
        private const int ReadBlockSizeSize_Defolt = 1024;

      

        /// <summary>
        /// Метод определяет начинается ли поток с указанного массива байтов.
        /// </summary>
        /// <param name="inputStream">Поток байтов</param>
        /// <param name="buffer">Массив байтов</param>
        /// <returns>Поток байтов начинается указанного массива байтов</returns>
        public static bool Start_With(this Stream inputStream, byte[] buffer)
        {
            byte[] Stream_Buffer = new byte[buffer.Length];
            if (inputStream.Position > 0)
                inputStream.Seek(0, SeekOrigin.Begin);

            if (inputStream.Read(Stream_Buffer, 0, Stream_Buffer.Length) > 0)
            {
                inputStream.Seek(0, SeekOrigin.Begin);
                return CompareArrays(Stream_Buffer, 0, buffer);
            }

            return false;
        }

        /// <summary>
        /// Первое вхождение указанного массива байтов в потоке
        /// </summary>
        /// <param name="inputStream">Поток байтов</param>
        /// <param name="blockHeader">Массив байтов</param>
        /// <param name="readBlockSize">Размер блока для чтения потока байтов</param>
        /// <returns>Первое вхождение указанного массива байтов</returns>
        public static long GetBufferIndex(this Stream inputStream, byte[] blockHeader, int readBlockSize = ReadBlockSizeSize_Defolt)
        {
            while (inputStream.Position < inputStream.Length)
            {
                long startPosition = inputStream.Position;

                byte[] buffer = new byte[readBlockSize];
                if (inputStream.Read(buffer, 0, buffer.Length) == 0)
                    break;

                var arrayIndexes = GetSubArrayIndexes(buffer, blockHeader);
                if (arrayIndexes.Length > 0)
                {
                    inputStream.Position = arrayIndexes.Length == 1 ? startPosition + readBlockSize : startPosition + arrayIndexes[1];
                    return startPosition + arrayIndexes[0];
                }

                if (inputStream.Position < inputStream.Length)
                    inputStream.Position -= blockHeader.Length;
            }

            return -1;
        }

        private static long[] GetSubArrayIndexes(byte[] array, byte[] subArray)
        {
            var indexes = new List<long>();

            for (int i = 0; i < array.Length; i++)
            {
                if (CompareArrays(array, i, subArray))
                {
                    indexes.Add(i);
                }
            }

            return indexes.ToArray();
        }

        private static bool CompareArrays(byte[] array, int startIndex, byte[] arrayToCompare)
        {
            if (startIndex < 0 || startIndex > array.Length - arrayToCompare.Length)
            {
                return false;
            }

            for (int i = 0; i < arrayToCompare.Length; i++)
            {
                if (array[startIndex + i] != arrayToCompare[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
