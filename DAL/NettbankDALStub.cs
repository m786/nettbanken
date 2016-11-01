using System;
using Nettbanken.DAL;
using Nettbanken.Models;

namespace DAL
{
    public class NettbankDALStub : DBContext
    {

        public String krypterPassord(String passord)
        {
            if (passord == "" || passord.Length < 7)
            {
                return "";
            }

            return "ok";
        }

        public String registrerKunde(Kunde kunde)
        {

            if (kunde.fornavn == "" || kunde.etternavn == "" || kunde.adresse == "" || kunde.personNr.Length > 11 || kunde.postNr.Length > 4 || kunde.telefonNr.Length > 8 || kunde.poststed == "" || kunde.bankId == "" || kunde.passord.Length < 8)
            {
                return "";
            }
            return "ok";
        }

        public bool registrerNyKonto(Konto nykonto)
        {
            //hva bør testes her?hva kan gå galt ?
            return true;
        }
    }

}