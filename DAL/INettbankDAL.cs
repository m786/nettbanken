using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nettbanken.Models;

namespace Nettbanken.DAL
{
    public interface INettbankDAL
    {
        // ---------------------------------------------------------------------------------------
        // Admin Metoder

        Boolean adminLogginn(Admin admin);
        List<Kunde> alleKunder();

        Boolean registrerNyKunde(Kunde kunde);
        Boolean endreKunde(string idnr,Kunde innKunde);
        Boolean slettKunde(string personNr);
        Kunde finnKunde(string sok);
        Boolean sjekkSaldo(String personnr);
        String lagPassord();

        // ---------------------------------------------------------------------------------------
        // Kunde Metoder

        Boolean registrerKunde(Kunde kunde);
        Boolean registrerNyKonto(Konto nykonto);
        void opprettStandardkonto(string[] nyKundeInfo);
        Boolean kundeLogginn(Kunde kunde);
        Transaksjon registrerTransaksjon(Transaksjon transaksjon);
        List<String> hentKontoer(String personnr);
      
        String hentKontoInformasjon(String kontonavn, String personnr);
        String hentKontoUtskrift(String kontonavn, String personnr);
        void oppdaterKontoer(String fraKonto, String tilKonto, String belop);
        void startsjekk();
        void startSjekkTransaksjonStatus();

    }
}
