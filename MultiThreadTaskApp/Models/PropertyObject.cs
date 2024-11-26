using MultiThreadTaskApp.ViewModels;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MultiThreadTaskApp.Models
{
    public class PropertyObject : BaseViewModel, INotifyPropertyChanged
    {
        private static readonly Random m_random = new Random();

        private bool m_boolValue1;
        public bool BoolValue1
        {
            get => m_boolValue1;
            set => SetProperty(ref m_boolValue1, value);
        }

        private bool m_boolValue2;
        public bool BoolValue2
        {
            get => m_boolValue2;
            set => SetProperty(ref m_boolValue2, value);
        }
        private int m_intValue1;
        public int IntValue1
        {
            get => m_intValue1;
            set => SetProperty(ref m_intValue1, value);
        }

        private int m_intValue2;
        public int IntValue2
        {
            get => m_intValue2;
            set => SetProperty(ref m_intValue2, value);
        }

        private double m_doubleValue1;
        public double DoubleValue1
        {
            get => m_doubleValue1;
            set => SetProperty(ref m_doubleValue1, value);
        }

        private double m_doubleValue2;
        public double DoubleValue2
        {
            get => m_doubleValue2;
            set => SetProperty(ref m_doubleValue2, value);
        }

        private string m_longString;
        public string LongString
        {
            get => m_longString;
            set => SetProperty(ref m_longString, value);
        } 

        private List<Color> m_colors;
        public List<Color> Colors
        {
            get => m_colors;
            set => SetProperty(ref m_colors, value);
        } 

        private DateTime m_lastTimeModified;
        public DateTime LastTimeModified
        {
            get => m_lastTimeModified;
            set => SetProperty(ref m_lastTimeModified, value);
        }

        private string? m_generatedGuid;
        public string? GeneratedGuid
        {
            get => m_generatedGuid;
            set => SetProperty(ref m_generatedGuid, value);
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
    }
}
