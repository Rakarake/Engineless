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

        private static void SuperSystem(int a, string b, OmegaStruct c)
        {
        }

        private static void Delegators(Delegate method)
        {
            MethodInfo methodInfo = method.Method;
            ParameterInfo[] parameters = methodInfo.GetParameters();
            foreach (ParameterInfo parameter in parameters)
            {
                Console.WriteLine(parameter.ParameterType.Name);
            }
        }

        public static void TestMethod()
        {
            System.Console.WriteLine("Yes");
            Delegators(SuperSystem);
        }
    }
}
