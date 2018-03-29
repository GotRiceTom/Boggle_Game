using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetGUI;

namespace SpreadsheetUnitTest
{
    [TestClass]
    public class UnitTest1
    {
        /// <summary>
        /// Test closing a spreadsheet
        /// </summary>
        [TestMethod]
        public void TestClose()
        {
            ControllerStub stub = new ControllerStub();

            SpreadsheetControllers controller = new SpreadsheetControllers(stub, null);
            stub.FireCloseEvent();
            Assert.IsTrue(stub.CalledDoClose);
        }

        /// <summary>
        /// Test saving a spreadsheet
        /// </summary>
        [TestMethod]
        public void TestSave()
        {
            ControllerStub stub = new ControllerStub();

            SpreadsheetControllers controller = new SpreadsheetControllers(stub, null);
            stub.FireSaveEvent();
            Assert.IsTrue(controller.ReachedSaved);
        }

        /// <summary>
        /// Testing arrow keys pressed DOWN
        /// </summary>
        [TestMethod]
       // [ExpectedException(typeof(NotImplementedException))]
        public void TestArrowKeysDown()
        {
            ControllerStub stub = new ControllerStub();

            SpreadsheetControllers controller = new SpreadsheetControllers(stub, null);
            stub.FireKeyArrowsEventDown();

            Assert.IsTrue(controller.ReachedSaved);
        }

        /// <summary>
        /// Testing arrow keys pressed UP
        /// </summary>
        [TestMethod]
        public void TestArrowKeysUp()
        {
            ControllerStub stub = new ControllerStub();

            SpreadsheetControllers controller = new SpreadsheetControllers(stub, null);
            stub.FireKeyArrowsEventUp();

            Assert.IsTrue(controller.ReachedSaved);
        }

        /// <summary>
        /// Testing arrow keys pressed LEFT
        /// </summary>
        [TestMethod]
        public void TestArrowKeysLeft()
        {
            ControllerStub stub = new ControllerStub();

            SpreadsheetControllers controller = new SpreadsheetControllers(stub, null);
            stub.FireKeyArrowsEventLeft();

            Assert.IsTrue(controller.ReachedSaved);
        }

        
        /// <summary>
        /// Testing arrow keys pressed RIGHT
        /// </summary>
        [TestMethod]
       // [ExpectedException(typeof(NotImplementedException))]
        public void TestArrowKeysRight()
        {
            ControllerStub stub = new ControllerStub();

            SpreadsheetControllers controller = new SpreadsheetControllers(stub, null);
            stub.FireKeyArrowsEventRight();

        }

        /// <summary>
        /// Test clicking help
        /// </summary>
        [TestMethod]
        public void TestHelp()
        {
            ControllerStub stub = new ControllerStub();

            SpreadsheetControllers controller = new SpreadsheetControllers(stub, null);
            stub.FireHelpEvent();
            Assert.IsTrue(stub.CalledDoClose);
        }



        [TestMethod]
        public void TestOpen()
        {
            ControllerStub stub = new ControllerStub();

            SpreadsheetControllers controller = new SpreadsheetControllers(stub, null);
            stub.FireOpenEvent();
            Assert.IsTrue(controller.ReachedSaved);
        }


        [TestMethod]
        public void TestChange()
        {
            ControllerStub stub = new ControllerStub();

            SpreadsheetControllers controller = new SpreadsheetControllers(stub, null);
            stub.FireChangeButton();
            Assert.IsTrue(controller.ReachedSaved);
        }

    }
}
