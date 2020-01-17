using System;
using System.Collections.Generic;


namespace ConsoleApplication58
{
    public class Event:EventArgs  // класс для обработки событий
    {

        public enum Finish_Status
        {
            /// <summary>
            /// Операция завершилась успешно
            /// </summary>
            Successed,


            /// <summary>
            /// Операция отменена
            /// </summary>
           Cancel,

            /// <summary>
            /// Операция завершилась с ошибками
            /// </summary>
            Faulted
        }

        public static Event Success(long inputFileSize, long outputFileSize)
        {
            return new Event(Finish_Status.Successed, inputFileSize, outputFileSize, null);
        }
        public static Event Cancell()
        {
            return new Event(Finish_Status.Cancel, 0, 0, null);
        }
      

        public static Event Fault(List<Exception> exceptions)
        {
            return new Event(Finish_Status.Faulted, 0, 0, exceptions);
        }

        private Event(
            Finish_Status status,
            long inFileSize,
            long outFileSize,
            IEnumerable<Exception> exceptions)
        {
            Status = status;
            InFile_Size = inFileSize;
            OutFile_Size = outFileSize;
            Exceptions = exceptions == null ? new List<Exception>() : new List<Exception>(exceptions);
        }

        /// <summary>
        /// Статус операции
        /// </summary>
        public Finish_Status Status { get; private set; }

        /// <summary>
        /// Размер исходного файла
        /// </summary>
        public long InFile_Size { get; private set; }

        /// <summary>
        /// Размер преобразованного файла
        /// </summary>
        public long OutFile_Size { get; private set; }

        /// <summary>
        /// Ошибки при выполнении операции
        /// </summary>
        public List<Exception> Exceptions { get; private set; }
    }





    
}
