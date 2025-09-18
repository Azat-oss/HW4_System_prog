using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


//C# Реализовать механизмы синхронизации (Monitor, lock, Mutex, Semaphore, WaitHandle) 
//для нескольких потоков доступа к данным счетов клиентов (увеличение или уменьшение),
//хранящихся в файле JSON (имя клиента - сумма счета).
//В параметрах метода - имя клиента и сумма изменения счета.
//Реализовать с помощью Task (Factory<Task>)


namespace HW_4 
{
     class Program
    {
        static async Task Main(string[] args)
        {
            ///////
            var manager = new AccountManager();

            Console.WriteLine("Запуск 5 задач...");

            var tasks = new List<Task<decimal>>();

            for (int i = 0; i < 5; i++)
            {

                tasks.Add(manager.UpdateBalanceWithLockAsync("Щербаков Д.Н.", 1021));
                
               
            }

            var results = await Task.WhenAll(tasks);

            Console.WriteLine("\nРезультаты по Lock:");
            foreach (var balance in results)
            {
                Console.WriteLine($"Новый баланс Щербаков Д.Н. : {balance} ");
            }

            ///////
            var manager2 = new AccountManager();

            Console.WriteLine("\n Запуск 6 задач...");

            var tasks2 = new List<Task<decimal>>();

            for (int i = 0; i < 6; i++)
            {

                tasks2.Add(manager2.UpdateBalanceWithMutexAsync("Васильев А.Ю.", 1000));

            }

            var results2 = await Task.WhenAll(tasks2);

            Console.WriteLine("\nРезультаты по Mutex:");
            foreach (var balance in results2)
            {
                Console.WriteLine($"Новый баланс Васильев А.Ю. : {balance} ");
            }


            ///////
            var manager3 = new AccountManager();

            Console.WriteLine("\n Запуск 7 задач...");

            var tasks3 = new List<Task<decimal>>();

            for (int i = 0; i < 7; i++)
            {

                tasks3.Add(manager3.UpdateBalanceWithMonitorAsync("Савельев И.И.", 1000));

            }

            var results3 = await Task.WhenAll(tasks3);

            Console.WriteLine("\nРезультаты по Monitor:");
            foreach (var balance in results3)
            {
                Console.WriteLine($"Новый баланс Савельев И.И. : {balance} ");
            }

            ///////
            var manager4 = new AccountManager();

            Console.WriteLine("\n Запуск 8 задач...");

            var tasks4 = new List<Task<decimal>>();

            for (int i = 0; i < 8; i++)
            {

                tasks4.Add(manager4.UpdateBalanceWithSemaphoreAsync("Хабирова А.Ж.", 1000));

            }

            var results4 = await Task.WhenAll(tasks4);

            Console.WriteLine("\nРезультаты по Semaphore:");
            foreach (var balance in results4)
            {
                Console.WriteLine($"Новый баланс Хабирова А.Ж.: {balance} ");
            }

        }
    }
}
