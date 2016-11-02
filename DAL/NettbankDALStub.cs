using System;
using Nettbanken.DAL;
using Nettbanken.Models;
using System.Collections.Generic;

namespace Nettbanken.DAL
{
    public class NettbankDALStub : INettbankDAL
    {

        // ---------------------------------------------------------------------------------------
        // Admin Metoder

        // Admin metode skal inn her

        // ---------------------------------------------------------------------------------------
        // Kunde Metoder

        public Boolean registrerKunde(Kunde kunde)
        {
            return true;
        }

        public Boolean registrerNyKonto(Konto nykonto)
        {
            return true;
        }

        public void opprettStandardkonto(string[] nyKundeInfo)
        {

        }

        public Boolean adminLogginn(Admin admin)
        {
            return true;
        }

        public Boolean kundeLogginn(Kunde kunde)
        {
            return true;
        }

        public Transaksjon registrerTransaksjon(Transaksjon transaksjon)
        {
            return null;
        }

        public List<String> hentKontoer(String personnr)
        {
            return null;
        }

        public String hentKontoInformasjon(String kontonavn, String personnr)
        {
            return "";
        }

        public String hentKontoUtskrift(String kontonavn, String personnr)
        {
            return "";
        }

        public void startsjekk()
        {

        }

    }

}