using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WrapAndCastTests
{
    public interface TestInterface2
    {
        string StringProp { get; }

        void TestMethod2();

        void MissingMethod();
    }
}
