using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DotNetCoreSqlDb.Models
{
    public class MyDatabaseContext : DbContext
    {
        public MyDatabaseContext (DbContextOptions<MyDatabaseContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DotNetCoreSqlDb.DataModels.PlaylistTrack>().HasKey(pl => new { pl.PlaylistId, pl.TrackId });
        }



        public DbSet<DotNetCoreSqlDb.Models.Todo> Todo { get; set; }
        public DbSet<DotNetCoreSqlDb.DataModels.MediaType> MediaType { get; set; }
        public DbSet<DotNetCoreSqlDb.DataModels.Album> Album { get; set; }
        public DbSet<DotNetCoreSqlDb.DataModels.Artist> Artist { get; set; }
        public DbSet<DotNetCoreSqlDb.DataModels.Customer> Customer { get; set; }
        public DbSet<DotNetCoreSqlDb.DataModels.Employee> Employee { get; set; }
        public DbSet<DotNetCoreSqlDb.DataModels.Genre> Genre { get; set; }
        public DbSet<DotNetCoreSqlDb.DataModels.Invoice> Invoice { get; set; }
        public DbSet<DotNetCoreSqlDb.DataModels.InvoiceLine> InvoiceLine { get; set; }
        public DbSet<DotNetCoreSqlDb.DataModels.Playlist> Playlist { get; set; }
        public DbSet<DotNetCoreSqlDb.DataModels.PlaylistTrack> PlaylistTrack { get; set; }
        public DbSet<DotNetCoreSqlDb.DataModels.Track> Track { get; set; }
    }
}
