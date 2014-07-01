using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CSharp.RuntimeBinder;
using NUnit.Framework;
using WrapAndCast;

namespace WrapAndCastTests
{
    [TestFixture]
    public class CastItTests
    {
        private Castor castor { get; set; }
        private TestClass rawObj { get; set; }

        [TestFixtureSetUp]
        public void SetUp()
        {
            castor = new Castor();
            rawObj = new TestClass();
        }

        [Test]
        public void Test_Castor_1()
        {
            Assert.IsNotNull(castor);
        }

        [Test]
        public void Test_Castor_CacheType_1()
        {
            castor.CacheType(typeof(TestInterface1));
            Assert.IsTrue(CastIt.typeDict.ContainsKey(typeof(TestInterface1)));
            Assert.IsNotNull(CastIt.typeDict[typeof(TestInterface1)]);
            CleanUp();
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void Test_Castor_CacheType_2()
        {
            castor.CacheType(typeof(String));
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Test_Castor_CacheType_3()
        {
            castor.CacheType(null);
        }

        [Test]
        public void Test_Castor_CleanType_1()
        {
            castor.CacheType(typeof(TestInterface1));
            castor.CleanType(typeof(TestInterface1));
            Assert.IsFalse(CastIt.typeDict.ContainsKey(typeof(TestInterface1)));
            CleanUp();
        }

        [Test]
        public void Test_Castor_CacheFromThisAssembly_1()
        {
            castor.CacheFromThisAssembly(this);
            Assert.IsTrue(CastIt.typeDict.ContainsKey(typeof(TestInterface1)));
            Assert.IsNotNull(CastIt.typeDict[typeof(TestInterface1)]);

            Assert.IsTrue(CastIt.typeDict.ContainsKey(typeof(TestInterface2)));
            Assert.IsNotNull(CastIt.typeDict[typeof(TestInterface2)]);
            CleanUp();
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Test_Castor_CacheFromThisAssembly_2()
        {
            castor.CacheFromThisAssembly(null);
        }

        [Test]
        public void Test_Castor_To_1()
        {
            var wrapped = castor.To(typeof(TestInterface1), rawObj) as TestInterface1;
            Assert.IsNotNull(wrapped);
            Assert.IsInstanceOf<TestInterface1>(wrapped);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Test_Castor_To_2()
        {
            var wrapped = castor.To(null, rawObj);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Test_Castor_To_3()
        {
            var wrapped = castor.To(typeof(TestInterface1), null);
        }

        [Test]
        public void Test_Castor_To_4()
        {
            var wrapped = castor.To(typeof(TestInterface1), rawObj) as TestInterface1;
            wrapped.TestMethod2();
            Assert.IsTrue(wrapped.StringProp == "TestMethod2");
        }

        [Test]
        [ExpectedException(typeof(RuntimeBinderException))]
        public void Test_Castor_To_5()
        {
            var wrapped = castor.To(typeof(TestInterface1), rawObj) as TestInterface1;
            wrapped.MissingMethod();
        }




        private void CleanUp()
        {
            CastIt.typeDict.Clear();
        }
    }
}
