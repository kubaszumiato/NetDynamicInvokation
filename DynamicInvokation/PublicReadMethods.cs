namespace DynamicInvokation
{
    public interface IPublicReadMethods
    {
        string TestString(string a);
        int TestInt(int a, int b);
    }

    public class PublicReadMethods : IPublicReadMethods
    {
        public static string TestString(string a)
        {
            return a + "_res";
        }

        int IPublicReadMethods.TestInt(int a, int b)
        {
            return TestInt(a, b);
        }

        string IPublicReadMethods.TestString(string a)
        {
            return TestString(a);
        }

        public static int TestInt(int a, int b)
        {
            return a + b;
        }
    }
}