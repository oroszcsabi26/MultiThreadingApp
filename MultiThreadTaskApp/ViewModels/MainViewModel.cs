using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MultiThreadTaskApp.Models;

namespace MultiThreadTaskApp.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private ObservableCollection<PropertyObject> m_propertyObjects;
        public ObservableCollection<PropertyObject> PropertyObjects
        {
            get => m_propertyObjects;
            set => SetProperty(ref m_propertyObjects, value); // amikor a PropertyObjects értéke megváltozik, akkor értesíti a UI-t az OnpropertyChanged metódus segítségével
        }

        private CancellationTokenSource m_cancellationTokenSource;

        public ICommand StartCommand { get; }
        public ICommand StopCommand { get; }

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

        public MainViewModel()
        {
            PropertyObjects = new ObservableCollection<PropertyObject>();
            StartCommand = new Command(StartGeneratingObjects);
            StopCommand = new Command(StopGeneratingObjects);
        }

        private void StopGeneratingObjects()
        {
            m_cancellationTokenSource?.Cancel();
        }

        private void StartGeneratingObjects()
        {
            m_cancellationTokenSource = new CancellationTokenSource();
            Task.Run(() => GenerateObjectsAsync(m_cancellationTokenSource.Token)); // háttérszálon végezzük az objektumok létrehozását így a UI elvileg nem akadhat meg
        }

        private async Task GenerateObjectsAsync(CancellationToken token)
        {
            Random random = new Random();

            while (!token.IsCancellationRequested)
            {
                // Dispatchher, hogy a UI elemekhez csak a fő szál férhessen hozzá
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
                // mivel async metódusban így megvárja az await-et
                await Task.Delay(2000); // új objektum generálása 2 mp-ként
            }
        }
    }
}
