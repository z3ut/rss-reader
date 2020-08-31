using Microsoft.EntityFrameworkCore;
using RSSReader.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace RSSReader.DataAccess
{
	public class FeedContext : DbContext
	{
		public DbSet<Category> Categories { get; set; }
		public DbSet<Channel> Channels { get; set; }
		public DbSet<FeedItem> FeedItems { get; set; }

		public FeedContext() { }

		public FeedContext(DbContextOptions<FeedContext> options) : base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Category>()
				.HasOne<Category>()
				.WithMany(c => c.ChildCategories)
				.HasForeignKey(c => c.ParentCategoryId)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<Category>()
				.Property(c => c.CategoryId)
				.ValueGeneratedOnAdd();


			modelBuilder.Entity<Channel>()
				.HasOne<Category>()
				.WithMany(c => c.Channels)
				.HasForeignKey(c => c.CategoryId)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<Channel>()
				.Property(c => c.ChannelId)
				.ValueGeneratedOnAdd();

			modelBuilder.Entity<FeedItem>()
				.Property(c => c.FeedItemId)
				.ValueGeneratedOnAdd();
		}

		protected override void OnConfiguring(DbContextOptionsBuilder options)
		{
			if (!options.IsConfigured)
			{
				options.UseSqlite("Data Source=feed.db");
			}
		}
	}
}
