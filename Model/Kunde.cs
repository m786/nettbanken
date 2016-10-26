using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nettbanken.Models
{
    public class Kunde
    {
        [Display(Name = "BankID")]
        [Required(ErrorMessage = "Vennligst fyll inn bankID")]
        public string bankId { get; set; } // BankID for innlogging

        [Display(Name = "Personnummer")]
        [Required(ErrorMessage = "Vennligst fyll inn personnummer")]
        public string personNr { get; set; }

        [Display(Name = "Passord")]
        [Required(ErrorMessage = "Vennligst fyll inn passord")]
        public string passord { get; set; }

        [Display(Name = "Fornavn")]
        [Required(ErrorMessage = "Vennligst fyll inn fornavn")]
        [RegularExpression(@"[A-Z]{2,}", ErrorMessage = "Fornavn kan kun inneholde 2 eller flere bokstaver")]
        public string fornavn { get; set; }

        [Display(Name = "Etternavn")]
        [Required(ErrorMessage = "Vennligst fyll inn etternavn")]
        [RegularExpression(@"[A-Z]{2,}", ErrorMessage = "Etternavn kan kun inneholde 2 eller flere bokstaver")]
        public string etternavn { get; set; }

        [Display(Name = "Adresse")]
        [Required(ErrorMessage = "Vennligst fyll inn adresse")]
        public string adresse { get; set; }

        [Display(Name = "Telefonnummer")]
        [Required(ErrorMessage = "Vennligst fyll inn telefonnummer")]
        public string telefonNr { get; set; }

        [Display(Name = "Postnummer")]
        [Required(ErrorMessage = "Vennligst fyll inn postnummer")]
        [RegularExpression(@"[0-9]{4}", ErrorMessage = "Postnr må være 4 siffer")]
        public string postNr { get; set; }

        [Display(Name = "Poststed")]
        public string poststed { get; set; } // Hver kunde har kun et postnr og poststed

        public List<String> kontoer { get; set; } // En kunde kan ha flere kontoer, aka List<Konto>
    }

}
