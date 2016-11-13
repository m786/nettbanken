using System;
using Nettbanken.DAL;
using Nettbanken.Models;
using System.Collections.Generic;

namespace Nettbanken.DAL
{
    public class NettbankDALStub : INettbankDAL
    {
        List<string[]> listeAvKunder;
        List<byte[]> ph;
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

        //kunder registrerers i minne..
        public bool registrerNyKunde(Kunde kunde)
        {
            if (listeAvKunder==null) {
                listeAvKunder = new List<string[]>();
                ph = new List<byte[]>();
            }
            
            string personNr, bankId, passord,fornavn, etternavn, addresse, telefonNr, postNr;
            byte[] salt;

            personNr = kunde.postNr;
            bankId = kunde.bankId;
            passord = kunde.passord;
            salt = kunde.salt;
            fornavn = kunde.fornavn;
            etternavn = kunde.etternavn;
            addresse = kunde.adresse;
            telefonNr = kunde.telefonNr;
            postNr = kunde.postNr;

            string[] enKunde = { personNr,bankId,passord,fornavn,etternavn,addresse,telefonNr,postNr};

            if (kunde == null)
            {
                return false;
            }
            else {
                listeAvKunder.Add(enKunde);
                ph.Add(salt);
            }
            return true;
        }

        public bool endreKunde(string idnr, Kunde innKunde)
        {
            string[] kundenSomSkalEndres = null;

            if (listeAvKunder != null)
            {
                for (int i = 0; i < listeAvKunder.Count; i++)
                {
                    string[] enKunde = listeAvKunder[i];
                    if (enKunde[0].Equals(idnr))
                    {
                        kundenSomSkalEndres = enKunde;
                        ph[i] = innKunde.salt;
                        break;
                    }
                }
                //innholdet til arrayen som inneholder kunden med gitt id oppdateres
                kundenSomSkalEndres[0] = innKunde.personNr;
                kundenSomSkalEndres[1] = innKunde.bankId;
                kundenSomSkalEndres[2] = innKunde.passord;
                kundenSomSkalEndres[3] = innKunde.fornavn;
                kundenSomSkalEndres[4] = innKunde.etternavn;
                kundenSomSkalEndres[5] = innKunde.adresse;
                kundenSomSkalEndres[6] = innKunde.telefonNr;
                kundenSomSkalEndres[7] = innKunde.postNr;
                return true;
            }
            else {
                return false;
            }
            
        }

        //slett en kunde
        public bool slettKunde(string personNr)
        {
            string[] kundenSomSkalSlettes = null;

            if (listeAvKunder != null)
            {
                for (int i = 0; i < listeAvKunder.Count; i++)
                {
                    string[] enKunde = listeAvKunder[i];
                    if (enKunde[0].Equals(personNr))
                    {
                        kundenSomSkalSlettes = enKunde;
                        ph.RemoveAt(i);
                        break;
                    }
                }
                listeAvKunder.Remove(kundenSomSkalSlettes);
                return true;
            }
            else
            {
                return false;
            }
        }

        public Kunde finnKunde(string sok)
        {
            Kunde funnet = new Kunde();
            string[] kundenFunnet = null;

            if (listeAvKunder != null)
            {
                for (int i = 0; i < listeAvKunder.Count; i++)
                {
                    string[] enKunde = listeAvKunder[i];
                    if (enKunde[0].Equals(sok))
                    {
                        kundenFunnet = enKunde;
                        funnet.salt = ph[i];
                        break;
                    }
                } 
                funnet.personNr = kundenFunnet[0];
                funnet.bankId = kundenFunnet[1];
                funnet.passord = kundenFunnet[2];
                funnet.personNr = kundenFunnet[3];
                funnet.personNr = kundenFunnet[4];
                funnet.personNr = kundenFunnet[5];
                funnet.personNr = kundenFunnet[6];
                funnet.personNr = kundenFunnet[7];
                return funnet;
            }
            else
            {
                return null; 
            }

        }

        public String lagPassord()
        {
            string velgFra = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789!@$?_-";
            char[] bokstaver = new char[9];
            Random tilfeldig = new Random();

            for (int i = 0; i < 9; i++)
            {
                bokstaver[i] = velgFra[tilfeldig.Next(0, velgFra.Length)];
            }

            return new string(bokstaver);
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

        public void oppdaterKontoer(String fraKonto, String tilKonto, String belop)
        {
            throw new NotImplementedException();
        }

        public void startsjekk()
        {
            throw new NotImplementedException();
        }

        public void startSjekkTransaksjonStatus()
        {
            throw new NotImplementedException();
        }

    }
}