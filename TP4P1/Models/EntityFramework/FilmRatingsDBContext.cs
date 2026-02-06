using Microsoft.EntityFrameworkCore;
using TP4P1.Models.EntityFramework;

namespace TP4P1.Models.EntityFramework
{
    public class FilmRatingsDBContext : DbContext
    {
        public FilmRatingsDBContext(DbContextOptions<FilmRatingsDBContext> options)
            : base(options) { }

        public DbSet<Film> Films { get; set; } = null!;
        public DbSet<Utilisateur> Utilisateurs { get; set; } = null!;
        public DbSet<Notation> Notations { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("public"); // demandé par le TP :contentReference[oaicite:7]{index=7}

            // Film : index non unique sur titre (comme TP)
            modelBuilder.Entity<Film>()
                .HasIndex(f => f.Titre)
                .HasDatabaseName("ix_flm_titre");

            // Utilisateur : mail unique
            modelBuilder.Entity<Utilisateur>()
                .HasIndex(u => u.Mail)
                .IsUnique()
                .HasDatabaseName("ux_utl_mail");

            // Valeurs par défaut demandées (pays='France', datecreation=now()) :contentReference[oaicite:8]{index=8}
            modelBuilder.Entity<Utilisateur>()
                .Property(u => u.Pays)
                .HasDefaultValue("France");

            modelBuilder.Entity<Utilisateur>()
                .Property(u => u.DateCreation)
                .HasDefaultValueSql("now()");

            // Notation : PK composée + relations + delete restrict :contentReference[oaicite:9]{index=9}
            modelBuilder.Entity<Notation>()
                .HasKey(n => new { n.UtilisateurId, n.FilmId });

            modelBuilder.Entity<Notation>()
                .HasOne(n => n.UtilisateurNotant)
                .WithMany(u => u.NotesUtilisateur)
                .HasForeignKey(n => n.UtilisateurId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_not_utl");

            modelBuilder.Entity<Notation>()
                .HasOne(n => n.FilmNote)
                .WithMany(f => f.NotesFilm)
                .HasForeignKey(n => n.FilmId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_not_flm");
        }
    }
}
