﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nettbanken.Models
{
    // Admin domenemodell, falt admin
    public class Admin
    {
        [Display(Name = "Admin ID")]
        public string adminId { get; set; } // BankID for innlogging

        [Display(Name = "Passord")]
        [Required(ErrorMessage = "Vennligst fyll inn passord")]
        public string passord { get; set; }

        public byte[] salt { get; set; }

        [Display(Name = "Fornavn")]
        [Required(ErrorMessage = "Vennligst fyll inn fornavn")]
        [RegularExpression(@"[A-ZÆØÅa-zæøå]{2,}", ErrorMessage = "Fornavn kan kun inneholde 2 eller flere bokstaver")]
        public string fornavn { get; set; }

        [Display(Name = "Etternavn")]
        [Required(ErrorMessage = "Vennligst fyll inn etternavn")]
        [RegularExpression(@"[A-ZÆØÅa-zæøå]{2,}", ErrorMessage = "Etternavn kan kun inneholde 2 eller flere bokstaver")]
        public string etternavn { get; set; }

        [Display(Name = "Adresse")]
        [Required(ErrorMessage = "Vennligst fyll inn adresse")]
        public string adresse { get; set; }

        [Display(Name = "Telefonnummer")]
        [Required(ErrorMessage = "Vennligst fyll inn telefonnummer")]
        [RegularExpression(@"[0-9]{8}", ErrorMessage = "Telefonnummer må inneholde 8 siffer")]
        public string telefonNr { get; set; }

        [Display(Name = "Postnummer")]
        [Required(ErrorMessage = "Vennligst fyll inn postnummer")]
        [RegularExpression(@"[0-9]{4}", ErrorMessage = "Postnummer må være 4 siffer")]
        public string postNr { get; set; }

        [Display(Name = "Poststed")]
        [Required(ErrorMessage = "Vennligst fyll inn poststedsnavn")]
        [RegularExpression(@"[A-ZÆØÅa-zæøå]{2,}", ErrorMessage = "Poststedsnavn kan kun inneholde bokstaver")]
        public string poststed { get; set; }
    }

}
