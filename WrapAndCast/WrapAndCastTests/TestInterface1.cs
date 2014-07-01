using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WrapAndCastTests
{
    public interface TestInterface1 : TestInterface2
    {
        int IntProp { get; set; }

        T TestMethod1<T>();
    }
}
