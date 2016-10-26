using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nettbanken.Models
{
    public class Poststed
    {
        [Display(Name = "Postnummer")]
        [Required(ErrorMessage = "Vennligst fyll inn postnummer")]
        [RegularExpression(@"[0-9]{4}", ErrorMessage = "Postnr må være 4 siffer")]
        //[Required(ErrorMessage = "Vennligst fyll inn postnummer")]
        public string postNr { get; set; }

        [Display(Name = "Poststed")]
        [Required(ErrorMessage = "Vennligst fyll poststed")]
        // [Required(ErrorMessage = "Vennligst fyll inn poststed")]
        public string poststed { get; set; }

        // Hver poststed kan ha flere kunder/admins knyttet til seg
        public List<string> admins { get; set; }

        public List<string> kunder { get; set; }
    }

}
