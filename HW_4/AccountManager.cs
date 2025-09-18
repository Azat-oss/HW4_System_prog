using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;


namespace HW_4
{
    public class AccountManager
    {
        
        private static readonly string FilePath = @"C:\account_users\accounts.json"; 

      
        private static readonly object _lockObject = new object();
        private static readonly Mutex _mutex = new Mutex(false, "Global\\AccountManagerMutex");
        private static readonly Semaphore _semaphore = new Semaphore(1, 1);
        private static readonly ManualResetEvent _waitHandle = new ManualResetEvent(true);

        
        static AccountManager()
        {
            var directory = Path.GetDirectoryName(FilePath);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
        }

      
        private Dictionary<string, decimal> LoadAccounts()
        {
            if (!File.Exists(FilePath))
            {
                var initial = new Dictionary<string, decimal>
            {
                { "Осипов А.А.", 1000m },
                { "Щербаков Д.Н.", 500m },
                { "Савельев И.И.", 500m },
                { "Васильев А.Ю.", 500m },
                { "Хабирова А.Ж.", 500m }
            };
                SaveAccounts(initial); 
                return initial;
            }

            var json = File.ReadAllText(FilePath);
            return JsonSerializer.Deserialize<Dictionary<string, decimal>>(json) ?? new Dictionary<string, decimal>();
        }

       
        private void SaveAccounts(Dictionary<string, decimal> accounts)
        {
            var json = JsonSerializer.Serialize(accounts, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(FilePath, json);
        }

        //  lock
        public async Task<decimal> UpdateBalanceWithLockAsync(string clientName, decimal amount)
        {
            return await Task.Factory.StartNew(() =>
            {
                lock (_lockObject)
                {
                    var accounts = LoadAccounts();
                    if (!accounts.ContainsKey(clientName))
                        throw new KeyNotFoundException($"Клиент {clientName} не найден.");

                    accounts[clientName] += amount;
                    SaveAccounts(accounts);
                    return accounts[clientName];
                }
            });
        }

        //  Monitor
        public async Task<decimal> UpdateBalanceWithMonitorAsync(string clientName, decimal amount)
        {
            return await Task.Factory.StartNew(() =>
            {
                Monitor.Enter(_lockObject);
                try
                {
                    var accounts = LoadAccounts();
                    if (!accounts.ContainsKey(clientName))
                        throw new KeyNotFoundException($"Клиент {clientName} не найден.");

                    accounts[clientName] += amount;
                    SaveAccounts(accounts);
                    return accounts[clientName];
                }
                finally
                {
                    Monitor.Exit(_lockObject);
                }
            });
        }

        // Mutex
        public async Task<decimal> UpdateBalanceWithMutexAsync(string clientName, decimal amount)
        {
            return await Task.Factory.StartNew(() =>
            {
                _mutex.WaitOne();
                try
                {
                    var accounts = LoadAccounts();
                    if (!accounts.ContainsKey(clientName))
                        throw new KeyNotFoundException($"Клиент {clientName} не найден.");

                    accounts[clientName] += amount;
                    SaveAccounts(accounts);
                    return accounts[clientName];
                }
                finally
                {
                    _mutex.ReleaseMutex();
                }
            });
        }

        //  Semaphore
        public async Task<decimal> UpdateBalanceWithSemaphoreAsync(string clientName, decimal amount)
        {
            return await Task.Factory.StartNew(() =>
            {
                _semaphore.WaitOne();
                try
                {
                    var accounts = LoadAccounts();
                    if (!accounts.ContainsKey(clientName))
                        throw new KeyNotFoundException($"Клиент {clientName} не найден.");

                    accounts[clientName] += amount;
                    SaveAccounts(accounts);
                    return accounts[clientName];
                }
                finally
                {
                    _semaphore.Release();
                }
            });
        }

        // WaitHandle
        public async Task<decimal> UpdateBalanceWithWaitHandleAsync(string clientName, decimal amount)
        {
            return await Task.Factory.StartNew(() =>
            {
                _waitHandle.WaitOne();
                _waitHandle.Reset();
                try
                {
                    var accounts = LoadAccounts();
                    if (!accounts.ContainsKey(clientName))
                        throw new KeyNotFoundException($"Клиент {clientName} не найден.");

                    accounts[clientName] += amount;
                    SaveAccounts(accounts);
                    return accounts[clientName];
                }
                finally
                {
                    _waitHandle.Set(); 
                }
            });
        }

    }
}
