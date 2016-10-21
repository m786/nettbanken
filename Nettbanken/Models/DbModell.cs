namespace Nettbanken.Models
{
    using Controllers;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Data.Entity;
    using System.Data.Entity.ModelConfiguration.Conventions;
    using System.Linq;

    public class DbModell : DbContext
    {
        

        public DbModell()
            : base("name=NettbankDB") // Navnet på vår connectionString
        {
            Database.CreateIfNotExists(); // Lager databasen hvis den ikke eksisterer
        }

        // Fjerner automatisk "flertalls-innsetting" på tabellene (Kunder => Kunders)
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

        // Metodene som lager tabellene
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Kunde> Kunder { get; set; }
        public DbSet<Konto> Kontoer { get; set; }
        public DbSet<Transaksjon> Transaksjoner { get; set; }
        public DbSet<Poststed> Poststeder { get; set; }
    }

    public class Admin
    {
        [Key]
        public string adminId { get; set; } // BankID for innlogging
        public string passord { get; set; }

        public string fornavn { get; set; }
        public string etternavn { get; set; }
        public string adresse { get; set; }
        public string telefonNr { get; set; }
        public string postNr { get; set; }

        public virtual Poststed poststed { get; set; } // Hver Admin har kun ett postnr og poststed
    }

    public class Kunde
    {
        [Display(Name = "BankID")]
        [Required(ErrorMessage ="Vennligst fyll inn bankId")]
        public string bankId { get; set; } // BankID for innlogging

        [Key]
        [Display (Name = "Personnummer")]
        [Required(ErrorMessage = "Vennligst fyll inn personNr")]
        public string personNr { get; set; }
        [Display(Name = "Passord")]
        [Required(ErrorMessage = "Vennligst fyll inn passord")]
        public string passord { get; set; }
        [Display(Name = "Fornavn")]
        [Required(ErrorMessage = "Vennligst fyll inn fornavn")]
        public string fornavn { get; set; }
        [Display(Name = "Etternavn")]
        [Required(ErrorMessage = "Vennligst fyll inn etter navn")]
        public string etternavn { get; set; }
        [Display(Name = "Adresse")]
        [Required(ErrorMessage = "Vennligst fyll inn adresse")]
        public string adresse { get; set; }
        [Display(Name = "Telefonnummer")]
        [Required(ErrorMessage = "Vennligst fyll inn telefonNr")]
        public string telefonNr { get; set; }
        [Display(Name = "Postnummer")]
       // [Required(ErrorMessage = "Vennligst fyll inn postnummer")]
        public string postNr { get; set; }

        [Display(Name = "Poststed")]
       // [Required(ErrorMessage = "Vennligst fyll inn poststed")]
        public virtual Poststed poststed { get; set; } // Hver kunde har kun et postnr og poststed
        public virtual List<Konto> konto { get; set; } // En kunde kan ha flere kontoer, aka List<Konto>

    }

    public class Konto
    {
        [Key]
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


        public virtual Kunde kunde { get; set; } // Hver Konto er knyttet til en Kunde
        public virtual List<Transaksjon> transaksjon { get; set; } // Hver Konto kan ha flerne transaksjoner
    }

    // En tabell som vil inneholde alle transaksjoner.
    public class Transaksjon
    {
        [Key]
        [Display(Name = "Transaksjonsnummer")]
        public int Id { get; set; } 
        [Display(Name = "Status")]
        public string status { get; set; } // Status, Betalt, Ikke-Betalt, Kansellert osv.
        [Display(Name = "Saldo inn")]
        public int saldoInn { get; set; }
        [Display(Name = "Saldo ut")]
        [Required(ErrorMessage = "Vennligst fyll inn mengde")]
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

        public virtual Konto konto { get; set; } // Hver transaksjon vil tilhøre en konto
    }

    public class Poststed
    {
        [Key]
        [Required(ErrorMessage = "Vennligst fyll inn postNr")]
        [Display(Name = "Postnummer")]
        //[Required(ErrorMessage = "Vennligst fyll inn postnummer")]
        public string postNr { get; set; }
        [Display(Name = "Poststed")]
        [Required(ErrorMessage = "Vennligst fyll poststed")]
        // [Required(ErrorMessage = "Vennligst fyll inn poststed")]
        public string poststed { get; set; }

        // Hver poststed kan ha flere kunder/admins knyttet til seg
        public virtual List<Admin> admin { get; set; } 
        public virtual List<Kunde> kunde { get; set; }
    }
}