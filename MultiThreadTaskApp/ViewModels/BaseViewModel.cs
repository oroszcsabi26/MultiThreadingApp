using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MultiThreadTaskApp.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged // biztosítja hogy a PropertyObject-ek változásai értesítik a UI-t
    {
        // értesíti a felhasználói felületet ha egy tulajdonság értéke megváltozott
        public event PropertyChangedEventHandler PropertyChanged;

        // értesíti a UI elemeket ha egy adott tulajdonság értéke változik
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            // kiváltjuk a PropertyChanged eseményt ami frissíti a hozzá kötött elemeket
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // ez a metódus ellenőrzi hogy megváltozott-e egy tulajdonság értéke mielőtt frissítené azt
        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null) //field: PropertyObject, T value az új érték, propertyName: a tulajdonság neve
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
            {
                return false;
            }
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
