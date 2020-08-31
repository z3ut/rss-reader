using Autofac;
using Microsoft.EntityFrameworkCore;
using RSSReader.BusinessLogic.Categories;
using RSSReader.BusinessLogic.Channels;
using RSSReader.BusinessLogic.Feeds;
using RSSReader.BusinessLogic.Loader;
using RSSReader.BusinessLogic.Updater;
using RSSReader.DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace RSSReader.BusinessLogic.Configuration
{
	public class BusinessLogicModule : Module
	{
		private readonly string _connectionString;

		public BusinessLogicModule(string connectionString)
		{
			_connectionString = connectionString;
		}

		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterType<CategoryService>().As<ICategoryService>();
			builder.RegisterType<ChannelService>().As<IChannelService>();
			builder.RegisterType<FeedService>().As<IFeedService>();
			builder.RegisterType<FeedLoader>().As<IFeedLoader>();
			builder.RegisterType<FeedUpdater>().As<IFeedUpdater>();

			var feedContextOptions = new DbContextOptionsBuilder<FeedContext>()
				.UseSqlite(_connectionString)
				.Options;
			builder.RegisterInstance(feedContextOptions).As<DbContextOptions<FeedContext>>();
			builder.RegisterType<FeedContext>().As<FeedContext>();
		}
	}
}
