using System;

namespace DynamicInvokation
{
    class DelegateInvoke : ITester
    {
        private static Func<string, string> _testString;
        private static Func<int, int, int> _testInt;

        public static void TestIteratively(int runNumber)
        {
            for (int i = 0; i < runNumber; i++)
            {
                _testString.Invoke("lalamido");
                _testInt.Invoke(1, 2);
            }
        }

        void ITester.Setup()
        {
            _testString = PublicReadMethods.TestString;
            _testInt = PublicReadMethods.TestInt;

        }
        void ITester.TestIteratively(int runNumber)
        {
            TestIteratively(runNumber);
        }
    }

}
