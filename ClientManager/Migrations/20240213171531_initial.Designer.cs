﻿// <auto-generated />
using System;
using ClientManager.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace ClientManager.Migrations
{
    [DbContext(typeof(ClientManagementContext))]
    [Migration("20240213171531_initial")]
    partial class initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ClientManager.Models.Client", b =>
                {
                    b.Property<int>("ClientId")
                        .HasColumnType("int")
                        .HasColumnName("ClientID");

                    b.Property<decimal?>("AccountBalance")
                        .HasColumnType("decimal(18, 2)");

                    b.Property<string>("CallCenterName")
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("varchar(100)");

                    b.Property<DateTime?>("CaptureDate")
                        .HasColumnType("datetime");

                    b.Property<string>("CapturedBy")
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("ClientName")
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)");

                    b.Property<string>("ClientSurname")
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)");

                    b.Property<string>("ContactNumber")
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("varchar(20)");

                    b.Property<string>("Email")
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("Idnumber")
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("varchar(20)")
                        .HasColumnName("IDNumber");

                    b.Property<decimal?>("PaymentsToDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("decimal(18, 2)")
                        .HasDefaultValue(0m);

                    b.HasKey("ClientId")
                        .HasName("PK__Clients__E67E1A04ADF996C7");

                    b.ToTable("Clients");
                });

            modelBuilder.Entity("ClientManager.Models.Payment", b =>
                {
                    b.Property<int>("PaymentId")
                        .HasColumnType("int")
                        .HasColumnName("PaymentID");

                    b.Property<decimal?>("AmountOfPayment")
                        .HasColumnType("decimal(18, 2)");

                    b.Property<int?>("ClientId")
                        .HasColumnType("int")
                        .HasColumnName("ClientID");

                    b.Property<DateOnly?>("DateOfPayment")
                        .HasColumnType("date");

                    b.Property<string>("ReferenceForPayment")
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("varchar(100)");

                    b.HasKey("PaymentId")
                        .HasName("PK__Payments__9B556A58D8BC7B2C");

                    b.HasIndex("ClientId");

                    b.ToTable("Payments");
                });

            modelBuilder.Entity("ClientManager.Models.Payment", b =>
                {
                    b.HasOne("ClientManager.Models.Client", "Client")
                        .WithMany("Payments")
                        .HasForeignKey("ClientId")
                        .HasConstraintName("FK__Payments__Client__3E52440B");

                    b.Navigation("Client");
                });

            modelBuilder.Entity("ClientManager.Models.Client", b =>
                {
                    b.Navigation("Payments");
                });
#pragma warning restore 612, 618
        }
    }
}
