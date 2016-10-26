using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nettbanken.Models
{
    public class Transaksjon
    {
        [Display(Name = "Transaksjonsnummer")]
        public int Id { get; set; }

        [Display(Name = "Status")]
        public string status { get; set; } // Status, Betalt, Ikke-Betalt, Kansellert osv.

        [Display(Name = "Saldo inn")]
        public int saldoInn { get; set; }

        [Display(Name = "Saldo ut")]
        [Required(ErrorMessage = "Vennligst fyll inn saldo")]
        [RegularExpression(@"[0-9]{1,}", ErrorMessage = "Saldoen må være over eller lik et siffer")]
        public int saldoUt { get; set; }

        [Display(Name = "Dato")]
        [Required(ErrorMessage = "Vennligst fyll inn dato")]
        public string dato { get; set; }

        [Display(Name = "KID")]
        [Required(ErrorMessage = "Vennligst fyll inn KID")]
        public string KID { get; set; }

        [Display(Name = "Fra konto")]
        public string fraKonto { get; set; }

        [Display(Name = "Til konto")]
        [Required(ErrorMessage = "Vennligst fyll inn konto")]
        public string tilKonto { get; set; }

        [Display(Name = "Melding")]
        public string melding { get; set; }
    }

}
