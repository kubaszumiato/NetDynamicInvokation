namespace DynamicInvokation
{
    class ExpressionTreesInvoke : ITester
    {
        private static LateBoundMethod _mi1;
        private static LateBoundMethod _mi2;
        private static object[] _stringMethodParams;
        private static object[] _intMethodParams;
        public static void TestIteratively(int runNumber)
        {
            for (int i = 0; i < runNumber; i++)
            {
                _mi1.Invoke(null, _stringMethodParams);
                _mi2.Invoke(null, _intMethodParams);
            }
        }

        void ITester.Setup()
        {
            _mi1 = DelegateFactory.Create(typeof(PublicReadMethods).GetMethod("TestString"));
            _mi2 = DelegateFactory.Create(typeof(PublicReadMethods).GetMethod("TestInt"));

            _stringMethodParams = new object[] { "lalamido" };
            _intMethodParams = new object[] { 1, 2 };
        }

        void ITester.TestIteratively(int runNumber)
        {
            TestIteratively(runNumber);
        }
    }
}
