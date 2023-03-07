using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace FurnitureSalon
{
    public partial class ModelDB : DbContext
    {
        public ModelDB()
            : base("name=ModelDB")
        {
        }

        public virtual DbSet<ConsumerList> ConsumerList { get; set; }
        public virtual DbSet<FurnitureList> FurnitureList { get; set; }
        public virtual DbSet<FurnitureName> FurnitureName { get; set; }
        public virtual DbSet<FurnitureType> FurnitureType { get; set; }
        public virtual DbSet<OrderList> OrderList { get; set; }
        public virtual DbSet<sysdiagrams> sysdiagrams { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ConsumerList>()
                .HasMany(e => e.OrderList)
                .WithRequired(e => e.ConsumerList)
                .HasForeignKey(e => e.consumer_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<FurnitureList>()
                .Property(e => e.price)
                .HasPrecision(19, 4);

            modelBuilder.Entity<FurnitureList>()
                .HasMany(e => e.OrderList)
                .WithRequired(e => e.FurnitureList)
                .HasForeignKey(e => e.furniture_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<FurnitureName>()
                .HasMany(e => e.FurnitureList)
                .WithRequired(e => e.FurnitureName)
                .HasForeignKey(e => e.furniture_name_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<FurnitureType>()
                .HasMany(e => e.FurnitureList)
                .WithRequired(e => e.FurnitureType)
                .HasForeignKey(e => e.furniture_type_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<OrderList>()
                .Property(e => e.deliver_price)
                .HasPrecision(19, 4);

            modelBuilder.Entity<OrderList>()
                .Property(e => e.installation_price)
                .HasPrecision(19, 4);

            modelBuilder.Entity<OrderList>()
                .Property(e => e.total_sum)
                .HasPrecision(19, 4);
        }
    }
}
