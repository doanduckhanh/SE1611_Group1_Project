using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace SE1611_Group1_Project.Models
{
    public partial class FoodOrderContext : DbContext
    {
        public FoodOrderContext()
        {
        }

        public FoodOrderContext(DbContextOptions<FoodOrderContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Cart> Carts { get; set; } = null!;
        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<Food> Foods { get; set; } = null!;
        public virtual DbSet<Order> Orders { get; set; } = null!;
        public virtual DbSet<OrderDetail> OrderDetails { get; set; } = null!;
        public virtual DbSet<Promo> Promos { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
                optionsBuilder.UseSqlServer(config.GetConnectionString("ConnectionStrings"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cart>(entity =>
            {
                entity.HasKey(e => e.RecordId)
                    .HasName("PK__Carts__603930688B159E9D");

                entity.Property(e => e.RecordId).HasColumnName("Record_id");

                entity.Property(e => e.CartId)
                    .HasMaxLength(160)
                    .IsUnicode(false)
                    .HasColumnName("Cart_id");

                entity.Property(e => e.DateCreated).HasColumnType("datetime");

                entity.Property(e => e.FoodId).HasColumnName("Food_id");

                entity.HasOne(d => d.CartNavigation)
                    .WithMany(p => p.Carts)
                    .HasPrincipalKey(p => p.UserName)
                    .HasForeignKey(d => d.CartId)
                    .HasConstraintName("FK__Carts__Cartid__Username");

                entity.HasOne(d => d.Food)
                    .WithMany(p => p.Carts)
                    .HasForeignKey(d => d.FoodId)
                    .HasConstraintName("FK__Carts__Food_id__32E0915F");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Category");

                entity.Property(e => e.CategoryId).HasColumnName("Category_id");

                entity.Property(e => e.CategoryDescription)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("Category_description");

                entity.Property(e => e.CategoryName)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("Category_name");
            });

            modelBuilder.Entity<Food>(entity =>
            {
                entity.Property(e => e.FoodId).HasColumnName("Food_id");

                entity.Property(e => e.CategoryId).HasColumnName("Category_id");

                entity.Property(e => e.FoodImage)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("Food_image");

                entity.Property(e => e.FoodName)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("Food_name");

                entity.Property(e => e.FoodPrice)
                    .HasColumnType("numeric(10, 2)")
                    .HasColumnName("Food_price");

                entity.Property(e => e.FoodStatus).HasColumnName("Food_status");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Foods)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("FK__Foods__Category___31EC6D26");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Order");

                entity.Property(e => e.OrderId).HasColumnName("Order_id");

                entity.Property(e => e.OrderDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Order_Date");

                entity.Property(e => e.PromoCode)
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.Total).HasColumnType("numeric(10, 2)");

                entity.Property(e => e.UserId).HasColumnName("User_id");

                entity.Property(e => e.UserName)
                    .HasMaxLength(160)
                    .IsUnicode(false);

                entity.HasOne(d => d.PromoCodeNavigation)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.PromoCode)
                    .HasConstraintName("FK_Order_Promo");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_Order_User");
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.Property(e => e.OrderDetailId).HasColumnName("OrderDetail_id");

                entity.Property(e => e.FoodId).HasColumnName("Food_id");

                entity.Property(e => e.OrderId).HasColumnName("Order_id");

                entity.Property(e => e.UnitPrice).HasColumnType("numeric(10, 2)");

                entity.HasOne(d => d.Food)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.FoodId)
                    .HasConstraintName("FK__OrderDeta__Food___30F848ED");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("FK__OrderDeta__Order__300424B4");
            });

            modelBuilder.Entity<Promo>(entity =>
            {
                entity.HasKey(e => e.PromoCode)
                    .HasName("PK__Promo__32DBED3473F7547C");

                entity.ToTable("Promo");

                entity.Property(e => e.PromoCode)
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.PromoDescribe)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("Promo_Describe");

                entity.Property(e => e.PromoValue)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("Promo_Value");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Role");

                entity.Property(e => e.RoleId).HasColumnName("Role_id");

                entity.Property(e => e.RoleName)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("Role_name");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");

                entity.HasIndex(e => e.UserName, "UQ__User__C9F28456DA72EA3A")
                    .IsUnique();

                entity.Property(e => e.UserId).HasColumnName("User_id");

                entity.Property(e => e.Address)
                    .HasMaxLength(70)
                    .IsUnicode(false);

                entity.Property(e => e.City)
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.Country)
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .HasMaxLength(160)
                    .IsUnicode(false);

                entity.Property(e => e.FirstName)
                    .HasMaxLength(160)
                    .IsUnicode(false);

                entity.Property(e => e.LastName)
                    .HasMaxLength(160)
                    .IsUnicode(false);

                entity.Property(e => e.Password)
                    .HasMaxLength(160)
                    .IsUnicode(false);

                entity.Property(e => e.Phone)
                    .HasMaxLength(24)
                    .IsUnicode(false);

                entity.Property(e => e.RoleId).HasColumnName("Role_id");

                entity.Property(e => e.State)
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.UserName)
                    .HasMaxLength(160)
                    .IsUnicode(false);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("FK__User__Role_id__33D4B598");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
