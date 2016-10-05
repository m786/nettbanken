using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nettbanken.Models
{
    public class DBMetoder
    {
        public static String skrivInnKunde()
        {

            using (var db = new DbModell())
            {
                var kunde = new Kunde
                {
                    bankId = "236777",
                    personNr = "061195",
                    passord = "testbruker",
                    fornavn = "Erik",
                    etternavn = "Li",
                    adresse = "askerveien 32",
                    telefonNr = "123456789",
                    postNr = "123"
                };

                var poststed = new Poststed
                {
                    postNr = "123",
                    poststed = "Oslo"
                };

                kunde.poststed = poststed;

                try
                {
                    db.Kunder.Add(kunde);
                    db.SaveChanges();
                    return "Kunde har blitt lagt til!";
                }
                catch (Exception feil)
                {
                    return "Feil i innsetting" + feil.InnerException + " - " +feil.Source;
                }

            }
        }
    }
}