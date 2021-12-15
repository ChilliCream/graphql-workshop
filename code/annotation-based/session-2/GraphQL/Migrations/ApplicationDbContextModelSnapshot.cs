﻿// <auto-generated />
using System;
using ConferencePlanner.GraphQL.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace GraphQL.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.0");

            modelBuilder.Entity("ConferencePlanner.GraphQL.Data.Attendee", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int?>("AttendeeId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("EmailAddress")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("TEXT");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("TEXT");

                    b.Property<int?>("SessionId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("AttendeeId");

                    b.HasIndex("SessionId");

                    b.ToTable("Attendees");
                });

            modelBuilder.Entity("ConferencePlanner.GraphQL.Data.Session", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Abstract")
                        .HasMaxLength(4000)
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset?>("EndTime")
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset?>("StartTime")
                        .HasColumnType("TEXT");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("TEXT");

                    b.Property<int?>("TrackId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("TrackId");

                    b.ToTable("Sessions");
                });

            modelBuilder.Entity("ConferencePlanner.GraphQL.Data.Speaker", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Bio")
                        .HasMaxLength(4000)
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("TEXT");

                    b.Property<string>("WebSite")
                        .HasMaxLength(1000)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Speakers");
                });

            modelBuilder.Entity("ConferencePlanner.GraphQL.Data.Track", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Tracks");
                });

            modelBuilder.Entity("SessionSpeaker", b =>
                {
                    b.Property<int>("SessionsId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("SpeakersId")
                        .HasColumnType("INTEGER");

                    b.HasKey("SessionsId", "SpeakersId");

                    b.HasIndex("SpeakersId");

                    b.ToTable("SessionSpeaker");
                });

            modelBuilder.Entity("ConferencePlanner.GraphQL.Data.Attendee", b =>
                {
                    b.HasOne("ConferencePlanner.GraphQL.Data.Attendee", null)
                        .WithMany("Attendees")
                        .HasForeignKey("AttendeeId");

                    b.HasOne("ConferencePlanner.GraphQL.Data.Session", null)
                        .WithMany("Attendees")
                        .HasForeignKey("SessionId");
                });

            modelBuilder.Entity("ConferencePlanner.GraphQL.Data.Session", b =>
                {
                    b.HasOne("ConferencePlanner.GraphQL.Data.Track", "Track")
                        .WithMany("Sessions")
                        .HasForeignKey("TrackId");

                    b.Navigation("Track");
                });

            modelBuilder.Entity("SessionSpeaker", b =>
                {
                    b.HasOne("ConferencePlanner.GraphQL.Data.Session", null)
                        .WithMany()
                        .HasForeignKey("SessionsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ConferencePlanner.GraphQL.Data.Speaker", null)
                        .WithMany()
                        .HasForeignKey("SpeakersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ConferencePlanner.GraphQL.Data.Attendee", b =>
                {
                    b.Navigation("Attendees");
                });

            modelBuilder.Entity("ConferencePlanner.GraphQL.Data.Session", b =>
                {
                    b.Navigation("Attendees");
                });

            modelBuilder.Entity("ConferencePlanner.GraphQL.Data.Track", b =>
                {
                    b.Navigation("Sessions");
                });
#pragma warning restore 612, 618
        }
    }
}
