using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using static System.Console;
using static System.Int32;

namespace DynamicInvokation
{
    public class Program
    {
        private static int method;
        private static int runNumber;
        private static void DefaultParameters()
        {
            method = (int)InvokationMethod.AllInvokations;
            runNumber = 1000000;
        }
        public static void Main(string[] args)
        {
            //parse command line args
            if (args.Length > 1)
            {
                TryParse(args[0], out method);
                TryParse(args[1], out runNumber);
            }
            else if (args.Length == 1)
            {
                TryParse(args[0], out method);
            }
            else
            {
                InputParametersFromConsole();
            }

            WriteLine($"Testing with method: {(InvokationMethod)method} with number of runs: {runNumber}. Press any key to continue...");
            ReadKey();

            WriteLine("Populating cache, press any key to continue... ");
            ReadKey();
            MethodsDictionary.FeedCache();


            TestApproaches(method, runNumber);

            ReadKey();
        }

        private static void InputParametersFromConsole()
        {
            WriteLine("Please provide number of the method that should be used for invokation:");
            foreach (var value in Enum.GetValues(typeof(InvokationMethod)))
            {
                WriteLine($"{(int)value}: {(InvokationMethod)value}");
            }
            var option = ReadLine();
            Clear();
            if (!TryParse(option, out method) || Enum.IsDefined(typeof(InvokationMethod), option))
            {
                DefaultParameters();
                WriteLine($"Provided method number is incorrect. Using default value: {InvokationMethod.AllInvokations}");
            }

            WriteLine("Please provide a positive, integer number of method invokations used for tests");
            var number = ReadLine();
            if (!TryParse(number, out runNumber) || runNumber < 1)
            {
                DefaultParameters();
                WriteLine($"Provided value is not a positive, integer value, using default value of {runNumber}");
            }
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
            switch ((InvokationMethod)method)
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
                        });
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
                WriteLine("Testing: {0}, press any key to continue...", name);
                ReadLine();

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
            WriteLine($"\n\n================ {name} ==============\n\n");
            foreach (var entry in dict.OrderBy(k => k.Value))
            {
                WriteLine("Method: {0,-25}  {1}: {2,-15}", entry.Key, name, entry.Value);
            }
        }
    }
}