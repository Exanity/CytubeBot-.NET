﻿// <auto-generated />
using System;
using CytubeBotWeb.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CytubeBotWeb.Migrations.ServerDb
{
    [DbContext(typeof(ServerDbContext))]
    partial class ServerDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.8-servicing-32085")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("CytubeBotWeb.Models.ChannelModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ChannelName");

                    b.Property<int>("ServerModelId");

                    b.HasKey("Id");

                    b.HasIndex("ServerModelId");

                    b.ToTable("Channels");
                });

            modelBuilder.Entity("CytubeBotWeb.Models.CommandLogsModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ChannelModelId");

                    b.Property<string>("Command");

                    b.Property<string>("Message");

                    b.Property<DateTime>("MessageTime");

                    b.Property<string>("User");

                    b.HasKey("Id");

                    b.HasIndex("ChannelModelId");

                    b.ToTable("CommandLogs");
                });

            modelBuilder.Entity("CytubeBotWeb.Models.ServerModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Host");

                    b.Property<string>("Password");

                    b.Property<string>("Username");

                    b.HasKey("Id");

                    b.ToTable("Servers");
                });

            modelBuilder.Entity("CytubeBotWeb.Models.ChannelModel", b =>
                {
                    b.HasOne("CytubeBotWeb.Models.ServerModel", "Server")
                        .WithMany("Channels")
                        .HasForeignKey("ServerModelId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("CytubeBotWeb.Models.CommandLogsModel", b =>
                {
                    b.HasOne("CytubeBotWeb.Models.ChannelModel", "Channel")
                        .WithMany()
                        .HasForeignKey("ChannelModelId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
