using System;
using System.Threading;
using ConsoleApplication58;

public class PCQueue
{
    /// <summary>
    /// Потоки в работе
    /// </summary>
    private int currentThreadsCount;

    private readonly Semaphor threadSemaphore;

    public PCQueue(int maxThreads)
    {
        MaxThreads = maxThreads;

        threadSemaphore = new Semaphor(maxThreads);
    }

    /// <summary>
    /// Максимальное число потоков
    /// </summary>
    public int MaxThreads { get; private set; }

    public int CurrentThreadsCount
    {
        get { return currentThreadsCount; }
    }

    /// <summary>
    /// Когда достигнуто максимальное количество потоков
    /// планировщик блокирует вызывающий поток и ожидает завершения одного из запущенных потоков.
    /// </summary>
    /// <param name="threadOperation">Выполняемая операция</param>
    public void Enqueue(Action threadOperation)
    {
        currentThreadsCount = threadSemaphore.Wait();

        // Запускаем отдельном потоке
        var thread = new Thread(ExceuteThread);
        thread.Start(threadOperation);
      

       
    }

    private void ExceuteThread(object state)
    {
        var threadAction = state as Action;
        if (threadAction != null)
            threadAction();

        currentThreadsCount = threadSemaphore.Release();

      
    }
}

