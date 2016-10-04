using UWIC.FinalProject.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace UWIC.FinalProject.UnitTests
{
    
    
    /// <summary>
    ///This is a test class for ConversionsTest and is intended
    ///to contain all ConversionsTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ConversionsTest
    {
        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        /// <summary>
        ///A test for ConvertStringToEnum
        ///</summary>
        [TestMethod()]
        public void ConvertStringToEnumTestHelper<T>()
            where T : struct
        {
            string value = string.Empty; // TODO: Initialize to an appropriate value
            T expected = new T(); // TODO: Initialize to an appropriate value
            T actual;
            actual = Conversions.ConvertStringToEnum<T>(value);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        public void ConvertStringToEnumTest()
        {
            Assert.Inconclusive("No appropriate type parameter is found to satisfies the type constraint(s) of T. " +
                    "Please call ConvertStringToEnumTestHelper<T>() with appropriate type parameters." +
                    "");
        }

        /// <summary>
        ///A test for ConvertIntegerToEnum
        ///</summary>
        [TestMethod()]
        public void ConvertIntegerToEnumTestHelper<T>()
            where T : struct
        {
            int value = 0; // TODO: Initialize to an appropriate value
            T expected = new T(); // TODO: Initialize to an appropriate value
            T actual;
            actual = Conversions.ConvertIntegerToEnum<T>(value);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        public void ConvertIntegerToEnumTest()
        {
            Assert.Inconclusive("No appropriate type parameter is found to satisfies the type constraint(s) of T. " +
                    "Please call ConvertIntegerToEnumTestHelper<T>() with appropriate type parameters" +
                    ".");
        }

        /// <summary>
        ///A test for ConvertEnumToInt
        ///</summary>
        [TestMethod()]
        public void ConvertEnumToIntTest()
        {
            Enum value = CommandType.start_dictation_mode; //Command Type Enum
            var expected = 1; // Expected value is 1
            int actual;
            actual = Conversions.ConvertEnumToInt(value);
            Assert.AreEqual(expected, actual);
        }
    }
}
