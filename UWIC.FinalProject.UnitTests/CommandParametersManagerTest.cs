using UWIC.FinalProject.SpeechProcessingEngine.Commands;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace UWIC.FinalProject.UnitTests
{
    
    
    /// <summary>
    ///This is a test class for CommandParametersManagerTest and is intended
    ///to contain all CommandParametersManagerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class CommandParametersManagerTest
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
        ///A test for GetTabIndexForGoToTabCommand
        ///</summary>
        [TestMethod()]
        public void GetTabIndexForGoToTabCommandTest()
        {
            string command = "go to tab one"; // TODO: Initialize to an appropriate value
            string expected = "1"; // TODO: Initialize to an appropriate value
            string actual;
            actual = CommandParametersManager.GetTabIndexForGoToTabCommand(command);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for GetWebsiteNameForGoCommand
        ///</summary>
        [TestMethod()]
        public void GetWebsiteNameForGoCommandTest()
        {
            var commandSegments = new List<string> {"go", "to", "google"}; // The command provided by the user
            var expected = "facebook"; // Expected value
            string actual;
            actual = CommandParametersManager.GetWebsiteNameForGoCommand(commandSegments);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for GetxyValuesToMouseMoveCommand
        ///</summary>
        [TestMethod()]
        public void GetxyValuesToMouseMoveCommandTest()
        {
            var command = "go to zero one two three four five six seven"; // TODO: Initialize to an appropriate value
            var expected = "0123,4567"; // TODO: Initialize to an appropriate value
            string actual;
            actual = CommandParametersManager.GetxyValuesToMouseMoveCommand(command);
            Assert.AreEqual(expected, actual);
        }
    }
}
