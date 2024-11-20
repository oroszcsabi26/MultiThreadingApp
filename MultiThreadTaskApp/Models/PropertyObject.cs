namespace MultiThreadTaskApp.Models
{
    public class PropertyObject
    {
        private static readonly Random random = new Random();

        public bool BoolValue1 { get; set; }
        public bool BoolValue2 { get; set; }
        public int IntValue1 { get; set; }
        public int IntValue2 { get; set; }
        public double DoubleValue1 { get; set; }
        public double DoubleValue2 { get; set; }
        public string LongString { get; set; } 
        public List<Color> Colors { get; set; } 
        public DateTime LastTimeModified { get; set; }
        public string GeneratedGuid { get; private set; } // Egyedi GUID készítése a LongString alapján
        public PropertyObject()
        {
            BoolValue1 = random.Next(0, 2) == 1;
            BoolValue2 = random.Next(0, 2) == 1;
            IntValue1 = random.Next(0, 100000);
            IntValue2 = random.Next(0, 1000);
            DoubleValue1 = random.NextDouble() * 100000;
            DoubleValue2 = random.NextDouble() * 1000;
            LastTimeModified = DateTime.Now;

            LongString = GenerateRandomString(30000);
            Colors = GenerateRandomColors();
            GeneratedGuid = GenerateGuidFromLongString();
        }

        private string? GenerateGuidFromLongString()
        {
            int startIndex = random.Next(0, LongString.Length - 32);
            string guidSource = LongString.Substring(startIndex, 32);
            return $"{guidSource.Substring(0, 8)}-{guidSource.Substring(8, 4)}-{guidSource.Substring(12, 4)}-{guidSource.Substring(16, 4)}-{guidSource.Substring(20, 12)}";
        }

        private string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            char[] stringChars = new char[length];
            for (int i = 0; i < length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
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
