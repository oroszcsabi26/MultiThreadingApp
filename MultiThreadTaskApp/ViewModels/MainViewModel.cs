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
        private ObservableCollection<PropertyObject> m_propertyObjects;
        public ObservableCollection<PropertyObject> PropertyObjects
        {
            get => m_propertyObjects;
            set => SetProperty(ref m_propertyObjects, value);
        }

        private ProcessorForTasks m_taskProcessor;
        private CancellationTokenSource m_objectGenerationTokenSource;

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
            set => SetProperty(ref m_executedTaskCount, value);
        }

        private void OnExecutedTaskCountChangedHendler(int p_count)
        {
            ExecutedTaskCount = p_count;
        }

        public MainViewModel()
        {
            PropertyObjects = new ObservableCollection<PropertyObject>();

            StartObjectGenerationCommand = new Command(StartGeneratingObjects);
            StopObjectGenerationCommand = new Command(StopGeneratingObjects);

            StartTaskProcessingCommand = new Command(StartProcessingTasks);
            StopTaskProcessingCommand = new Command(StopProcessingTasks);
        }

        private async void StartGeneratingObjects()
        {
            m_objectGenerationTokenSource = new CancellationTokenSource();
            _ = Task.Run(() => GenerateObjectsAsync(m_objectGenerationTokenSource.Token));

            // Várunk, amíg a PropertyObjects legalább egy elemet tartalmaz
            while (PropertyObjects.Count == 0)
            {
                await Task.Delay(100); // Rövid várakozás az UI megakadása nélkül
            }

            // Csak ezután példányosítjuk a TaskProcessor-t
            if (m_taskProcessor == null)
            {
                m_taskProcessor = new ProcessorForTasks(PropertyObjects, m_maxQueueSize, m_workerCount);
            }
        }

        private void StopGeneratingObjects()
        {
            m_objectGenerationTokenSource?.Cancel();
        }

        private async Task GenerateObjectsAsync(CancellationToken token)
        {
            Random random = new Random();

            while (!token.IsCancellationRequested)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    while (PropertyObjects.Count >= m_maxObjectListSize)
                    {
                        int removeIndex = random.Next(0, PropertyObjects.Count);
                        PropertyObjects.RemoveAt(removeIndex);
                    }

                    PropertyObject newObject = new PropertyObject();
                    PropertyObjects.Add(newObject);
                });

                await Task.Delay(2000);
            }
        }

        private void StartProcessingTasks()
        {
            m_taskProcessor.ResetCancellationToken();
            m_taskProcessor.OnExecutedTaskCountChanged += OnExecutedTaskCountChangedHendler;
            m_taskProcessor.StartProcessing();
        }

        private void StopProcessingTasks()
        {
            m_taskProcessor.OnExecutedTaskCountChanged -= OnExecutedTaskCountChangedHendler;
            m_taskProcessor.StopProcessing();
        }
    }
}
