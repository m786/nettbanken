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

        private INettbankDAL _repository;

        public NettbankBLL()
        {
            _repository = new NettbankDAL();
        }

        public NettbankBLL(INettbankDAL stub)
        {
            _repository = stub;
        }

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
            return _repository.registrerKunde(kunde);
        }

        //Konto registrering: opprette konto for kunder samtidig som di registreres!
        public Boolean registrerNyKonto(Konto nyKonto)
        {
            return _repository.registrerNyKonto(nyKonto);
        }

        // Oppretter ny konto ved ny kunde
        public void opprettStandardkonto(string[] nyKundeinfo)
        {
            _repository.opprettStandardkonto(nyKundeinfo);
        }

        // Innloggingsmetode for admin
        public Boolean adminLogginn(Admin admin)
        {
            return _repository.adminLogginn(admin);
        }

        // Innloggingsmetode for kunder
        public Boolean kundeLogginn(Kunde kunde)
        {
            return _repository.kundeLogginn(kunde);
        }

        // Registrerer en transaksjon
        public Transaksjon registrerTransaksjon(Transaksjon transaksjon)
        {
            return _repository.registrerTransaksjon(transaksjon);
        }

        // Henter alle kontoer som tilhører gitt personnr
        public List<String> hentKontoer(String personnr)
        {
            var kontoer = _repository.hentKontoer(personnr);

            return kontoer;
        }

        // Meetode som lager tabellen for konto informasjon
        public String hentKontoInformasjon(String kontonavn, String personnr)
        {
            String kontoInformasjon = _repository.hentKontoInformasjon(kontonavn, personnr);
 
            return kontoInformasjon;
        }

        // Metode som lager tabell for kontoutskrifter
        public String hentKontoUtskrift(String kontonavn, String personnr)
        {
            String kontoUtskrift = _repository.hentKontoUtskrift(kontonavn, personnr);

            return kontoUtskrift;
        }

        // Startsjekk som sjekker for dummydata
        public void startsjekk()
        {
            _repository.startsjekk();
        }
        
    }
}