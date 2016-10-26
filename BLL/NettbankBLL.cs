using System;
using System.Collections.Generic;
using Nettbanken.DAL;
using Nettbanken.Models;

namespace Nettbanken.BLL
{
    public class NettbankBLL
    {
        private static int bankId = 0;

        // Metode for kryptering av passord
        public String krypterPassord(String passord)
        {
            var NettbankDAL = new NettbankDAL();
            String utPassord = NettbankDAL.krypterPassord(passord);

            return utPassord;
        }


        // Registrering av kunde. Tar et Kunde objekt direkte dra Html.beginForm()
        public String registrerKunde(Kunde kunde)
        {
            var nettbankDAL = new NettbankDAL();
            String OK = nettbankDAL.registrerKunde(kunde);

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
            var NettbankDAL = new NettbankDAL();
            NettbankDAL.opprettStandardkonto(nyKundeinfo);
        }

        // Innloggingsmetode for kunder
        public Boolean kundeLogginn(Kunde kunde)
        {
            var NettbankDAL = new NettbankDAL();

            return NettbankDAL.kundeLogginn(kunde);
        }

        public Transaksjon registrerTransaksjon(Transaksjon transaksjon)
        {
            var NettbankDAL = new NettbankDAL();

            return NettbankDAL.registrerTransaksjon(transaksjon);
        }

        // Henter alle kontoer som tilhører gitt personnr
        public List<String> hentKontoer(String personnr)
        {
            var NettbankDAL = new NettbankDAL();
            var kontoer = NettbankDAL.hentKontoer(personnr);

            return kontoer;
        }

        // Meetode som lager tabellen for konto informasjon
        public String hentKontoInformasjon(String kontonavn, String personnr)
        {
            var NettbankDAL = new NettbankDAL();
            String kontoInformasjon = NettbankDAL.hentKontoInformasjon(kontonavn, personnr);
 
            return kontoInformasjon;
        }

        // Metode som lager tabell for kontoutskrifter
        public String hentKontoUtskrift(String kontonavn, String personnr)
        {
            var NettbankDAL = new NettbankDAL();
            String kontoUtskrift = NettbankDAL.hentKontoUtskrift(kontonavn, personnr);

            return kontoUtskrift;
        }

        public void startsjekk()
        {
            var nettbankDAL = new NettbankDAL();
            nettbankDAL.startsjekk();
        }
        
    }
}