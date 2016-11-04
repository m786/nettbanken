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

        // Admin metode skal inn her
        List<Kunde> alleKunder();
        // ---------------------------------------------------------------------------------------
        // Kunde Metoder

        Boolean registrerKunde(Kunde kunde);
        Boolean registrerNyKonto(Konto nykonto);
        void opprettStandardkonto(string[] nyKundeInfo);
        Boolean adminLogginn(Admin admin);
        Boolean kundeLogginn(Kunde kunde);
        Transaksjon registrerTransaksjon(Transaksjon transaksjon);
        List<String> hentKontoer(String personnr);
        List<Kunde> alleKunder();
        String hentKontoInformasjon(String kontonavn, String personnr);
        String hentKontoUtskrift(String kontonavn, String personnr);
        void startsjekk();

    }
}
