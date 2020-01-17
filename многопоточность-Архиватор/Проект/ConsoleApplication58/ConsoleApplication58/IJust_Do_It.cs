using System;


namespace ConsoleApplication58
{
   public  interface IJust_Do_It
   {
    
       //Запуск операции
      
       void Execute(string inputPath, string outputPath);

    
       event EventHandler<Event> Completed;

       void Cancel();
      


   }
}
