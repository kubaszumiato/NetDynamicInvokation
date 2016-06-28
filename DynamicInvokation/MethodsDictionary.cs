using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.Caching;

namespace DynamicInvokation
{
    internal class MethodsDictionary
    {
        private static readonly ObjectCache MethodsCache = MemoryCache.Default;

        static MethodsDictionary()
        {
            FeedCache();
        }

        MethodsDictionary(bool feedCache)
        {
            if (feedCache)
            {
                FeedCache();
            }
        }


        public static ConcurrentDictionary<string, LateBoundMethod> Methods
        {
            get
            {
                if (!MethodsCache.Contains("MethodsDictionary"))
                {
                    MethodsCache.Add("MethodsDictionary", new ConcurrentDictionary<string, LateBoundMethod>()
                        , DateTimeOffset.MaxValue);
                }
                return MethodsCache["MethodsDictionary"] as ConcurrentDictionary<string, LateBoundMethod>;

            }
        }

        public static ConcurrentDictionary<string, ParameterInfo[]> Parameters
        {
            get
            {
                if (!MethodsCache.Contains("ParametersDictionary"))
                {
                    MethodsCache.Add("ParametersDictionary", new ConcurrentDictionary<string, ParameterInfo[]>()
                        , DateTimeOffset.MaxValue);
                }
                return MethodsCache["ParametersDictionary"] as ConcurrentDictionary<string, ParameterInfo[]>;

            }
        }
        /// <summary>
        /// Populates cache with methods compilation before first use.
        /// The default type is used 
        /// </summary>
        public static void FeedCache()
        {
            Type defaultReadType = typeof(IPublicReadMethods);
            FeedCache(defaultReadType);

            // readtype?
        }

        public static void FeedCache(Type t)
        {
            var staticPublicMethods = t.GetMethods(BindingFlags.Static | BindingFlags.Public);
            for (int i = 0; i < staticPublicMethods.Length; i++)
            {
                MethodInfo method = staticPublicMethods[i];
                Methods[method.Name] = DelegateFactory.Create(method);
                Parameters[method.Name] = method.GetParameters();
            }
        }
    }
}
