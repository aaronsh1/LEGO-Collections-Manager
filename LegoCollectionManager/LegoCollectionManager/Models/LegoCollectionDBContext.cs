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

        public virtual DbSet<Colour> Colours { get; set; }
        public virtual DbSet<MissingPiece> MissingPieces { get; set; }
        public virtual DbSet<Piece> Pieces { get; set; }
        public virtual DbSet<PieceCategory> PieceCategories { get; set; }
        public virtual DbSet<Set> Sets { get; set; }
        public virtual DbSet<SetCategory> SetCategories { get; set; }
        public virtual DbSet<SetPiece> SetPieces { get; set; }
        public virtual DbSet<SetPieceCategory> SetPieceCategories { get; set; }
        public virtual DbSet<SubstitutePool> SubstitutePools { get; set; }
        public virtual DbSet<SubstitutePoolItem> SubstitutePoolItems { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserSet> UserSets { get; set; }
        public virtual DbSet<UserSparePiece> UserSparePieces { get; set; }
        public virtual DbSet<UserSubsititutePool> UserSubsititutePools { get; set; }

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

            modelBuilder.Entity<Colour>(entity =>
            {
                entity.ToTable("Colour");

                entity.Property(e => e.ColourName)
                    .HasMaxLength(1)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<MissingPiece>(entity =>
            {
                entity.ToTable("MissingPiece");

                entity.HasOne(d => d.ColourNavigation)
                    .WithMany(p => p.MissingPieces)
                    .HasForeignKey(d => d.Colour)
                    .HasConstraintName("FK__MissingPi__Colou__5AEE82B9");

                entity.HasOne(d => d.PieceNavigation)
                    .WithMany(p => p.MissingPieces)
                    .HasForeignKey(d => d.Piece)
                    .HasConstraintName("FK__MissingPi__Piece__5812160E");

                entity.HasOne(d => d.UserSetNavigation)
                    .WithMany(p => p.MissingPieces)
                    .HasForeignKey(d => d.UserSet)
                    .HasConstraintName("FK__MissingPi__UserS__571DF1D5");
            });

            modelBuilder.Entity<Piece>(entity =>
            {
                entity.ToTable("Piece");

                entity.Property(e => e.PieceImage)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.PieceName)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.HasOne(d => d.PieceCategoryNavigation)
                    .WithMany(p => p.Pieces)
                    .HasForeignKey(d => d.PieceCategory)
                    .HasConstraintName("FK__Piece__PieceCate__4E88ABD4");
            });

            modelBuilder.Entity<PieceCategory>(entity =>
            {
                entity.ToTable("PieceCategory");

                entity.Property(e => e.PieceCategoryName)
                    .HasMaxLength(1)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Set>(entity =>
            {
                entity.ToTable("Set");

                entity.Property(e => e.Image)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.SetName)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.ShopLink)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.HasOne(d => d.SetCategoryNavigation)
                    .WithMany(p => p.Sets)
                    .HasForeignKey(d => d.SetCategory)
                    .HasConstraintName("FK__Set__SetCategory__4D94879B");
            });

            modelBuilder.Entity<SetCategory>(entity =>
            {
                entity.ToTable("SetCategory");

                entity.Property(e => e.SetCategoryName)
                    .HasMaxLength(1)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<SetPiece>(entity =>
            {
                entity.ToTable("SetPiece");

                entity.Property(e => e.SetPieceId).ValueGeneratedOnAdd();

                entity.HasOne(d => d.ColourNavigation)
                    .WithMany(p => p.SetPieces)
                    .HasForeignKey(d => d.Colour)
                    .HasConstraintName("FK__SetPiece__Colour__59063A47");

                entity.HasOne(d => d.Set)
                    .WithMany(p => p.SetPieces)
                    .HasForeignKey(d => d.SetId)
                    .HasConstraintName("FK__SetPiece__SetId__5070F446");

                entity.HasOne(d => d.SetPieceNavigation)
                    .WithOne(p => p.SetPiece)
                    .HasForeignKey<SetPiece>(d => d.SetPieceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__SetPiece__SetPie__4F7CD00D");
            });

            modelBuilder.Entity<SetPieceCategory>(entity =>
            {
                entity.ToTable("SetPieceCategory");

                entity.HasOne(d => d.PieceCategoryNavigation)
                    .WithMany(p => p.SetPieceCategories)
                    .HasForeignKey(d => d.PieceCategory)
                    .HasConstraintName("FK__SetPieceC__Piece__5535A963");

                entity.HasOne(d => d.SetNavigation)
                    .WithMany(p => p.SetPieceCategories)
                    .HasForeignKey(d => d.Set)
                    .HasConstraintName("FK__SetPieceCat__Set__5629CD9C");
            });

            modelBuilder.Entity<SubstitutePool>(entity =>
            {
                entity.HasKey(e => e.SubstitutePool1)
                    .HasName("PK__Substitu__77049D978B644CD4");

                entity.ToTable("SubstitutePool");

                entity.Property(e => e.SubstitutePool1).HasColumnName("SubstitutePool");

                entity.Property(e => e.Name)
                    .HasMaxLength(1)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<SubstitutePoolItem>(entity =>
            {
                entity.ToTable("SubstitutePoolItem");

                entity.HasOne(d => d.PieceNavigation)
                    .WithMany(p => p.SubstitutePoolItems)
                    .HasForeignKey(d => d.Piece)
                    .HasConstraintName("FK__Substitut__Piece__5CD6CB2B");

                entity.HasOne(d => d.SubstitutePoolNavigation)
                    .WithMany(p => p.SubstitutePoolItems)
                    .HasForeignKey(d => d.SubstitutePool)
                    .HasConstraintName("FK__Substitut__Subst__5BE2A6F2");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");

                entity.Property(e => e.Username)
                    .HasMaxLength(1)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<UserSet>(entity =>
            {
                entity.HasKey(e => e.UseSetId)
                    .HasName("PK__UserSet__85C9DDD6E01DB285");

                entity.ToTable("UserSet");

                entity.HasOne(d => d.SetNavigation)
                    .WithMany(p => p.UserSets)
                    .HasForeignKey(d => d.Set)
                    .HasConstraintName("FK__UserSet__Set__5441852A");

                entity.HasOne(d => d.UserNavigation)
                    .WithMany(p => p.UserSets)
                    .HasForeignKey(d => d.User)
                    .HasConstraintName("FK__UserSet__User__534D60F1");
            });

            modelBuilder.Entity<UserSparePiece>(entity =>
            {
                entity.HasNoKey();

                entity.HasOne(d => d.ColourNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.Colour)
                    .HasConstraintName("FK__UserSpare__Colou__59FA5E80");

                entity.HasOne(d => d.PieceNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.Piece)
                    .HasConstraintName("FK__UserSpare__Piece__52593CB8");

                entity.HasOne(d => d.UserNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.User)
                    .HasConstraintName("FK__UserSpareP__User__5165187F");
            });

            modelBuilder.Entity<UserSubsititutePool>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("UserSubsititutePool");

                entity.HasOne(d => d.SubstitutePoolNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.SubstitutePool)
                    .HasConstraintName("FK__UserSubsi__Subst__5DCAEF64");

                entity.HasOne(d => d.UserNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.User)
                    .HasConstraintName("FK__UserSubsit__User__5EBF139D");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
