﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RSSReader.DataAccess;

namespace RSSReader.DataAccess.Migrations
{
    [DbContext(typeof(FeedContext))]
    partial class FeedContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.5");

            modelBuilder.Entity("RSSReader.DataAccess.Models.Category", b =>
                {
                    b.Property<int>("CategoryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int?>("ParentCategoryCategoryId")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("ParentCategoryId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Title")
                        .HasColumnType("TEXT");

                    b.HasKey("CategoryId");

                    b.HasIndex("ParentCategoryCategoryId");

                    b.HasIndex("ParentCategoryId");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("RSSReader.DataAccess.Models.Channel", b =>
                {
                    b.Property<int>("ChannelId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("CategoryId")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("CategoryId1")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Link")
                        .HasColumnType("TEXT");

                    b.Property<string>("Title")
                        .HasColumnType("TEXT");

                    b.HasKey("ChannelId");

                    b.HasIndex("CategoryId");

                    b.HasIndex("CategoryId1");

                    b.ToTable("Channels");
                });

            modelBuilder.Entity("RSSReader.DataAccess.Models.FeedItem", b =>
                {
                    b.Property<int>("FeedItemId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("ChannelId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("DateTime")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsRead")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Link")
                        .HasColumnType("TEXT");

                    b.Property<string>("RSSFeedId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Title")
                        .HasColumnType("TEXT");

                    b.HasKey("FeedItemId");

                    b.HasIndex("ChannelId");

                    b.ToTable("FeedItems");
                });

            modelBuilder.Entity("RSSReader.DataAccess.Models.Category", b =>
                {
                    b.HasOne("RSSReader.DataAccess.Models.Category", "ParentCategory")
                        .WithMany()
                        .HasForeignKey("ParentCategoryCategoryId");

                    b.HasOne("RSSReader.DataAccess.Models.Category", null)
                        .WithMany("ChildCategories")
                        .HasForeignKey("ParentCategoryId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("RSSReader.DataAccess.Models.Channel", b =>
                {
                    b.HasOne("RSSReader.DataAccess.Models.Category", null)
                        .WithMany("Channels")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("RSSReader.DataAccess.Models.Category", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId1");
                });

            modelBuilder.Entity("RSSReader.DataAccess.Models.FeedItem", b =>
                {
                    b.HasOne("RSSReader.DataAccess.Models.Channel", "Channel")
                        .WithMany("FeedItems")
                        .HasForeignKey("ChannelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
