using CytubeBotWeb.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CytubeBotWeb.Data
{
    public partial class ServerDbContext : DbContext
    {
        public ServerDbContext()
        {
        }

        public ServerDbContext(DbContextOptions<ServerDbContext> options)
           : base(options)
        {
        }

        public DbSet<ServerModel> Servers { get; set; }
        public DbSet<ChannelModel> Channels { get; set; }
        public DbSet<CommandLogsModel> CommandLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<ChannelModel>(entity =>
            {
                entity.HasOne(c => c.Server)
                    .WithMany(s => s.Channels)
                    .HasForeignKey(c => c.ServerModelId);
            });

            modelBuilder.Entity<ServerModel>(entity =>
            {
                entity.HasMany(s => s.Channels)
                    .WithOne(c => c.Server);
            });

            modelBuilder.Entity<CommandLogsModel>(entity => 
            {
                entity.HasOne(c => c.Channel);
            });

            base.OnModelCreating(modelBuilder);
        }
    }

}
