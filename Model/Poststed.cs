using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nettbanken.Models
{
    // Poststed domenemodell, flat poststed
    public class Poststed
    {
        [Display(Name = "Postnummer")]
        [Required(ErrorMessage = "Vennligst fyll inn postnummer")]
        [RegularExpression(@"[0-9]{4}", ErrorMessage = "Postnummer må være 4 siffer")]
        public string postNr { get; set; }

        [Display(Name = "Poststed")]
        [Required(ErrorMessage = "Vennligst fyll inn poststedsnavn")]
        [RegularExpression(@"[A-ZÆØÅa-zæøå]{2,}", ErrorMessage = "Poststedsnavn kan kun inneholde bokstaver")]
        public string poststed { get; set; }

        // Hver poststed kan ha flere kunder/admins knyttet til seg
        //public List<string> admins { get; set; }

        //public List<string> kunder { get; set; }
    }

}
