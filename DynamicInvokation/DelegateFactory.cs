using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DynamicInvokation
{
    /// <summary>
    /// LateBoundMethod is a generic method signature that is passed an instance
    /// and an array of parameters and returns an object. It basically can be 
    /// used to call any method.
    /// 
    /// </summary>
    /// <param name="target">The instance that the dynamic method is called on</param>
    /// <param name="arguments"></param>
    /// <returns></returns>
    public delegate object LateBoundMethod(object target, object[] arguments);




    /// <summary>
    /// This class creates a generic method delegate from a MethodInfo signature
    /// converting the method call into a LateBoundMethod delegate call. Using
    /// this class allows making repeated calls very quickly.
    /// 
    /// Note: this class will be very inefficient for individual dynamic method
    /// calls - compilation of the expression is very expensive up front, so using
    /// this delegate factory makes sense only if you re-use and cache the dynamicly 
    /// loaded method repeatedly.
    /// 
    /// Entirely based on Nate Kohari's blog post:
    /// http://kohari.org/2009/03/06/fast-late-bound-invocation-with-expression-trees/
    /// </summary>
    public static class DelegateFactory
    {

        /// <summary>
        /// Creates a LateBoundMethod delegate from a MethodInfo structure
        /// Basically creates a dynamic delegate on the fly.
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public static LateBoundMethod Create(MethodInfo method)
        {
            ParameterExpression instanceParameter = Expression.Parameter(typeof(object), "target");
            ParameterExpression argumentsParameter = Expression.Parameter(typeof(object[]), "arguments");

            MethodCallExpression call = Expression.Call(
                //Expression.Convert(instanceParameter, method.DeclaringType),
                method,
                CreateParameterExpressions(method, argumentsParameter));

            Expression<LateBoundMethod> lambda = Expression.Lambda<LateBoundMethod>(
                Expression.Convert(call, typeof(object)),
                instanceParameter,
                argumentsParameter);

            return lambda.Compile();
        }

        /// <summary>
        /// Creates a LateBoundMethod from type methodname and parameter signature that
        /// is turned into a MethodInfo structure and then parsed into a dynamic delegate
        /// </summary>
        /// <param name="type"></param>
        /// <param name="methodName"></param>
        /// <param name="parameterTypes"></param>
        /// <returns></returns>
        public static LateBoundMethod Create(Type type, string methodName, params Type[] parameterTypes)
        {
            return Create(type.GetMethod(methodName, parameterTypes));
        }

        private static Expression[] CreateParameterExpressions(MethodInfo method, Expression argumentsParameter)
        {
            return method.GetParameters().Select((parameter, index) =>
                Expression.Convert(
                    Expression.ArrayIndex(argumentsParameter, Expression.Constant(index)),
                    parameter.ParameterType)).ToArray();
        }
    }



    /// <summary>
    /// Collection of Reflection and type conversion related utility functions
    /// </summary>
    public class ReflectionUtils
    {

        /// <summary>
        /// Binding Flags constant to be reused for all Reflection access methods.
        /// </summary>
        public const BindingFlags MemberAccess =
            BindingFlags.Public | BindingFlags.NonPublic |
            BindingFlags.Static | BindingFlags.Instance | BindingFlags.IgnoreCase;



        public static object CallMethod(object instance, string method, Type[] parameterTypes, params object[] parms)
        {
            if (parameterTypes == null && parms.Length > 0)
                // Call without explicit parameter types - might cause problems with overloads    
                // occurs when null parameters were passed and we couldn't figure out the parm type
                return instance.GetType().GetMethod(method, MemberAccess | BindingFlags.InvokeMethod).Invoke(instance, parms);
            else
            // Call with parameter types - works only if no null values were passed
                return instance.GetType().GetMethod(method, MemberAccess | BindingFlags.InvokeMethod, null, parameterTypes, null).Invoke(instance, parms);
        }


        public static object CallMethod(object instance, string method, params object[] parms)
        {
            // Pick up parameter types so we can match the method properly
            Type[] parameterTypes = null;
            if (parms != null)
            {
                parameterTypes = new Type[parms.Length];
                for (int x = 0; x < parms.Length; x++)
                {
                    // if we have null parameters we can't determine parameter types - exit
                    if (parms[x] == null)
                    {
                        parameterTypes = null;  // clear out - don't use types        
                        break;
                    }
                    parameterTypes[x] = parms[x].GetType();
                }
            }
            return CallMethod(instance, method, parameterTypes, parms);
        }

    }
}