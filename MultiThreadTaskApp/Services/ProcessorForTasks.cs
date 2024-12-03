using MultiThreadTaskApp.Models;
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
        private readonly ConcurrentQueue<TasksForObjects> m_tasksQueue; // a végrehajtandó feladatok listája
        private CancellationTokenSource m_cancellationTokenSource;
        private int m_executedTaskCount; // a végrehajtott feladatok száma
        private int m_maxQueueSize; // a maximális feladatok száma
        private int m_workerClassCount; // a háttérszálon dolgozó osztályok száma

        public event Action<int> OnExecutedTaskCountChanged;

        public ProcessorForTasks(ObservableCollection<PropertyObject> p_propertyObjects, int p_maxQueueSize = 30, int p_workerClassCount = 3)
        {
            m_propertyObjects = p_propertyObjects ?? throw new ArgumentNullException(nameof(p_propertyObjects));
            m_tasksQueue = new ConcurrentQueue<TasksForObjects>();
            m_cancellationTokenSource = new CancellationTokenSource();
            m_executedTaskCount = 0;
            m_maxQueueSize = p_maxQueueSize;
            m_workerClassCount = p_workerClassCount;

            if (m_propertyObjects.Count > 0)
            {
                GenerateTasks();
            }
        }

        public void StartProcessing()
        {
            for (int i = 0; i < m_workerClassCount; i++)
            {
                Task.Run(() => ProcessTasksAsync(m_cancellationTokenSource.Token));
            }
        }

        public void StopProcessing()
        {
            m_cancellationTokenSource.Cancel();
        }

        // Párhuzamos feldolgozók számának beállítása
        public void SetWorkerCount(int p_count)
        {
            if(p_count < 1)
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

        //Todo : Errol beszeljunk szoban , hogyan kepzelted ezt el  
        private async void ProcessTasksAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                if (m_tasksQueue.TryDequeue(out TasksForObjects task))
                {
                    task.Execute();
                    Interlocked.Increment(ref m_executedTaskCount); // szálbiztos növelése a száméálónak
                    
                    OnExecutedTaskCountChanged?.Invoke(m_executedTaskCount);

                    if (m_tasksQueue.Count < m_maxQueueSize / 2)
                    {
                        GenerateTasks();
                    }

                    // 2 másodperces késleltetés minden task végrehajtása után
                    await Task.Delay(2000);
                }
                else
                {
                    await Task.Delay(100);
                }
            }
        }

        private void GenerateTasks()
        {
            Random random = new Random();
            while (m_tasksQueue.Count < m_maxQueueSize)
            {
                PropertyObject randomTargetObject = m_propertyObjects[random.Next(0, m_propertyObjects.Count)];
                TasksForObjects.TaskType randomTaskType = (TasksForObjects.TaskType)random.Next(0, 5);
                m_tasksQueue.Enqueue(new TasksForObjects(randomTaskType, randomTargetObject));            
            }
        }

        public void ResetCancellationToken()
        {
            m_cancellationTokenSource?.Dispose();
            m_cancellationTokenSource = new CancellationTokenSource();
        }
    }
}
