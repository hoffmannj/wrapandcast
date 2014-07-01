using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WrapAndCastTests
{
    public class TestClass
    {
        public int IntProp { get; set; }

        public T TestMethod1<T>()
        {
            return default(T);
        }

        public string StringProp { get; private set; }

        public void TestMethod2()
        {
            StringProp = "TestMethod2";
        }


        public TestClass()
        {
            IntProp = 0;
            StringProp = string.Empty;
        }
    }
}
