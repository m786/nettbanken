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

        public Boolean adminLogginn(Admin admin)
        {
            if (admin.fornavn == null) return false;
            else return true;
        }

        public List<Kunde> alleKunder()
        {
            var allekunder = new List<Kunde>();
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
            }

            return allekunder;
        }

        public bool registrerNyKunde(Kunde kunde)
        {
            throw new NotImplementedException();
        }

        public bool endreKunde(int personNr, Kunde innKunde)
        {
            throw new NotImplementedException();
        }

        public bool slettKunde(string personNr)
        {
            throw new NotImplementedException();
        }

        public Kunde finnKunde(string sok)
        {
            throw new NotImplementedException();
        }

        public bool sjekkSaldo(string personnr)
        {
            throw new NotImplementedException();
        }

        // ---------------------------------------------------------------------------------------
        // Kunde Metoder

        public Boolean registrerKunde(Kunde kunde)
        {
            throw new NotImplementedException();
        }

        public Boolean registrerNyKonto(Konto nykonto)
        {
            throw new NotImplementedException();
        }

        public void opprettStandardkonto(string[] nyKundeInfo)
        {
            throw new NotImplementedException();
        }

        public Boolean kundeLogginn(Kunde kunde)
        {
            throw new NotImplementedException();
        }

        public Transaksjon registrerTransaksjon(Transaksjon transaksjon)
        {
            throw new NotImplementedException();
        }

        public List<String> hentKontoer(String personnr)
        {
            throw new NotImplementedException();
        }

        public String hentKontoInformasjon(String kontonavn, String personnr)
        {
            throw new NotImplementedException();
        }

        public String hentKontoUtskrift(String kontonavn, String personnr)
        {
            throw new NotImplementedException();
        }

        public void oppdaterKontoer(String[] fraKonto, String[] tilKonto, String[] belop)
        {
            throw new NotImplementedException();
        }

        public void startsjekk()
        {
            throw new NotImplementedException();
        }

    }

}