﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PaulSound_VideoCallMessenger.Context;

#nullable disable

namespace PaulSound_VideoCallMessenger.Migrations
{
    [DbContext(typeof(MessengerDbContext))]
    [Migration("20241105103516_initTables")]
    partial class initTables
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("PaulSound_VideoCallMessenger.Data.ContactList", b =>
                {
                    b.Property<int>("user1_id")
                        .HasColumnType("int");

                    b.Property<int>("user2_id")
                        .HasColumnType("int");

                    b.Property<int>("contactId")
                        .HasColumnType("int");

                    b.HasKey("user1_id", "user2_id");

                    b.ToTable("contact_list");
                });

            modelBuilder.Entity("PaulSound_VideoCallMessenger.Data.Conversation", b =>
                {
                    b.Property<int>("conversationId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("conversationId"));

                    b.Property<string>("ConversationName")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("conversationId");

                    b.ToTable("conversations");
                });

            modelBuilder.Entity("PaulSound_VideoCallMessenger.Data.ConversationMember", b =>
                {
                    b.Property<int>("memberId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("memberId"));

                    b.Property<int>("conversationId")
                        .HasColumnType("int");

                    b.Property<int>("userId")
                        .HasColumnType("int");

                    b.HasKey("memberId");

                    b.HasIndex("conversationId");

                    b.HasIndex("userId");

                    b.ToTable("members");
                });

            modelBuilder.Entity("PaulSound_VideoCallMessenger.Data.Message", b =>
                {
                    b.Property<int>("messageId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("messageId"));

                    b.Property<string>("MessageText")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("SentTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("conversationId")
                        .HasColumnType("int");

                    b.HasKey("messageId");

                    b.HasIndex("conversationId");

                    b.ToTable("messages");
                });

            modelBuilder.Entity("PaulSound_VideoCallMessenger.Data.User", b =>
                {
                    b.Property<int>("userId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("userId"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UniqueIdentifier")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.HasKey("userId");

                    b.ToTable("users");
                });

            modelBuilder.Entity("PaulSound_VideoCallMessenger.Data.ConversationMember", b =>
                {
                    b.HasOne("PaulSound_VideoCallMessenger.Data.Conversation", "conversation")
                        .WithMany()
                        .HasForeignKey("conversationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PaulSound_VideoCallMessenger.Data.User", "user")
                        .WithMany()
                        .HasForeignKey("userId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("conversation");

                    b.Navigation("user");
                });

            modelBuilder.Entity("PaulSound_VideoCallMessenger.Data.Message", b =>
                {
                    b.HasOne("PaulSound_VideoCallMessenger.Data.Conversation", "Conversation")
                        .WithMany()
                        .HasForeignKey("conversationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Conversation");
                });
#pragma warning restore 612, 618
        }
    }
}