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

        // Innloggingsmetode for admin
        public Boolean adminLogginn(Admin admin)
        {
            return _repository.adminLogginn(admin);
        }

        // Henter alle kunder for admin
        public List<Kunde> alleKunder()
        {
            return _repository.alleKunder();
        }

        public Kunde finnKunde(string sok)
        {
            return _repository.finnKunde(sok);
        }
        public Boolean slettKunde(string personNr)
        {
            return _repository.slettKunde(personNr);
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

        // Metode for å oppdatere kontobalanser etter transaksjoner
        public void oppdaterKontoer(String fraKonto, String tilKonto, String belop)
        {
            _repository.oppdaterKontoer(fraKonto, tilKonto, belop);
        }

        // Startsjekk som sjekker for dummydata
        public void startsjekk()
        {
            _repository.startsjekk();
        }

        //start metoden som sjekker transaksjoner
        public void startSjekkTransaksjonStatus()
        {
            _repository.startSjekkTransaksjonStatus();
        }

    }
}