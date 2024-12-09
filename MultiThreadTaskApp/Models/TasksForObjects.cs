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
        public Task CreateTasks(CancellationToken token)
        {
            return new Task(() =>
            {
                TargetObject.StartOperation();
                try
                {
                    if (token.IsCancellationRequested) return;
                    switch (Type)
                    {
                        case TaskType.BoolNegate:
                            lock (TargetObject.GetPropertyLock(nameof(PropertyObject.BoolValue1)))
                            {
                                if (token.IsCancellationRequested) return;
                                TargetObject.BoolValue1 = !TargetObject.BoolValue1;
                            }
                            lock (TargetObject.GetPropertyLock(nameof(PropertyObject.BoolValue2)))
                            {
                                if (token.IsCancellationRequested) return;
                                TargetObject.BoolValue2 = !TargetObject.BoolValue2;
                            }
                            break;
                        case TaskType.IntIncrement:
                            lock (TargetObject.GetPropertyLock(nameof(PropertyObject.IntValue1)))
                            {
                                if (token.IsCancellationRequested) return;
                                TargetObject.IntValue1++;
                            }
                            lock (TargetObject.GetPropertyLock(nameof(PropertyObject.IntValue2)))
                            {
                                if (token.IsCancellationRequested) return;
                                TargetObject.IntValue2++;
                            }
                            break;
                        case TaskType.DoubleRandomization:
                            lock (TargetObject.GetPropertyLock(nameof(PropertyObject.DoubleValue1)))
                            {
                                if (token.IsCancellationRequested) return;
                                TargetObject.DoubleValue1 = m_random.NextDouble() * 100000;
                            }
                            lock (TargetObject.GetPropertyLock(nameof(PropertyObject.DoubleValue2)))
                            {
                                if (token.IsCancellationRequested) return;
                                TargetObject.DoubleValue2 = m_random.NextDouble() * 1000;
                            }
                            break;
                        /* // Lassítjuk a folyamatot ahogy karesz kérte
                    case TaskType.StringModifying:
                        lock(TargetObject.GetPropertyLock(nameof(PropertyObject.LongString)))
                        {
                            var builder = new StringBuilder(TargetObject.LongString);

                            int lengthToModify = Math.Min(100, builder.Length);
                            for (int i = 0; i < lengthToModify; i++)
                            {
                                builder[i] = char.ToUpper(builder[i]);

                                // Frissítés minden módosítás után
                                TargetObject.LongString = builder.ToString();
                            }
                            Thread.Sleep(50); 
                        }
                        break;
                        */
                        case TaskType.StringModifying:
                            lock (TargetObject.GetPropertyLock(nameof(PropertyObject.GeneratedGuid)))
                            {
                                if (token.IsCancellationRequested) return;
                                TargetObject.GeneratedGuid = TargetObject.GeneratedGuid.Substring(1) + "AAA";
                            }
                            break;
                        case TaskType.ColorRandomization:
                            lock (TargetObject.GetPropertyLock(nameof(PropertyObject.Colors)))
                            {
                                if (token.IsCancellationRequested) return;
                                int colorCount = m_random.Next(1, 6);
                                List<Color> colors = new List<Color>();
                                for (int i = 0; i < colorCount; i++)
                                {
                                    colors.Add(Color.FromRgb(m_random.Next(0, 256), m_random.Next(0, 256), m_random.Next(0, 256)));
                                }
                                TargetObject.Colors = colors;
                            }
                            break;
                    }
                    lock (TargetObject.GetPropertyLock(nameof(PropertyObject.LastTimeModified)))
                    {
                        if (token.IsCancellationRequested) return;
                        TargetObject.LastTimeModified = DateTime.Now;
                    }
                }
                finally
                {
                    TargetObject.EndOperation();
                }
            }, token);
        }
    }
}
