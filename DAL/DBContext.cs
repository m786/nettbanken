namespace Nettbanken.DAL
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Data.Entity;
    using System.Data.Entity.ModelConfiguration.Conventions;
    using System.Linq;

    public class DBContext : DbContext
    {
        

        public DBContext()
            : base("name=NettbankDB") // Navnet på vår connectionString
        {
            Database.CreateIfNotExists(); // Lager databasen hvis den ikke eksisterer
        }

        // Fjerner automatisk "flertalls-innsetting" på tabellene (Kunder => Kunders)
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AdminDB>().HasKey(a => a.adminId);
            modelBuilder.Entity<KundeDB>().HasKey(k => k.personNr);
            modelBuilder.Entity<KontoDB>().HasKey(ko => ko.kontoNr);
            modelBuilder.Entity<TransaksjonDB>().HasKey(t => t.Id);
            modelBuilder.Entity<PoststedDB>().HasKey(p => p.postNr);
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

        // Metodene som lager tabellene
        public DbSet<AdminDB> Admins { get; set; }
        public DbSet<KundeDB> Kunder { get; set; }
        public DbSet<KontoDB> Kontoer { get; set; }
        public DbSet<TransaksjonDB> Transaksjoner { get; set; }
        public DbSet<PoststedDB> Poststeder { get; set; }
    }

    public class AdminDB
    {
        public string adminId { get; set; } // BankID for innlogging

        public string passord { get; set; }

        public string fornavn { get; set; }

        public string etternavn { get; set; }

        public string adresse { get; set; }

        public string telefonNr { get; set; }

        public string postNr { get; set; }

        public virtual PoststedDB poststed { get; set; } // Hver Admin har kun ett postnr og poststed
    }

    public class KundeDB
    {
        public string bankId { get; set; } // BankID for innlogging

        public string personNr { get; set; }

        public string passord { get; set; }
        
        public string fornavn { get; set; }
        
        public string etternavn { get; set; }

        public string adresse { get; set; }

        public string telefonNr { get; set; }

        public string postNr { get; set; }

        public virtual PoststedDB poststed { get; set; } // Hver kunde har kun et postnr og poststed
    }

    public class KontoDB
    {
        public string kontoNr { get; set; }

        public int saldo { get; set; }

        public string kontoNavn { get; set; }

        public string personNr { get; set; }

        public virtual KundeDB kunde { get; set; } // Hver Konto er knyttet til en Kunde
    }

    // En tabell som vil inneholde alle transaksjoner.
    public class TransaksjonDB
    {
        public int Id { get; set; } 

        public string status { get; set; } // Status, Betalt, Ikke-Betalt, Kansellert osv.

        public int saldoInn { get; set; }

        public int saldoUt { get; set; }
 
        public string dato { get; set; }
   
        public string KID { get; set; }
        
        public string fraKonto { get; set; }

        public string tilKonto { get; set; }
     
        public string melding { get; set; }

        public virtual KontoDB konto { get; set; } // Hver transaksjon vil tilhøre en konto
    }

    public class PoststedDB
    {
        public string postNr { get; set; }

        public string poststed { get; set; }
    }
}