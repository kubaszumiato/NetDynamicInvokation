using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DynamicInvokation
{
    public class Program
    {
        public static void Main(string[] args)
        {
            int method = 5;
            int runNumber = 1000000;
            try
            {
                if (args.Length > 1)
                {
                    Int32.TryParse(args[0], out method);
                    Int32.TryParse(args[1], out runNumber);
                }
                else if (args.Length == 1)
                {
                    Int32.TryParse(args[0], out method);
                }
            }
            catch
            {
                Console.WriteLine("There was a problem, when applying custom starting arguments. Trying to continue the process...");
            }

            Console.WriteLine("Populating cache, press any key to continue... ");
            Console.ReadLine();
            MethodsDictionary.FeedCache();

            Console.WriteLine($"Testing with method: {(InvokationMethod)method} with number of runs: {runNumber}. Press any key to continue...");
            Console.ReadLine();
            TestApproaches(method, runNumber);

            Console.ReadKey();
        }

        public enum InvokationMethod
        {
            MethodInfo = 0,
            DelegateDynamicInvoke = 1,
            DelegateInvoke = 2,
            ExpressionTrees = 3,
            DirectInvokation = 4,
            AllInvokations = 5
        }

        public static void TestApproaches(int method, int runNumber)
        {
            List<ITester> testers = new List<ITester>();
            switch ((InvokationMethod) method)
            {
                case InvokationMethod.DirectInvokation:
                    testers.Add(new DirectInvoke());
                    break;
                case InvokationMethod.ExpressionTrees:
                    testers.Add(new ExpressionTreesInvoke());
                    break;
                case InvokationMethod.DelegateDynamicInvoke:
                    testers.Add(new DelegateDynamicInvoke());
                    break;
                case InvokationMethod.DelegateInvoke:
                    testers.Add(new DelegateInvoke());
                    break;
                case InvokationMethod.AllInvokations:
                    testers.AddRange( 
                        new ITester[]
                        {
                            new DirectInvoke(),
                            new DelegateDynamicInvoke(),
                            new DelegateInvoke(),
                            new ExpressionTreesInvoke(), 
                            new MethodInfoInvoke()
                        } );
                    break;
                default:
                    testers.Add(new MethodInfoInvoke());
                    break;
            }
            Dictionary<string, TimeSpan> totals = new Dictionary<string, TimeSpan>();
            Dictionary<string, TimeSpan> setups = new Dictionary<string, TimeSpan>();
            Dictionary<string, TimeSpan> methods = new Dictionary<string, TimeSpan>();
            testers.ForEach(t =>
            {
                var name = t.GetType().Name;
                Console.WriteLine("Testing: {0}, press any key to continue...", name);
                Console.ReadLine();

                Stopwatch sw = new Stopwatch();

                //setup
                sw.Start();

                t.Setup();

                sw.Stop();
                TimeSpan setup = sw.Elapsed;
                setups.Add(name, setup);

                //method
                sw.Restart();

                t.TestIteratively(runNumber);

                sw.Stop();
                TimeSpan executionTime = sw.Elapsed;
                methods.Add(name, executionTime);

                //total
                totals.Add(name, setup + executionTime);
            });

            
            PrintDict(nameof(setups), setups);
            PrintDict(nameof(methods), methods);
            PrintDict(nameof(totals), totals);
        }

        private static void PrintDict(string name, Dictionary<string, TimeSpan> dict)
        {
            Console.WriteLine($"\n\n================ {name} ==============\n\n");
            foreach (var entry in dict.OrderBy(k => k.Value))
            {
                Console.WriteLine("Method: {0,-25}  {1}: {2,-15}", entry.Key, name, entry.Value);
            }
        }
    }
}