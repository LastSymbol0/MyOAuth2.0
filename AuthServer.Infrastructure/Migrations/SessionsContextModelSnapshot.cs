﻿// <auto-generated />
using System;
using AuthServer.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AuthServer.Infrastructure.Migrations
{
    [DbContext(typeof(SessionsContext))]
    partial class SessionsContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("AuthServer.Domain.AggregatesModel.SessionAggregate.Client", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<string>("ClientSecret")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.ToTable("Client");
                });

            modelBuilder.Entity("AuthServer.Domain.AggregatesModel.SessionAggregate.ResourceOwner", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("ResourceOwner");
                });

            modelBuilder.Entity("AuthServer.Domain.AggregatesModel.SessionAggregate.Session", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<int>("ClientGrantType")
                        .HasColumnType("int");

                    b.Property<string>("ClientGrantValue")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int>("ClientId")
                        .HasColumnType("int");

                    b.Property<DateTime>("ExpireIn")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("ResourceOwnerId")
                        .HasColumnType("int");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Sessions");
                });

            modelBuilder.Entity("AuthServer.Domain.AggregatesModel.SessionAggregate.Session", b =>
                {
                    b.OwnsOne("AuthServer.Domain.AggregatesModel.SessionAggregate.AccessParameters", "AccessParameters", b1 =>
                        {
                            b1.Property<int>("SessionId")
                                .HasColumnType("int");

                            b1.Property<string>("Scopes")
                                .HasColumnType("longtext CHARACTER SET utf8mb4");

                            b1.HasKey("SessionId");

                            b1.ToTable("Sessions");

                            b1.WithOwner()
                                .HasForeignKey("SessionId");
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
