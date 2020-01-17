using System;
using System.Diagnostics;
using System.IO;
using System.Threading;


namespace ConsoleApplication58
{


    // Структура программы следующая.
    // Главный абстрактный клас MainModul.cs, в нем методы компрессии и декомпрессии , и также метод для записи в файл.
    // Compressor.cs Decompressor.cs классы для компрессии и декомпрессии.
    // RedyQueue.cs  файл очереди для хранения блоков данных.
    // PCQueue.cs  имплементация Постовщика-Потребителя.
    // Semafor.cs реализация своего Семафора.
    // Metods_For_threads.cs методы для работы с потоками.
    // Event.cs класс описывает события успешного и неуспешного окончания операции, а также отмену операции.
    // IJust_Do_It.cs интерфейс для запуска операции.



    class Program
    {
        
        private static IJust_Do_It Go;
        private static readonly AutoResetEvent Reset = new AutoResetEvent(false);
       

       private static void Main(string[] args)
        {
           
            try
            {
                Stopwatch sw = new Stopwatch();


                Console.WriteLine("GZip Тестовое задание.");
                Console.WriteLine("Для отмены операции нажмите Ctrl+C");
                Console.CancelKeyPress += OnConsoleCancelKeyPressed; // событие отмены операции

//                args = new string[3];
//
//                args[0] = "compress";
//                args[1] = @"d:\4.stl";
//                args[2] = @"d:\Compress142.gz";
                

                if (args.Length != 3)
                {
                    Help();
                }
                else
                {
                    if (args.Length == 3)
                    {

                        switch (args[0])
                        {
                            case "compress":
                                Go = new Compressor();
                                Console.WriteLine("Архивирую файл");
                                break;
                            case "decompress":

                                Go = new Decompressor();
                                Console.WriteLine("Разархивирую файл");
                                break;
                            default:
                                Help();
                                break;
                        }

                        // Запускаем операцию
                        var currentDirectory = Directory.GetCurrentDirectory();
                        var inputFileName = Path.Combine(currentDirectory, args[1]);
                        var ouitputFileName = Path.Combine(currentDirectory, args[2]);

                        Go.Completed += OnCompleted; // Событие на завершение
                        //Запускаем операцию
                        sw.Start();
                        Go.Execute(inputFileName, ouitputFileName);
                        Console.WriteLine("Процесс пошел )");
                        // Блокируем основной поток приложения до завершения операции.
                        Reset.WaitOne();

                    }



                }


                Console.WriteLine("Время выполнения операции {0}", sw.Elapsed);
                Console.ReadKey();
            }
              catch (Exception e)
            {
                Console.WriteLine(e.Message);
                
            }

        }


        private static void OnConsoleCancelKeyPressed(object sender, ConsoleCancelEventArgs e)
        {
            if (Go != null)
            {
                Go.Cancel();
                Reset.WaitOne();
            }
        }




       private static void OnCompleted(object sender, Event args)
       {
           
            switch (args.Status)
            {
               
                case Event.Finish_Status.Successed:
                   Console.WriteLine(" ");
                    Console.WriteLine("Операция завершена.");
                    Console.WriteLine(string.Format("Размер исходного файла: {0} bytes.", args.InFile_Size));
                    Console.WriteLine(string.Format("Размер преобразованного файла: {0} bytes.", args.OutFile_Size));
                break;

                case Event.Finish_Status.Cancel:
                    Console.WriteLine("Операция отменена.");
                break;
                    
                case Event.Finish_Status.Faulted:
                    Console.WriteLine("Операция закончена с ошибкой.");
                    foreach (var exception in args.Exceptions)
                    {
                        Console.WriteLine(exception.Message);
                    }
                    
                break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Reset.Set();
        }




        private static void Help()
        {
            Console.WriteLine("Компрессия файла");
            Console.WriteLine("GZipTest.exe compress [имя файла] [имя архивного файла]");
            Console.WriteLine("Декомпрессия файла:");
            Console.WriteLine("GZipTest.exe decompress [имя архивного файла] [имя файла]");
        }
    }
}
