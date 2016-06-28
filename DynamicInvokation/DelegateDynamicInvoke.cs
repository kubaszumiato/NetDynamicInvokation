using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DynamicInvokation
{
    class DelegateDynamicInvoke : ITester
    {
        private static Delegate CreateDelegate(MethodInfo info)
        {
            return Delegate.CreateDelegate(Expression.GetDelegateType(
                (from parameter in info.GetParameters() select parameter.ParameterType)
                    .Concat(new[] { info.ReturnType })
                    .ToArray()), info);
        }

        
        private static Delegate _mi1;
        private static Delegate _mi2;
        private static object[] _stringMethodParams;
        private static object[] _intMethodParams;

        public static void Setup()
        {
            var info1 = typeof(PublicReadMethods).GetMethod("TestString");
            _mi1 = CreateDelegate(info1);


            var info2 = typeof(PublicReadMethods).GetMethod("TestInt");
            _mi2 = CreateDelegate(info2);

            _stringMethodParams = new object[] { "lalamido" };
            _intMethodParams = new object[] { 1, 2 };
        }
        public static void TestIteratively(int runNumber)
        {
            for (int i = 0; i < runNumber; i++)
            {
                _mi1.DynamicInvoke(_stringMethodParams);
                _mi2.DynamicInvoke(_intMethodParams);
            }
        }

        void ITester.Setup()
        {
            Setup();
        }
        void ITester.TestIteratively(int runNumber)
        {
            
            TestIteratively(runNumber);
        }
    }

}
