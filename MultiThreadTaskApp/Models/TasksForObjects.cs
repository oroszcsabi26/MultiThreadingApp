using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiThreadTaskApp.Models
{
    public class TasksForObjects
    {
        public enum TaskType
        {
            BoolNegate,
            IntIncrement,
            DoubleRandomization,
            StringModifying,
            ColorRandomization
        }

        public TaskType Type { get; set; } // feladat típus
        public PropertyObject TargetObject { get; set; } // melyik objektumon hajtsuk végre a feladatot

        private static readonly Random m_random = new Random();

        public TasksForObjects(TaskType p_type, PropertyObject p_targetObject)
        {
            Type = p_type;
            TargetObject = p_targetObject;
        }

        // A feladatok végrehajtása
        public void Execute()
        {
            lock (TargetObject)
            {
                switch (Type)
                {
                    case TaskType.BoolNegate:
                        TargetObject.BoolValue1 = !TargetObject.BoolValue1;
                        TargetObject.BoolValue2 = !TargetObject.BoolValue2;
                        break;
                    case TaskType.IntIncrement:
                        TargetObject.IntValue1++;
                        TargetObject.IntValue2++;
                        break;
                    case TaskType.DoubleRandomization:
                        TargetObject.DoubleValue1 = m_random.NextDouble() * 100000;
                        TargetObject.DoubleValue2 = m_random.NextDouble() * 1000;
                        break;
                    case TaskType.StringModifying:
                        TargetObject.GeneratedGuid = TargetObject.GeneratedGuid.Substring(1) + "AAA";
                        break;
                    case TaskType.ColorRandomization:
                        int colorCount = m_random.Next(1, 6);
                        List<Color> colors = new List<Color>();
                        for (int i = 0; i < colorCount; i++)
                        {
                            colors.Add(Color.FromRgb(m_random.Next(0, 256), m_random.Next(0, 256), m_random.Next(0, 256)));
                        }
                        TargetObject.Colors = colors; //TargetObject.Colors = new List<Color>(newColors); ???
                        break;
                }
                TargetObject.LastTimeModified = DateTime.Now;
            }
        }
    }
}
