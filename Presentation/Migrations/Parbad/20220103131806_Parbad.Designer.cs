﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Parbad.Storage.EntityFrameworkCore.Context;

#nullable disable

namespace Presentation.Migrations.Parbad
{
    [DbContext(typeof(ParbadDataContext))]
    [Migration("20220103131806_Parbad")]
    partial class Parbad
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Parbad.Storage.EntityFrameworkCore.Domain.PaymentEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("payment_id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<decimal>("Amount")
                        .HasColumnType("numeric(18,2)")
                        .HasColumnName("amount");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_on");

                    b.Property<string>("GatewayAccountName")
                        .HasColumnType("text")
                        .HasColumnName("gateway_account_name");

                    b.Property<string>("GatewayName")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)")
                        .HasColumnName("gateway_name");

                    b.Property<bool>("IsCompleted")
                        .HasColumnType("boolean")
                        .HasColumnName("is_completed");

                    b.Property<bool>("IsPaid")
                        .HasColumnType("boolean")
                        .HasColumnName("is_paid");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("token");

                    b.Property<long>("TrackingNumber")
                        .HasColumnType("bigint")
                        .HasColumnName("tracking_number");

                    b.Property<string>("TransactionCode")
                        .HasColumnType("text")
                        .HasColumnName("transaction_code");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_on");

                    b.HasKey("Id")
                        .HasName("payment_id");

                    b.HasIndex("Token")
                        .IsUnique();

                    b.HasIndex("TrackingNumber")
                        .IsUnique();

                    b.ToTable("PaymentTable", "public");
                });

            modelBuilder.Entity("Parbad.Storage.EntityFrameworkCore.Domain.TransactionEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("transaction_id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("AdditionalData")
                        .HasColumnType("text")
                        .HasColumnName("additional_data");

                    b.Property<decimal>("Amount")
                        .HasColumnType("numeric(18,2)")
                        .HasColumnName("amount");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_on");

                    b.Property<bool>("IsSucceed")
                        .HasColumnType("boolean")
                        .HasColumnName("is_succeed");

                    b.Property<string>("Message")
                        .HasColumnType("text")
                        .HasColumnName("message");

                    b.Property<long>("PaymentId")
                        .HasColumnType("bigint");

                    b.Property<byte>("Type")
                        .HasColumnType("smallint")
                        .HasColumnName("type");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_on");

                    b.HasKey("Id")
                        .HasName("transaction_id");

                    b.HasIndex("PaymentId");

                    b.ToTable("TransactionTable", "public");
                });

            modelBuilder.Entity("Parbad.Storage.EntityFrameworkCore.Domain.TransactionEntity", b =>
                {
                    b.HasOne("Parbad.Storage.EntityFrameworkCore.Domain.PaymentEntity", "Payment")
                        .WithMany("Transactions")
                        .HasForeignKey("PaymentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Payment");
                });

            modelBuilder.Entity("Parbad.Storage.EntityFrameworkCore.Domain.PaymentEntity", b =>
                {
                    b.Navigation("Transactions");
                });
#pragma warning restore 612, 618
        }
    }
}
