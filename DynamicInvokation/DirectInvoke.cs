namespace DynamicInvokation
{
    class DirectInvoke : ITester
    {
        public static void TestIteratively(int runNumber)
        {
            for (int i = 0; i < runNumber; i++)
            {
                PublicReadMethods.TestString("lalamido");
                PublicReadMethods.TestInt(1, 2);
            }
        }


        void ITester.Setup()
        {
            //nothing to setup here :)
        }
        void ITester.TestIteratively(int runNumber)
        {
            TestIteratively(runNumber);
        }
    }
}
