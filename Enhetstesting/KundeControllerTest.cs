using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nettbanken.Controllers;
using Nettbanken.BLL;
using Nettbanken.DAL;
using SessionMockUnitTest.Controllers;
using MvcContrib.TestHelper;
namespace Enhetstesting
{
    [TestClass]
    public class KundeControllerTest
    {
        [TestMethod]
        public void adminLogginnViewSessionOK()
        {
            //Arrange
            var SessionMock = new TestControllerBuilder();
            var controller = new KundeController();
            // ???
            SessionMock.InitializeController(controller);

            //Act
            
        }
    }
}
