using System;
using System.Reflection;

namespace Test
{
    struct OmegaStruct
    {
        public OmegaStruct() {}
        public int crazy = 0;
        public bool superDuper = true;
    }

    class TestClass
    {
        private static int Testers(System.Func<int, string, int> f)
        {
            return f(1, "");
        }

        private static void GenericSystem<T>() {
        }

        private static void GenericDelegators(Delegate method) {
            Type t = typeof(OmegaStruct);
            var x = GenericSystem<int>;
            foreach (var xt in x.GetType().GetGenericArguments()) {
                Console.WriteLine(xt);
            }
        }

        private static void SuperSystem(int a, string b, OmegaStruct c, System.Collections.Generic.Dictionary<int, int> d, System.Collections.Generic.Dictionary<int, string> e, (int, string, bool)[] f)
        {
        }

        private static void Delegators(Delegate method)
        {
            MethodInfo methodInfo = method.Method;
            ParameterInfo[] parameters = methodInfo.GetParameters();
            foreach (ParameterInfo parameter in parameters)
            {
                Type maybeTypeInsideArray = parameter.ParameterType.GetElementType();
                if (maybeTypeInsideArray != null) {
                    Console.WriteLine("Inside array --- " + maybeTypeInsideArray);
                    Type[] types = maybeTypeInsideArray.GetGenericArguments();
                    foreach (var t in types) {
                        Console.WriteLine("OK:");
                        Console.WriteLine(t);
                    }
                }
            }
        }

        public static void TestMethod()
        {
            System.Console.WriteLine("Yes");
            //Delegators(SuperSystem);
            GenericDelegators(GenericSystem<int>);
        }
    }
}
