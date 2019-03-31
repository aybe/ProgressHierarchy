using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal static class Program
    {
        private static async Task Main()
        {
            var root = new ProgressNode {Name = "Root"};

            var child1 = root.CreateChild("Child 1");
            child1.CreateChild("Child 1A");
            child1.CreateChild("Child 1B");

            var child2 = root.CreateChild("Child 2");
            child2.CreateChild("Child 2A");
            child2.CreateChild("Child 2B");

            var child3 = root.CreateChild("Child 3");
            child3.CreateChild("Child 3A");
            child3.CreateChild("Child 3B");

            var child4 = root.CreateChild("Child 4");
            child4.CreateChild("Child 4A");
            child4.CreateChild("Child 4B");

            root.ProgressChanged += OnProgressChanged;

            var nodes = root.GetLeafs().ToArray();
            var tasks = new List<Task>();

            var random = new Random();

            foreach (var node in nodes)
            {
                var task = Task.Run(async () =>
                {
                    var total = random.Next(1, 50 + 1);
                    var delay = random.Next(10, 100 + 1);

                    for (var i = 0; i < total; i++)
                    {
                        await Task.Delay(delay);
                        node.SetProgress(total, i + 1);
                    }
                });
                tasks.Add(task);
            }

            await Task.WhenAll(tasks);
            Console.WriteLine("Done!");
            Console.ReadKey();
        }

        #region Console output

        private static readonly object Sync = new object();

        private static void OnProgressChanged(object sender, EventArgs e)
        {
            var root = (ProgressNode) sender;

            lock (Sync) // cheap hack for slow console
            {
                Console.SetCursorPosition(0, 0); // another cheap hack

                var flatten = root.Flatten(s => s);
                foreach (var node in flatten)
                {
                    var indent = 0;
                    var current = node;
                    while (current.Parent != null)
                    {
                        current = current.Parent;
                        indent++;
                    }

                    var value = node.GetTotalProcessed();
                    var total = node.GetTotalElements();
                    var progress = GetProgressBar(50, value, total);
                    var text = $"{new string(' ', indent)} '{node.Name}' {progress} {value} of {total}\r\n";
                    Console.WriteLine(text);
                }
            }
        }

        public static string GetProgressBar(int width, int value, int total, char foreground = '█', char background = '░')
        {
            var d = 1.0d / Math.Max(total, 1) * value; // avoid division by zero
            var i = (int) Math.Round(d * width);
            var s1 = new string(foreground, i);
            var s2 = new string(background, width - i);
            var s3 = $"{s1}{s2}";
            return s3;
        }

        #endregion
    }
}