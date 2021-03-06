﻿// <auto-generated />
using LanguageLearningChatBotCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace LanguageLearningChatBotCore.Migrations
{
    [DbContext(typeof(ScenarioContext))]
    [Migration("20180724205525_initial create")]
    partial class initialcreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.1-rtm-30846");

            modelBuilder.Entity("LanguageLearningChatBotCore.Prompt", b =>
                {
                    b.Property<int>("PromptID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("OptionNumber");

                    b.Property<string>("PromptText");

                    b.Property<int>("ResponseNumber");

                    b.Property<int>("ScenarioID");

                    b.HasKey("PromptID");

                    b.HasIndex("ScenarioID");

                    b.ToTable("Prompts");
                });

            modelBuilder.Entity("LanguageLearningChatBotCore.Scenario", b =>
                {
                    b.Property<int>("ScenarioID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Title");

                    b.HasKey("ScenarioID");

                    b.ToTable("Scenarios");
                });

            modelBuilder.Entity("LanguageLearningChatBotCore.Prompt", b =>
                {
                    b.HasOne("LanguageLearningChatBotCore.Scenario", "Scenario")
                        .WithMany("Prompts")
                        .HasForeignKey("ScenarioID")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
