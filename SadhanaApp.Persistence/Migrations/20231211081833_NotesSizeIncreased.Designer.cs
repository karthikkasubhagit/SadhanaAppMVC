﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace SadhanaApp.Persistence.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20231211081833_NotesSizeIncreased")]
    partial class NotesSizeIncreased
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.12")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ChantingRecord", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<int?>("DayRounds")
                        .HasColumnType("int");

                    b.Property<int?>("EveningRounds")
                        .HasColumnType("int");

                    b.Property<int?>("HearingDurationInMinutes")
                        .HasColumnType("int");

                    b.Property<string>("HearingTitle")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("MorningRounds")
                        .HasColumnType("int");

                    b.Property<string>("Notes")
                        .HasMaxLength(400)
                        .HasColumnType("nvarchar(400)");

                    b.Property<int?>("ReadingDurationInMinutes")
                        .HasColumnType("int");

                    b.Property<string>("ReadingTitle")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("RecordId")
                        .HasColumnType("int");

                    b.Property<int?>("ServiceDurationInMinutes")
                        .HasColumnType("int");

                    b.Property<int?>("ServiceTypeId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ServiceTypeId");

                    b.HasIndex("UserId");

                    b.ToTable("ChantingRecords");
                });

            modelBuilder.Entity("SadhanaApp.Domain.ServiceType", b =>
                {
                    b.Property<int>("ServiceTypeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ServiceTypeId"));

                    b.Property<string>("ServiceName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("ServiceTypeId");

                    b.HasIndex("UserId");

                    b.ToTable("ServiceTypes");

                    b.HasData(
                        new
                        {
                            ServiceTypeId = 1,
                            ServiceName = "Cleaning Temple",
                            UserId = 0
                        },
                        new
                        {
                            ServiceTypeId = 2,
                            ServiceName = "Garlands",
                            UserId = 0
                        },
                        new
                        {
                            ServiceTypeId = 3,
                            ServiceName = "Cooking",
                            UserId = 0
                        },
                        new
                        {
                            ServiceTypeId = 4,
                            ServiceName = "Serving Prasadam",
                            UserId = 0
                        },
                        new
                        {
                            ServiceTypeId = 5,
                            ServiceName = "Book Distribution",
                            UserId = 0
                        },
                        new
                        {
                            ServiceTypeId = 6,
                            ServiceName = "Giving Lecture",
                            UserId = 0
                        },
                        new
                        {
                            ServiceTypeId = 7,
                            ServiceName = "Deity Worship",
                            UserId = 0
                        },
                        new
                        {
                            ServiceTypeId = 8,
                            ServiceName = "Voice Program Lecture",
                            UserId = 0
                        },
                        new
                        {
                            ServiceTypeId = 9,
                            ServiceName = "Voice Program Service",
                            UserId = 0
                        },
                        new
                        {
                            ServiceTypeId = 10,
                            ServiceName = "Digital Service",
                            UserId = 0
                        });
                });

            modelBuilder.Entity("User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserId"));

                    b.Property<DateTime>("DateRegistered")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsInstructor")
                        .HasColumnType("bit");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("ShikshaGuruId")
                        .HasColumnType("int");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId");

                    b.HasIndex("ShikshaGuruId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("ChantingRecord", b =>
                {
                    b.HasOne("SadhanaApp.Domain.ServiceType", "ServiceType")
                        .WithMany()
                        .HasForeignKey("ServiceTypeId");

                    b.HasOne("User", "User")
                        .WithMany("ChantingRecords")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ServiceType");

                    b.Navigation("User");
                });

            modelBuilder.Entity("SadhanaApp.Domain.ServiceType", b =>
                {
                    b.HasOne("User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("User", b =>
                {
                    b.HasOne("User", "ShikshaGuru")
                        .WithMany("Devotees")
                        .HasForeignKey("ShikshaGuruId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("ShikshaGuru");
                });

            modelBuilder.Entity("User", b =>
                {
                    b.Navigation("ChantingRecords");

                    b.Navigation("Devotees");
                });
#pragma warning restore 612, 618
        }
    }
}
