using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace Server
{
    public partial class PlayDBContext : DbContext
    {
        public PlayDBContext()
        {
        }

        public PlayDBContext(DbContextOptions<PlayDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<Administrator> Administrators { get; set; }
        public virtual DbSet<Play> Plays { get; set; }
        public virtual DbSet<Spectator> Spectators { get; set; }
        public virtual DbSet<Ticket> Tickets { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            { 
                optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=PlayDB;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Account>(entity =>
            {
                entity.ToTable("Account");

                entity.HasIndex(e => e.Email, "UQ__Account__A9D10534AE756BD2")
                    .IsUnique();

                entity.HasIndex(e => e.Nickname, "UQ__Account__CC6CD17EC2222FF3")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Nickname)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Parol)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Administrator>(entity =>
            {
                entity.ToTable("Administrator");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");

                entity.HasOne(d => d.IdNavigation)
                    .WithOne(p => p.Administrator)
                    .HasForeignKey<Administrator>(d => d.Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Administrato__ID__44FF419A");
            });

            modelBuilder.Entity<Play>(entity =>
            {
                entity.ToTable("Play");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.EndDate)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.EndTime)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.PlayName)
                    .HasMaxLength(110)
                    .IsUnicode(false);

                entity.Property(e => e.StartDate)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.StartTime)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Spectator>(entity =>
            {
                entity.ToTable("Spectator");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");

                entity.Property(e => e.FullName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdNavigation)
                    .WithOne(p => p.Spectator)
                    .HasForeignKey<Spectator>(d => d.Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Spectator__ID__3E52440B");
            });

            modelBuilder.Entity<Ticket>(entity =>
            {
                entity.ToTable("Ticket");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Price).HasColumnType("money");

                entity.HasOne(d => d.ClientNavigation)
                    .WithMany(p => p.Tickets)
                    .HasForeignKey(d => d.Client)
                    .HasConstraintName("FK__Ticket__Client__4222D4EF");

                entity.HasOne(d => d.PlayNavigation)
                    .WithMany(p => p.Tickets)
                    .HasForeignKey(d => d.Play)
                    .HasConstraintName("FK__Ticket__Play__412EB0B6");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
