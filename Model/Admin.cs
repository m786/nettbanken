using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nettbanken.Models
{
    public class Admin
    {
        public string adminId { get; set; } // BankID for innlogging

        public string passord { get; set; }

        public string fornavn { get; set; }

        public string etternavn { get; set; }

        public string adresse { get; set; }

        public string telefonNr { get; set; }

        public string postNr { get; set; }

        public string poststed { get; set; } // Hver Admin har kun ett postnr og poststed
    }

}
