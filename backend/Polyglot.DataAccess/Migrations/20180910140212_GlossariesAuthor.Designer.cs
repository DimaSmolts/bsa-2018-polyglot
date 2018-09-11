﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Polyglot.DataAccess.SqlRepository;

namespace Polyglot.DataAccess.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20180910140212_GlossariesAuthor")]
    partial class GlossariesAuthor
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.2-rtm-30932")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Polyglot.DataAccess.Entities.Chat.ChatDialog", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("DialogName");

                    b.Property<int>("DialogType");

                    b.Property<long>("Identifier");

                    b.HasKey("Id");

                    b.ToTable("ChatDialogs");
                });

            modelBuilder.Entity("Polyglot.DataAccess.Entities.Chat.ChatMessage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Body");

                    b.Property<int?>("DialogId");

                    b.Property<bool>("IsRead");

                    b.Property<DateTime>("ReceivedDate");

                    b.Property<int>("SenderId");

                    b.HasKey("Id");

                    b.HasIndex("DialogId");

                    b.HasIndex("SenderId");

                    b.ToTable("ChatMessages");
                });

            modelBuilder.Entity("Polyglot.DataAccess.Entities.Chat.DialogParticipant", b =>
                {
                    b.Property<int?>("ChatDialogId");

                    b.Property<int?>("ParticipantId");

                    b.HasKey("ChatDialogId", "ParticipantId");

                    b.HasIndex("ParticipantId");

                    b.ToTable("DialogParticipant");
                });

            modelBuilder.Entity("Polyglot.DataAccess.Entities.Chat.UserState", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ChatUserId");

                    b.Property<bool>("IsOnline");

                    b.Property<DateTime>("LastSeen");

                    b.HasKey("Id");

                    b.HasIndex("ChatUserId");

                    b.ToTable("ChatUserStates");
                });

            modelBuilder.Entity("Polyglot.DataAccess.Entities.ComplexString", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ProjectId");

                    b.Property<string>("TranslationKey");

                    b.HasKey("Id");

                    b.HasIndex("ProjectId");

                    b.ToTable("ComplexStrings");
                });

            modelBuilder.Entity("Polyglot.DataAccess.Entities.File", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreatedOn");

                    b.Property<string>("Link");

                    b.Property<int?>("ProjectId");

                    b.Property<int?>("UploadedById");

                    b.HasKey("Id");

                    b.HasIndex("ProjectId");

                    b.HasIndex("UploadedById");

                    b.ToTable("Files");
                });

            modelBuilder.Entity("Polyglot.DataAccess.Entities.Glossary", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name");

                    b.Property<int?>("OriginLanguageId");

                    b.Property<int?>("UserProfileId");

                    b.HasKey("Id");

                    b.HasIndex("OriginLanguageId");

                    b.HasIndex("UserProfileId");

                    b.ToTable("Glossaries");
                });

            modelBuilder.Entity("Polyglot.DataAccess.Entities.GlossaryString", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ExplanationText");

                    b.Property<int?>("GlossaryId");

                    b.Property<string>("TermText");

                    b.HasKey("Id");

                    b.HasIndex("GlossaryId");

                    b.ToTable("GlossaryStrings");
                });

            modelBuilder.Entity("Polyglot.DataAccess.Entities.Language", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Code");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Languages");
                });

            modelBuilder.Entity("Polyglot.DataAccess.Entities.Notification", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Message");

                    b.Property<int>("NotificationAction");

                    b.Property<int>("Payload");

                    b.Property<int?>("ReceiverId");

                    b.Property<int?>("SenderId");

                    b.HasKey("Id");

                    b.HasIndex("ReceiverId");

                    b.HasIndex("SenderId");

                    b.ToTable("Notifications");
                });

            modelBuilder.Entity("Polyglot.DataAccess.Entities.Option", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("NotificationId");

                    b.Property<int>("OptionDefinition");

                    b.HasKey("Id");

                    b.HasIndex("NotificationId");

                    b.ToTable("Options");
                });

            modelBuilder.Entity("Polyglot.DataAccess.Entities.Project", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreatedOn");

                    b.Property<string>("Description");

                    b.Property<string>("ImageUrl");

                    b.Property<int?>("MainLanguageId");

                    b.Property<string>("Name");

                    b.Property<string>("Technology");

                    b.Property<int?>("UserProfileId");

                    b.HasKey("Id");

                    b.HasIndex("MainLanguageId");

                    b.HasIndex("UserProfileId");

                    b.ToTable("Projects");
                });

            modelBuilder.Entity("Polyglot.DataAccess.Entities.ProjectGlossary", b =>
                {
                    b.Property<int?>("GlossaryId");

                    b.Property<int?>("ProjectId");

                    b.HasKey("GlossaryId", "ProjectId");

                    b.HasIndex("ProjectId");

                    b.ToTable("ProjectGlossary");
                });

            modelBuilder.Entity("Polyglot.DataAccess.Entities.ProjectHistory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ActionType");

                    b.Property<int?>("AuthorId");

                    b.Property<string>("OriginValue");

                    b.Property<int?>("ProjectId");

                    b.Property<string>("TableName");

                    b.Property<DateTime>("Time");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("ProjectId");

                    b.ToTable("ProjectHistories");
                });

            modelBuilder.Entity("Polyglot.DataAccess.Entities.ProjectLanguage", b =>
                {
                    b.Property<int?>("LanguageId");

                    b.Property<int?>("ProjectId");

                    b.HasKey("LanguageId", "ProjectId");

                    b.HasIndex("ProjectId");

                    b.ToTable("ProjectLanguage");
                });

            modelBuilder.Entity("Polyglot.DataAccess.Entities.ProjectTeam", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ProjectId");

                    b.Property<int>("TeamId");

                    b.HasKey("Id");

                    b.HasIndex("ProjectId");

                    b.HasIndex("TeamId");

                    b.ToTable("ProjectTeams");
                });

            modelBuilder.Entity("Polyglot.DataAccess.Entities.Rating", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Comment");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("CreatedById");

                    b.Property<double>("Rate");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("CreatedById");

                    b.HasIndex("UserId");

                    b.ToTable("Ratings");
                });

            modelBuilder.Entity("Polyglot.DataAccess.Entities.Right", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Definition");

                    b.HasKey("Id");

                    b.ToTable("Rights");
                });

            modelBuilder.Entity("Polyglot.DataAccess.Entities.Tag", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Color");

                    b.Property<string>("Name");

                    b.Property<int?>("ProjectId");

                    b.HasKey("Id");

                    b.HasIndex("ProjectId");

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("Polyglot.DataAccess.Entities.Team", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("CreatedById");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.HasIndex("CreatedById");

                    b.ToTable("Teams");
                });

            modelBuilder.Entity("Polyglot.DataAccess.Entities.TeamTranslator", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("IsActivated");

                    b.Property<int>("TeamId");

                    b.Property<int>("TranslatorId");

                    b.HasKey("Id");

                    b.HasIndex("TeamId");

                    b.HasIndex("TranslatorId");

                    b.ToTable("TeamTranslators");
                });

            modelBuilder.Entity("Polyglot.DataAccess.Entities.TranslatorLanguage", b =>
                {
                    b.Property<int?>("TranslatorId");

                    b.Property<int?>("LanguageId");

                    b.Property<int>("Proficiency");

                    b.HasKey("TranslatorId", "LanguageId");

                    b.HasIndex("LanguageId");

                    b.ToTable("TranslatorLanguages");
                });

            modelBuilder.Entity("Polyglot.DataAccess.Entities.TranslatorRight", b =>
                {
                    b.Property<int?>("TeamTranslatorId");

                    b.Property<int?>("RightId");

                    b.HasKey("TeamTranslatorId", "RightId");

                    b.HasIndex("RightId");

                    b.ToTable("TranslatorRight");
                });

            modelBuilder.Entity("Polyglot.DataAccess.Entities.UserProfile", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Address");

                    b.Property<string>("AvatarUrl");

                    b.Property<DateTime?>("BirthDate");

                    b.Property<string>("City");

                    b.Property<string>("Country");

                    b.Property<string>("FullName");

                    b.Property<string>("Phone");

                    b.Property<string>("PostalCode");

                    b.Property<string>("Region");

                    b.Property<DateTime>("RegistrationDate");

                    b.Property<string>("Uid");

                    b.Property<int>("UserRole");

                    b.HasKey("Id");

                    b.ToTable("UserProfiles");
                });

            modelBuilder.Entity("Polyglot.DataAccess.Entities.Chat.ChatMessage", b =>
                {
                    b.HasOne("Polyglot.DataAccess.Entities.Chat.ChatDialog", "Dialog")
                        .WithMany("Messages")
                        .HasForeignKey("DialogId");

                    b.HasOne("Polyglot.DataAccess.Entities.UserProfile", "Sender")
                        .WithMany()
                        .HasForeignKey("SenderId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Polyglot.DataAccess.Entities.Chat.DialogParticipant", b =>
                {
                    b.HasOne("Polyglot.DataAccess.Entities.Chat.ChatDialog", "ChatDialog")
                        .WithMany("DialogParticipants")
                        .HasForeignKey("ChatDialogId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Polyglot.DataAccess.Entities.UserProfile", "Participant")
                        .WithMany()
                        .HasForeignKey("ParticipantId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Polyglot.DataAccess.Entities.Chat.UserState", b =>
                {
                    b.HasOne("Polyglot.DataAccess.Entities.UserProfile", "ChatUser")
                        .WithMany()
                        .HasForeignKey("ChatUserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Polyglot.DataAccess.Entities.ComplexString", b =>
                {
                    b.HasOne("Polyglot.DataAccess.Entities.Project", "Project")
                        .WithMany("Translations")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Polyglot.DataAccess.Entities.File", b =>
                {
                    b.HasOne("Polyglot.DataAccess.Entities.Project", "Project")
                        .WithMany()
                        .HasForeignKey("ProjectId");

                    b.HasOne("Polyglot.DataAccess.Entities.UserProfile", "UploadedBy")
                        .WithMany()
                        .HasForeignKey("UploadedById");
                });

            modelBuilder.Entity("Polyglot.DataAccess.Entities.Glossary", b =>
                {
                    b.HasOne("Polyglot.DataAccess.Entities.Language", "OriginLanguage")
                        .WithMany("Glossaries")
                        .HasForeignKey("OriginLanguageId");

                    b.HasOne("Polyglot.DataAccess.Entities.UserProfile", "UserProfile")
                        .WithMany()
                        .HasForeignKey("UserProfileId");
                });

            modelBuilder.Entity("Polyglot.DataAccess.Entities.GlossaryString", b =>
                {
                    b.HasOne("Polyglot.DataAccess.Entities.Glossary")
                        .WithMany("GlossaryStrings")
                        .HasForeignKey("GlossaryId");
                });

            modelBuilder.Entity("Polyglot.DataAccess.Entities.Notification", b =>
                {
                    b.HasOne("Polyglot.DataAccess.Entities.UserProfile", "Receiver")
                        .WithMany()
                        .HasForeignKey("ReceiverId");

                    b.HasOne("Polyglot.DataAccess.Entities.UserProfile", "Sender")
                        .WithMany()
                        .HasForeignKey("SenderId");
                });

            modelBuilder.Entity("Polyglot.DataAccess.Entities.Option", b =>
                {
                    b.HasOne("Polyglot.DataAccess.Entities.Notification", "Notification")
                        .WithMany("Options")
                        .HasForeignKey("NotificationId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Polyglot.DataAccess.Entities.Project", b =>
                {
                    b.HasOne("Polyglot.DataAccess.Entities.Language", "MainLanguage")
                        .WithMany("Projects")
                        .HasForeignKey("MainLanguageId");

                    b.HasOne("Polyglot.DataAccess.Entities.UserProfile", "UserProfile")
                        .WithMany("Projects")
                        .HasForeignKey("UserProfileId");
                });

            modelBuilder.Entity("Polyglot.DataAccess.Entities.ProjectGlossary", b =>
                {
                    b.HasOne("Polyglot.DataAccess.Entities.Glossary", "Glossary")
                        .WithMany("ProjectGlossaries")
                        .HasForeignKey("GlossaryId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Polyglot.DataAccess.Entities.Project", "Project")
                        .WithMany("ProjectGlossaries")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Polyglot.DataAccess.Entities.ProjectHistory", b =>
                {
                    b.HasOne("Polyglot.DataAccess.Entities.UserProfile", "Author")
                        .WithMany()
                        .HasForeignKey("AuthorId");

                    b.HasOne("Polyglot.DataAccess.Entities.Project", "Project")
                        .WithMany()
                        .HasForeignKey("ProjectId");
                });

            modelBuilder.Entity("Polyglot.DataAccess.Entities.ProjectLanguage", b =>
                {
                    b.HasOne("Polyglot.DataAccess.Entities.Language", "Language")
                        .WithMany()
                        .HasForeignKey("LanguageId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Polyglot.DataAccess.Entities.Project", "Project")
                        .WithMany("ProjectLanguageses")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Polyglot.DataAccess.Entities.ProjectTeam", b =>
                {
                    b.HasOne("Polyglot.DataAccess.Entities.Project", "Project")
                        .WithMany("ProjectTeams")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Polyglot.DataAccess.Entities.Team", "Team")
                        .WithMany("ProjectTeams")
                        .HasForeignKey("TeamId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Polyglot.DataAccess.Entities.Rating", b =>
                {
                    b.HasOne("Polyglot.DataAccess.Entities.UserProfile", "CreatedBy")
                        .WithMany()
                        .HasForeignKey("CreatedById")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Polyglot.DataAccess.Entities.UserProfile", "User")
                        .WithMany("Ratings")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Polyglot.DataAccess.Entities.Tag", b =>
                {
                    b.HasOne("Polyglot.DataAccess.Entities.Project")
                        .WithMany("Tags")
                        .HasForeignKey("ProjectId");
                });

            modelBuilder.Entity("Polyglot.DataAccess.Entities.Team", b =>
                {
                    b.HasOne("Polyglot.DataAccess.Entities.UserProfile", "CreatedBy")
                        .WithMany()
                        .HasForeignKey("CreatedById");
                });

            modelBuilder.Entity("Polyglot.DataAccess.Entities.TeamTranslator", b =>
                {
                    b.HasOne("Polyglot.DataAccess.Entities.Team", "Team")
                        .WithMany("TeamTranslators")
                        .HasForeignKey("TeamId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Polyglot.DataAccess.Entities.UserProfile", "UserProfile")
                        .WithMany("TeamTranslators")
                        .HasForeignKey("TranslatorId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Polyglot.DataAccess.Entities.TranslatorLanguage", b =>
                {
                    b.HasOne("Polyglot.DataAccess.Entities.Language", "Language")
                        .WithMany()
                        .HasForeignKey("LanguageId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Polyglot.DataAccess.Entities.UserProfile", "UserProfile")
                        .WithMany()
                        .HasForeignKey("TranslatorId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Polyglot.DataAccess.Entities.TranslatorRight", b =>
                {
                    b.HasOne("Polyglot.DataAccess.Entities.Right", "Right")
                        .WithMany("TranslatorRights")
                        .HasForeignKey("RightId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Polyglot.DataAccess.Entities.TeamTranslator", "TeamTranslator")
                        .WithMany("TranslatorRights")
                        .HasForeignKey("TeamTranslatorId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
