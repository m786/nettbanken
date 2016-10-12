namespace Nettbanken.Models
{
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
        public string bankId { get; set; } // BankID for innlogging

        [Key]
        [Display (Name = "Personnummer")]
        public string personNr { get; set; }
        [Display(Name = "Passord")]
        public string passord { get; set; }

        [Display(Name = "Fornavn")]
        public string fornavn { get; set; }
        [Display(Name = "Etternavn")]
        public string etternavn { get; set; }
        [Display(Name = "Adresse")]
        public string adresse { get; set; }
        [Display(Name = "Telefonnummer")]
        public string telefonNr { get; set; }
        [Display(Name = "Postnummer")]
        public string postNr { get; set; }

        public virtual Poststed poststed { get; set; } // Hver kunde har kun et postnr og poststed
        public virtual List<Konto> konto { get; set; } // En kunde kan ha flere kontoer, aka List<Konto>

    }

    public class Konto
    {
        [Key]
        public string kontoNr { get; set; }
        public int saldo { get; set; }
        public string bankId { get; set; }


        public virtual Kunde kunde { get; set; } // Hver Konto er knyttet til en Kunde
        public virtual List<Transaksjon> transaksjon { get; set; } // Hver Konto kan ha flerne transaksjoner
    }

    // En tabell som vil inneholde alle transaksjoner.
    public class Transaksjon
    {
        [Key]
        public int Id { get; set; } 
        public string status { get; set; } // Status, Betalt, Ikke-Betalt, Kansellert osv.
        public int saldoInn { get; set; }
        public int saldout { get; set; }
        public string dato { get; set; }
        public string KID { get; set; }
        public string fraKonto { get; set; }
        public string tilKonto { get; set; }
        public string melding { get; set; }

        public virtual Konto konto { get; set; } // Hver transaksjon vil tilhøre en konto
    }

    public class Poststed
    {
        [Key]
        [Display(Name = "Postnummer")]
        public string postNr { get; set; }
        [Display(Name = "Poststed")]
        public string poststed { get; set; }

        // Hver poststed kan ha flere kunder/admins knyttet til seg
        public virtual List<Admin> admin { get; set; } 
        public virtual List<Kunde> kunde { get; set; }
    }
}