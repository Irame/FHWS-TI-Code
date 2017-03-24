using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Graphs.Utils
{
    static class Extensions
    {
        public static void PushRange<T>(this Stack<T> stack, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                stack.Push(item);
            }
        }

        public static void EnqueueRange<T>(this Queue<T> stack, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                stack.Enqueue(item);
            }
        }

        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> values)
        {
            foreach (var value in values)
            {
                collection.Add(value);
            }
        }

        public static bool IsEmpty(this ICollection collection)
        {
            return collection.Count == 0;
        }

        public static bool IsEmpty<T>(this ICollection<T> collection)
        {
            return collection.Count == 0;
        }

        public static ValueWraper<T> Wrap<T>(this T value)
        {
            return new ValueWraper<T>(value);
        }

        public static T GetChildOfType<T>(this DependencyObject depObj)
            where T : DependencyObject
        {
            if (depObj == null) return null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);

                var result = (child as T) ?? GetChildOfType<T>(child);
                if (result != null) return result;
            }
            return null;
        }

        public static async Task DelayedForEach<T>(this IEnumerable<T> enumerable, Action<T> action, int msDelay = 500)
        {
            await Task.Run(() =>
            {
                foreach (var item in enumerable)
                {
                    action(item);
                    Task.Delay(msDelay);
                }
            });
        }
    }
}
