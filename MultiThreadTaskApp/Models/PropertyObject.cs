using MultiThreadTaskApp.ViewModels;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MultiThreadTaskApp.Models
{
    public class PropertyObject : BaseViewModel, INotifyPropertyChanged
    {
        private static readonly Random m_random = new Random();
        private int m_activeOperations = 0; // aktív műveletek számlálója
        private readonly object m_activeOperationsLock = new object(); // lock object a számlálóhoz

        // Lock objektum minden property-hez
        private readonly Dictionary<string, object> m_propertyLocks = new Dictionary<string, object>
        {
            { nameof(BoolValue1), new object() },
            { nameof(BoolValue2), new object() },
            { nameof(IntValue1), new object() },
            { nameof(IntValue2), new object() },
            { nameof(DoubleValue1), new object() },
            { nameof(DoubleValue2), new object() },
            { nameof(LongString), new object() },
            { nameof(Colors), new object() },
            { nameof(LastTimeModified), new object() },
            { nameof(GeneratedGuid), new object() },
        };

        private bool m_boolValue1;
        public bool BoolValue1
        {
            get
            {
                lock (m_propertyLocks[nameof(BoolValue1)])
                {
                    return m_boolValue1;
                }
            }
            set
            {
                lock (m_propertyLocks[nameof(BoolValue1)])
                {
                    SetProperty(ref m_boolValue1, value);
                }
            }
        }

        private bool m_boolValue2;
        public bool BoolValue2
        {
            get
            {
                lock (m_propertyLocks[nameof(BoolValue2)])
                {
                    return m_boolValue2;
                }
            }
            set
            {
                lock (m_propertyLocks[nameof(BoolValue2)])
                {
                    SetProperty(ref m_boolValue2, value);
                }
            }
        }

        private int m_intValue1;
        public int IntValue1
        {
            get
            {
                lock (m_propertyLocks[nameof(IntValue1)])
                {
                    return m_intValue1;
                }
            }
            set
            {
                lock (m_propertyLocks[nameof(IntValue1)])
                {
                    SetProperty(ref m_intValue1, value);
                }
            }
        }

        private int m_intValue2;
        public int IntValue2
        {
            get
            {
                lock (m_propertyLocks[nameof(IntValue2)])
                {
                    return m_intValue2;
                }
            }
            set
            {
                lock (m_propertyLocks[nameof(IntValue2)])
                {
                    SetProperty(ref m_intValue2, value);
                }
            }
        }

        private double m_doubleValue1;
        public double DoubleValue1
        {
            get
            {
                lock (m_propertyLocks[nameof(DoubleValue1)])
                {
                    return m_doubleValue1;
                }
            }
            set
            {
                lock (m_propertyLocks[nameof(DoubleValue1)])
                {
                    SetProperty(ref m_doubleValue1, value);
                }
            }
        }

        private double m_doubleValue2;
        public double DoubleValue2
        {
            get
            {
                lock (m_propertyLocks[nameof(DoubleValue2)])
                {
                    return m_doubleValue2;
                }
            }
            set
            {
                lock (m_propertyLocks[nameof(DoubleValue2)])
                {
                    SetProperty(ref m_doubleValue2, value);
                }
            }
        }

        private string m_longString;
        public string LongString
        {
            get
            {
                lock (m_propertyLocks[nameof(LongString)])
                {
                    return m_longString;
                }
            }
            set
            {
                lock (m_propertyLocks[nameof(LongString)])
                {
                    SetProperty(ref m_longString, value);
                }
            }
        }

        private List<Color> m_colors;
        public List<Color> Colors
        {
            get
            {
                lock (m_propertyLocks[nameof(Colors)])
                {
                    return m_colors;
                }
            }
            set
            {
                lock (m_propertyLocks[nameof(Colors)])
                {
                    SetProperty(ref m_colors, value);
                }
            }
        }

        private DateTime m_lastTimeModified;
        public DateTime LastTimeModified
        {
            get
            {
                lock (m_propertyLocks[nameof(LastTimeModified)])
                {
                    return m_lastTimeModified;
                }
            }
            set
            {
                lock (m_propertyLocks[nameof(LastTimeModified)])
                {
                    SetProperty(ref m_lastTimeModified, value);
                }
            }
        }

        private string? m_generatedGuid;
        public string? GeneratedGuid
        {
            get
            {
                lock (m_propertyLocks[nameof(GeneratedGuid)])
                {
                    return m_generatedGuid;
                }
            }
            set
            {
                lock (m_propertyLocks[nameof(GeneratedGuid)])
                {
                    SetProperty(ref m_generatedGuid, value);
                }
            }
        }

        public PropertyObject()
        {
            BoolValue1 = m_random.Next(0, 2) == 1;
            BoolValue2 = m_random.Next(0, 2) == 1;
            IntValue1 = m_random.Next(0, 100000);
            IntValue2 = m_random.Next(0, 1000);
            DoubleValue1 = m_random.NextDouble() * 100000;
            DoubleValue2 = m_random.NextDouble() * 1000;
            LastTimeModified = DateTime.Now;

            LongString = GenerateRandomString(30000);
            Colors = GenerateRandomColors();
            GeneratedGuid = GenerateGuidFromLongString();
        }

        // Egyedi GUID készítése a LongString alapján
        private string? GenerateGuidFromLongString()
        {
            int startIndex = m_random.Next(0, LongString.Length - 32);
            string guidSource = LongString.Substring(startIndex, 32);
            return $"{guidSource.Substring(0, 8)}-{guidSource.Substring(8, 4)}-{guidSource.Substring(12, 4)}-{guidSource.Substring(16, 4)}-{guidSource.Substring(20, 12)}";
        }

        private string GenerateRandomString(int p_length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            char[] stringChars = new char[p_length];
            for (int i = 0; i < p_length; i++)
            {
                stringChars[i] = chars[m_random.Next(chars.Length)];
            }
            return new string(stringChars);
        }

        private List<Color> GenerateRandomColors()
        {
            Random random = new Random();
            int colorCount = random.Next(1, 6);
            List<Color> colors = new List<Color>();
            for (int i = 0; i < colorCount; i++)
            {
                colors.Add(Color.FromRgb(random.Next(0, 256), random.Next(0, 256), random.Next(0, 256)));
            }
            return colors;
        }

        public object GetPropertyLock(string p_propertyName)
        {
            return m_propertyLocks[p_propertyName];
        }

        internal void StartOperation()
        {
            lock (m_activeOperationsLock)
            {
                m_activeOperations++;
            }
        }

        internal void EndOperation()
        {
            lock (m_activeOperationsLock)
            {
                m_activeOperations--;
                if (m_activeOperations == 0)
                {
                    Monitor.PulseAll(m_activeOperationsLock); // értesítjük a várakozó (törlési) műveleteket
                }
            }
        }

        internal void WaitForNoOperations()
        {
            lock (m_activeOperationsLock)
            {
                while (m_activeOperations > 0)
                {
                    Monitor.Wait(m_activeOperationsLock); // várakozás az aktív műveletek befejezésére
                }
            }
        }
    }
}
