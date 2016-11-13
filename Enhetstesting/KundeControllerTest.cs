using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nettbanken.Controllers;
using Nettbanken.BLL;
using Nettbanken.DAL;
//using SessionMockUnitTest.Controllers;
using MvcContrib.TestHelper;using System.Collections.Generic;
using Nettbanken.Models;

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
            //SessionMock.InitializeController(controller);

            
        }

        [TestMethod]
        public void test_alleKunder()
        {
            //Arrange
            var controller = new KundeController(new NettbankBLL(new NettbankDALStub()));

            var forventetKundeListe = new List<Kunde>();
            String[] personnr = { "111000111", "222000222", "222000222" };
            String[] fornavn = { "Per", "Ola", "Markus" };
            String[] etternavn = { "Pettersen", "Steinsnes", "Holia" };
            String[] adresse = { "Kalmia 12", "Holeveien 9", "Kalkis 43" };
            String[] postnr = { "4390", "5670", "7080" };
            String[] poststed = { "Stensrud", "Markesund", "Relgia" };
            String[] telefonnr = { "11223344", "55667788", "99001122" };

            for (int i = 0; i < personnr.Length; i++)
            {
                var kunde = new Kunde()
                {
                    personNr = personnr[i],
                    fornavn = fornavn[i],
                    etternavn = etternavn[i],
                    adresse = adresse[i],
                    postNr = postnr[i],
                    poststed = poststed[i],
                    telefonNr = telefonnr[i]
                };
                forventetKundeListe.Add(kunde);
            }

            // vi fikk ikke tid for UT
            //
           
            
        }

    }
}
