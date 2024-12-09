using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using MultiThreadTaskApp.Models;
using MultiThreadTaskApp.Services;

namespace MultiThreadTaskApp.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private ProcessorForTasks m_taskProcessor;
        private CancellationTokenSource m_objectGenerationTokenSource;
        private ObservableCollection<PropertyObject> m_propertyObjects;
        private readonly object propertyObjectsLock = new object(); // Lock objektum a PropertyObjects kollekcióhoz.

        public ObservableCollection<PropertyObject> PropertyObjects
        {
            get => m_propertyObjects;
            set => SetProperty(ref m_propertyObjects, value);
        }

        public ICommand StartObjectGenerationCommand { get; }
        public ICommand StopObjectGenerationCommand { get; }
        public ICommand StartTaskProcessingCommand { get; }
        public ICommand StopTaskProcessingCommand { get; }

        private int m_maxObjectListSize = 20;
        public string MaxObjectListSize
        {
            get => m_maxObjectListSize.ToString();
            set
            {
                if (int.TryParse(value, out int newSize) && newSize > 0)
                {
                    m_maxObjectListSize = newSize;
                    OnPropertyChanged();
                }
            }
        }

        private int m_maxQueueSize = 30;

        public string MaxQueueSize
        {
            get => m_maxQueueSize.ToString();
            set
            {
                if (int.TryParse(value, out int newValue) && newValue > 0)
                {
                    m_maxQueueSize = newValue;
                    m_taskProcessor?.SetMaxQueueSize(newValue); 
                    OnPropertyChanged();
                }
            }
        }

        private int m_workerCount = 3;
        public string WorkerCount
        {
            get => m_workerCount.ToString();
            set
            {
                if (int.TryParse(value, out int newValue) && newValue > 0)
                {
                    m_workerCount = newValue;
                    m_taskProcessor?.SetWorkerCount(newValue); 
                    OnPropertyChanged();
                }
            }
        }

        private int m_executedTaskCount;
        public int ExecutedTaskCount
        {
            get => m_executedTaskCount;
            private set => SetProperty(ref m_executedTaskCount, value); 
        }

        public MainViewModel()
        {
            PropertyObjects = new ObservableCollection<PropertyObject>();

            StartObjectGenerationCommand = new Command(StartGeneratingObjects);
            StopObjectGenerationCommand = new Command(StopGeneratingObjects);

            StartTaskProcessingCommand = new Command(StartProcessingTasks);
            StopTaskProcessingCommand = new Command(StopProcessingTasks);
        }

        private void StartGeneratingObjects()
        {
            m_objectGenerationTokenSource = new CancellationTokenSource();
            Thread objectGenerationThread = new Thread(() => GenerateObjects(m_objectGenerationTokenSource.Token));
            objectGenerationThread.IsBackground = true; 
            objectGenerationThread.Start();

            //Példányosítjuk a TaskProcessor-t
            if (m_taskProcessor == null)
            {
                m_taskProcessor = new ProcessorForTasks(this, PropertyObjects, m_maxQueueSize, m_workerCount);
            }
        }

        private void StopGeneratingObjects()
        {
            m_objectGenerationTokenSource?.Cancel();
        }

        private void GenerateObjects(CancellationToken token)
        {
            Random random = new Random();

            while (!token.IsCancellationRequested)
            {
                List<PropertyObject> newObjects = new List<PropertyObject>();

                lock (propertyObjectsLock)
                {
                    while (PropertyObjects.Count + newObjects.Count >= m_maxObjectListSize)
                    {
                        int removeIndex = random.Next(0, PropertyObjects.Count);
                        PropertyObject targetObject = PropertyObjects[removeIndex];

                        // Várunk, amíg semmilyen művelet nem fut az objektumon
                        targetObject.WaitForNoOperations();

                        if (PropertyObjects.Contains(targetObject))
                        {
                            PropertyObjects.Remove(targetObject);
                        }
                    }
                }

                // Új objektum létrehozása és hozzáadása
                PropertyObject newObject = new PropertyObject();
                newObjects.Add(newObject);

                lock (propertyObjectsLock)
                {
                    foreach (PropertyObject obj in newObjects)
                    {
                        PropertyObjects.Add(obj);
                    }
                }

                Thread.Sleep(2000); // Új objektumok között várakozás
            }
        }

        /*
        private void GenerateObjects(CancellationToken token)
        {
            Random random = new Random();

            while (!token.IsCancellationRequested)
            {
                List<PropertyObject> newObjects = new List<PropertyObject>(); 

                // Objektum törlése, ha szükséges
                while (PropertyObjects.Count + newObjects.Count >= m_maxObjectListSize)
                {
                    int removeIndex = random.Next(0, PropertyObjects.Count);
                    PropertyObject targetObject = PropertyObjects[removeIndex];

                    // Várunk, amíg semmilyen művelet nem fut az objektumon
                    targetObject.WaitForNoOperations();
                    lock (targetObject)
                    {
                        if (PropertyObjects.Contains(targetObject))
                        {
                            PropertyObjects.Remove(targetObject);
                        }
                    }
                }
                // Új objektum létrehozása és hozzáadása
                PropertyObject newObject = new PropertyObject();
                newObjects.Add(newObject);

                foreach (PropertyObject obj in newObjects)
                {
                    PropertyObjects.Add(obj);
                }

                Thread.Sleep(2000); 
            }
        }
        */

        private void StartProcessingTasks()
        {
            m_taskProcessor.ResetCancellationToken();
            m_taskProcessor.StartProcessing();
        }

        private void StopProcessingTasks()
        {
            m_taskProcessor.StopProcessing();
        }

        internal void IncrementExecutedTaskCount()
        {
            Interlocked.Increment(ref m_executedTaskCount);
            OnPropertyChanged(nameof(ExecutedTaskCount));
        }
    }
}