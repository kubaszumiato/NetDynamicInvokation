using System.Reflection;

namespace DynamicInvokation
{
    internal class MethodInfoInvoke : ITester
    {
        private static MethodInfo _mi1;
        private static MethodInfo _mi2;
        private static object[] _stringMethodParams;
        private static object[] _intMethodParams;
        void ITester.Setup()
        {
            _mi1 = typeof(PublicReadMethods).GetMethod("TestString");
            _mi2 = typeof(PublicReadMethods).GetMethod("TestInt");
            _stringMethodParams = new object[] { "lalamido" };
            _intMethodParams = new object[] { 1, 2 };
        }
        public static void TestIteratively(int runNumber)
        {
            for (int i = 0; i < runNumber; i++)
            {
                _mi1.Invoke(null, _stringMethodParams);
                _mi2.Invoke(null, _intMethodParams);
            }
        }

        void ITester.TestIteratively(int runNumber)
        {
            TestIteratively(runNumber);
        }
    }
}
