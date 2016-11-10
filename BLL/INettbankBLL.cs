using Nettbanken.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nettbanken.BLL
{
    public interface INettbankBLL
    {
        // ---------------------------------------------------------------------------------------
        // Admin Metoder

        Boolean adminLogginn(Admin admin);
        List<Kunde> alleKunder();
        Kunde finnKunde(string sok);
        Boolean slettKunde(string personNr);
        String lagPassord();
        Boolean registrerNyKunde(Kunde kunde);
        // ---------------------------------------------------------------------------------------
        // Kunde Metoder

        Boolean registrerKunde(Kunde kunde);
        Boolean registrerNyKonto(Konto nyKonto);
        void opprettStandardkonto(string[] nyKundeinfo);
        Boolean kundeLogginn(Kunde kunde);
        Transaksjon registrerTransaksjon(Transaksjon transaksjon);
        List<String> hentKontoer(String personnr);
        String hentKontoInformasjon(String kontonavn, String personnr);
        String hentKontoUtskrift(String kontonavn, String personnr);
        void oppdaterKontoer(String[] fraKonto, String[] tilKonto, String[] belop);
        void startsjekk();
    }
}
