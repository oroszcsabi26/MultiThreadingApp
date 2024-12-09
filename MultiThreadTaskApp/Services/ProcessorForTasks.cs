using MultiThreadTaskApp.Models;
using MultiThreadTaskApp.ViewModels;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiThreadTaskApp.Services
{
    public class ProcessorForTasks
    {
        private readonly ObservableCollection<PropertyObject> m_propertyObjects; // az elérhető objektumok listája
        private CancellationTokenSource m_taskProcessCancellationTokenSource;
        private CancellationTokenSource m_taskGenerationCancellationTokenSource;
        private int m_maxQueueSize; // a maximális feladatok száma
        private int m_workerClassCount; // a háttérszálon dolgozó osztályok száma
        private readonly MainViewModel m_mainViewModel;
        private List<Task> m_taskList = new List<Task>();
        private readonly SemaphoreSlim m_taskListSemaphore = new SemaphoreSlim(1, 1);

        public ProcessorForTasks(MainViewModel p_mainViewModel, ObservableCollection<PropertyObject> p_propertyObjects, int p_maxQueueSize = 30, int p_workerClassCount = 3)
        {
            m_mainViewModel = p_mainViewModel ?? throw new ArgumentNullException(nameof(p_mainViewModel));
            m_propertyObjects = p_propertyObjects ?? throw new ArgumentNullException(nameof(p_propertyObjects));
            m_taskProcessCancellationTokenSource = new CancellationTokenSource();
            m_maxQueueSize = p_maxQueueSize;
            m_workerClassCount = p_workerClassCount;
        }

        public void StartProcessing()
        {
            m_taskProcessCancellationTokenSource = new CancellationTokenSource();
            m_taskGenerationCancellationTokenSource = new CancellationTokenSource();

            // Indítsuk el a GenerateTasks metódust egy külöl Thread-ben
            Thread generateTasksThread = new Thread(() => GenerateTasks(m_taskGenerationCancellationTokenSource.Token))
            {
                IsBackground = true
            };
            generateTasksThread.Start();

            // Feladatfeldolgozók indítása
            Task.Run(() => ProcessTasksAsync(m_taskProcessCancellationTokenSource.Token));
        }

        public async void StopProcessing()
        {
            m_taskProcessCancellationTokenSource.Cancel();
            m_taskGenerationCancellationTokenSource.Cancel();

            await m_taskListSemaphore.WaitAsync();
            try
            {
                foreach (Task task in m_taskList)
                {
                    if (!task.IsCompleted && !task.IsCanceled)
                    {
                        try
                        {
                            await Task.WhenAny(task, Task.Delay(100));
                        }
                        catch (AggregateException) { }
                    }
                }
                m_taskList.Clear();
            }
            finally
            {
                m_taskListSemaphore.Release();
            }
        }

        // Párhuzamos feldolgozók számának beállítása
        public void SetWorkerCount(int p_count)
        {
            if (p_count < 1)
            {
                throw new ArgumentException("A párhuzamos feldolgozók száma nem lehet kisebb, mint 1.");
            }
            m_workerClassCount = p_count;
        }

        // Feladatsor maximális méretének beállítása
        public void SetMaxQueueSize(int p_maxSize)
        {
            if (p_maxSize < 1)
            {
                throw new ArgumentException("A feladatsor maximális mérete nem lehet kisebb, mint 1.");
            }
            m_maxQueueSize = p_maxSize;
        }

        private async void ProcessTasksAsync(CancellationToken token)
        {
            SemaphoreSlim semaphore = new SemaphoreSlim(m_workerClassCount);

            try
            {
                while (!token.IsCancellationRequested)
                {
                    List<Task> runningTasks = new List<Task>();

                    await m_taskListSemaphore.WaitAsync(token); // Szálbiztos hozzáférés a taskList-hez

                    try
                    {
                        foreach (Task task in m_taskList.ToList())
                        {
                            if (token.IsCancellationRequested) break;

                            await semaphore.WaitAsync(token);

                            task.Start();

                            Task runningTask = task.ContinueWith(t =>
                            {
                                semaphore.Release();
                                m_mainViewModel.IncrementExecutedTaskCount();
                            }, token);

                            runningTasks.Add(runningTask);
                            m_taskList.Remove(task);
                            await Task.Delay(1000);
                        }
                    }
                    finally
                    {
                        m_taskListSemaphore.Release();
                    }

                    await Task.WhenAll(runningTasks);
                }
            }
            catch (OperationCanceledException ex) 
            { 
                Console.WriteLine(ex.Message); 
            }
            finally
            {
                semaphore.Dispose();
            }
        }

        private async void GenerateTasks(CancellationToken token)
        {
            Random random = new Random();
            try
            {
                while (!token.IsCancellationRequested)
                {
                    await m_taskListSemaphore.WaitAsync(token); // Szálbiztos hozzáférés a taskList-hez

                    try
                    {
                        if (m_taskList.Count < m_maxQueueSize)
                        {
                            PropertyObject randomTargetObject = m_propertyObjects[random.Next(0, m_propertyObjects.Count)];
                            TasksForObjects.TaskType randomTaskType = (TasksForObjects.TaskType)random.Next(0, 5);

                            Task newTask = new TasksForObjects(randomTaskType, randomTargetObject).CreateTasks(token);
                            m_taskList.Add(newTask);
                        }
                    }
                    finally
                    {
                        m_taskListSemaphore.Release(); // Mindig engedjük fel a lock-ot
                    }

                    await Task.Delay(100); 
                }
            }
            catch (OperationCanceledException ex) 
            { 
                Console.WriteLine(ex.Message); 
            }
        }

        public void ResetCancellationToken()
        {
            m_taskProcessCancellationTokenSource?.Dispose();
            m_taskProcessCancellationTokenSource = new CancellationTokenSource();
        }
    }
}
