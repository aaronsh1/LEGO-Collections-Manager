using System;
using System.Configuration;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

#nullable disable

namespace LegoCollectionManager.Models
{
    public partial class LegoCollectionDBContext : DbContext
    {
        public LegoCollectionDBContext()
        {
        }

        public LegoCollectionDBContext(DbContextOptions<LegoCollectionDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Avatar> Avatars { get; set; }
        public virtual DbSet<Colour> Colours { get; set; }
        public virtual DbSet<MissingPiece> MissingPieces { get; set; }
        public virtual DbSet<Piece> Pieces { get; set; }
        public virtual DbSet<PieceCategory> PieceCategories { get; set; }
        public virtual DbSet<Set> Sets { get; set; }
        public virtual DbSet<SetCategory> SetCategories { get; set; }
        public virtual DbSet<SetPiece> SetPieces { get; set; }
        public virtual DbSet<SetPieceCategory> SetPieceCategories { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserAvatar> UserAvatars { get; set; }
        public virtual DbSet<UserSet> UserSets { get; set; }
        public virtual DbSet<UserSparePiece> UserSparePieces { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                IConfiguration _configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();
                string myDbConnection = _configuration.GetConnectionString("DefaultConnection");
                optionsBuilder.UseSqlServer(myDbConnection);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Avatar>(entity =>
            {
                entity.ToTable("Avatar");

                entity.Property(e => e.Url)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("URL");
            });

            modelBuilder.Entity<Colour>(entity =>
            {
                entity.ToTable("Colour");

                entity.Property(e => e.ColourId).ValueGeneratedNever();

                entity.Property(e => e.ColourName)
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<MissingPiece>(entity =>
            {
                entity.ToTable("MissingPiece");

                entity.Property(e => e.Piece)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.ColourNavigation)
                    .WithMany(p => p.MissingPieces)
                    .HasForeignKey(d => d.Colour)
                    .HasConstraintName("FK__MissingPi__Colou__45F365D3");

                entity.HasOne(d => d.PieceNavigation)
                    .WithMany(p => p.MissingPieces)
                    .HasForeignKey(d => d.Piece)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__MissingPi__Piece__44FF419A");

                entity.HasOne(d => d.UserSetNavigation)
                    .WithMany(p => p.MissingPieces)
                    .HasForeignKey(d => d.UserSet)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__MissingPi__UserS__440B1D61");
            });

            modelBuilder.Entity<Piece>(entity =>
            {
                entity.ToTable("Piece");

                entity.Property(e => e.PieceId)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.PieceName)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.HasOne(d => d.PieceCategoryNavigation)
                    .WithMany(p => p.Pieces)
                    .HasForeignKey(d => d.PieceCategory)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__Piece__PieceCate__300424B4");
            });

            modelBuilder.Entity<PieceCategory>(entity =>
            {
                entity.ToTable("PieceCategory");

                entity.Property(e => e.PieceCategoryId).ValueGeneratedNever();

                entity.Property(e => e.PieceCategoryName)
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Set>(entity =>
            {
                entity.ToTable("Set");

                entity.Property(e => e.SetId).ValueGeneratedNever();

                entity.Property(e => e.SetName)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.HasOne(d => d.SetCategoryNavigation)
                    .WithMany(p => p.Sets)
                    .HasForeignKey(d => d.SetCategory)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__Set__SetCategory__34C8D9D1");
            });

            modelBuilder.Entity<SetCategory>(entity =>
            {
                entity.ToTable("SetCategory");

                entity.Property(e => e.SetCategoryId).ValueGeneratedNever();

                entity.Property(e => e.SetCategoryName)
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<SetPiece>(entity =>
            {
                entity.ToTable("SetPiece");

                entity.Property(e => e.Piece)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.ColourNavigation)
                    .WithMany(p => p.SetPieces)
                    .HasForeignKey(d => d.Colour)
                    .HasConstraintName("FK__SetPiece__Colour__398D8EEE");

                entity.HasOne(d => d.PieceNavigation)
                    .WithMany(p => p.SetPieces)
                    .HasForeignKey(d => d.Piece)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__SetPiece__Piece__37A5467C");

                entity.HasOne(d => d.Set)
                    .WithMany(p => p.SetPieces)
                    .HasForeignKey(d => d.SetId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__SetPiece__SetId__38996AB5");
            });

            modelBuilder.Entity<SetPieceCategory>(entity =>
            {
                entity.ToTable("SetPieceCategory");

                entity.HasOne(d => d.PieceCategoryNavigation)
                    .WithMany(p => p.SetPieceCategories)
                    .HasForeignKey(d => d.PieceCategory)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__SetPieceC__Piece__48CFD27E");

                entity.HasOne(d => d.SetNavigation)
                    .WithMany(p => p.SetPieceCategories)
                    .HasForeignKey(d => d.Set)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__SetPieceCat__Set__49C3F6B7");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");

                entity.Property(e => e.Password)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Salt)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Username)
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<UserAvatar>(entity =>
            {
                entity.ToTable("UserAvatar");

                entity.HasOne(d => d.AvatarNavigation)
                    .WithMany(p => p.UserAvatars)
                    .HasForeignKey(d => d.Avatar)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__UserAvata__Avata__2B3F6F97");

                entity.HasOne(d => d.UserNavigation)
                    .WithMany(p => p.UserAvatars)
                    .HasForeignKey(d => d.User)
                    .HasConstraintName("FK__UserAvatar__User__2A4B4B5E");
            });

            modelBuilder.Entity<UserSet>(entity =>
            {
                entity.HasKey(e => e.UseSetId)
                    .HasName("PK__UserSet__85C9DDD61956C09B");

                entity.ToTable("UserSet");

                entity.HasOne(d => d.SetNavigation)
                    .WithMany(p => p.UserSets)
                    .HasForeignKey(d => d.Set)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__UserSet__Set__412EB0B6");

                entity.HasOne(d => d.UserNavigation)
                    .WithMany(p => p.UserSets)
                    .HasForeignKey(d => d.User)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__UserSet__User__403A8C7D");
            });

            modelBuilder.Entity<UserSparePiece>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.Piece)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.ColourNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.Colour)
                    .HasConstraintName("FK__UserSpare__Colou__3D5E1FD2");

                entity.HasOne(d => d.PieceNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.Piece)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__UserSpare__Piece__3C69FB99");

                entity.HasOne(d => d.UserNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.User)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__UserSpareP__User__3B75D760");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
