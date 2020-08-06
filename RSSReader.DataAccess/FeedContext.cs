using Microsoft.EntityFrameworkCore;
using RSSReader.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace RSSReader.DataAccess
{
	public class FeedContext : DbContext
	{
		private readonly string _connectionString;

		public DbSet<Category> Categories { get; set; }
		public DbSet<Channel> Channels { get; set; }
		public DbSet<FeedItem> FeedItems { get; set; }

		public FeedContext() { }

		//public FeedContext(string connectionString)
		//{
		//	_connectionString = connectionString;
		//}

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

			//modelBuilder.Entity<Channel>()
			//	.Property(c => c.NewFeedItemsCount)
			//	.HasComputedColumnSql("SELECT COUNT(*) FROM FeedItems WHERE FeedItems.ChannelId == ChannelId");


			//modelBuilder.Entity<FeedItem>()
			//	.HasOne<Channel>()
			//	.WithMany(c => c.FeedItems)
			//	.HasForeignKey(item => item.ChannelId)
			//	.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<FeedItem>()
				.Property(c => c.FeedItemId)
				.ValueGeneratedOnAdd();
		}

		protected override void OnConfiguring(DbContextOptionsBuilder options)
		{
			//options.UseSqlite(_connectionString);

			if (!options.IsConfigured)
			{
				options.UseSqlite("Data Source=feed.db");
			}
		}

		public void GetChannelNewFeedItemsCount()
		{
			// Database.ExecuteSqlCommand("");
			Database.ExecuteSqlRaw("");
		}
	}
}
