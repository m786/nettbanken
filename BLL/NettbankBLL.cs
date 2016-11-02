using System;
using System.Collections.Generic;
using Nettbanken.DAL;
using Nettbanken.Models;

namespace Nettbanken.BLL
{
    // Logikk laget som blir brukt til å aksessere DAL for databasemetoder
    public class NettbankBLL : INettbankBLL
    {
        //private static int bankId = 0;

        // ---------------------------------------------------------------------------------------
        // Admin Metoder

        public bool interfaceTEST()
        {
            return true;
        }


        // ---------------------------------------------------------------------------------------
        // Kunde Metoder

        // Registrering av kunde. Tar et Kunde objekt direkte dra Html.beginForm()
        public Boolean registrerKunde(Kunde kunde)
        {
            var nettbankDAL = new NettbankDAL();
            Boolean OK = nettbankDAL.registrerKunde(kunde);

            return OK;
        }

        //Konto registrering: opprette konto for kunder samtidig som di registreres!
        public bool registrerNyKonto(Konto nyKonto)
        {
            var nettbankDAL = new NettbankDAL();

            return nettbankDAL.registrerNyKonto(nyKonto);
        }

        // Oppretter ny konto ved ny kunde
        public void opprettStandardkonto(string[] nyKundeinfo)
        {
            var nettbankDAL = new NettbankDAL();
            nettbankDAL.opprettStandardkonto(nyKundeinfo);
        }

        // Innloggingsmetode for admin
        public Boolean adminLogginn(Admin admin)
        {
            var nettbankDAL = new NettbankDAL();

            return nettbankDAL.adminLogginn(admin);
        }

        // Innloggingsmetode for kunder
        public Boolean kundeLogginn(Kunde kunde)
        {
            var nettbankDAL = new NettbankDAL();

            return nettbankDAL.kundeLogginn(kunde);
        }

        // Registrerer en transaksjon
        public Transaksjon registrerTransaksjon(Transaksjon transaksjon)
        {
            var nettbankDAL = new NettbankDAL();

            return nettbankDAL.registrerTransaksjon(transaksjon);
        }

        // Henter alle kontoer som tilhører gitt personnr
        public List<String> hentKontoer(String personnr)
        {
            var nettbankDAL = new NettbankDAL();
            var kontoer = nettbankDAL.hentKontoer(personnr);

            return kontoer;
        }

        // Meetode som lager tabellen for konto informasjon
        public String hentKontoInformasjon(String kontonavn, String personnr)
        {
            var nettbankDAL = new NettbankDAL();
            String kontoInformasjon = nettbankDAL.hentKontoInformasjon(kontonavn, personnr);
 
            return kontoInformasjon;
        }

        // Metode som lager tabell for kontoutskrifter
        public String hentKontoUtskrift(String kontonavn, String personnr)
        {
            var nettbankDAL = new NettbankDAL();
            String kontoUtskrift = nettbankDAL.hentKontoUtskrift(kontonavn, personnr);

            return kontoUtskrift;
        }

        // Startsjekk som sjekker for dummydata
        public void startsjekk()
        {
            var nettbankDAL = new NettbankDAL();
            nettbankDAL.startsjekk();
        }
        
    }
}