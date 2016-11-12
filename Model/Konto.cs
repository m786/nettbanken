using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nettbanken.Models
{
    // Konto domenemodell, flat konto
    public class Konto
    {
        [Display(Name = "Kontonummer")]
        [Required(ErrorMessage = "Vennligst fyll inn kontonummer")]
        public string kontoNr { get; set; }

        [Display(Name = "Saldo")]
        public int saldo { get; set; }

        [Display(Name = "Kontonavn")]
        public string kontoNavn { get; set; }

        [Display(Name = "Personnummer")]
        [Required(ErrorMessage = "Vennligst fyll inn personnummer")]
        public string personNr { get; set; }
    }

}
