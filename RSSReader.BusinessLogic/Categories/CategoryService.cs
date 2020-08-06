using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RSSReader.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RSSReader.BusinessLogic.Categories
{
	public class CategoryService : ICategoryService
	{
		private readonly FeedContext _feedContext;
		private readonly IMapper _mapper;

		public CategoryService(FeedContext feedContext, IMapper mapper)
		{
			_feedContext = feedContext;
			_mapper = mapper;
		}

		public Category CreateCategory(Category category)
		{
			var categoryDA = _mapper.Map<DataAccess.Models.Category>(category);
			//categoryDA.CategoryId = Guid.NewGuid().ToString();
			_feedContext.Categories.Add(categoryDA);
			_feedContext.SaveChanges();
			return _mapper.Map<Category>(categoryDA);
		}

		public void DeleteCategory(int categoryId)
		{
			var category = _feedContext.Categories
				.SingleOrDefault(c => c.CategoryId == categoryId);

			if (category == null)
			{
				return;
			}

			_feedContext.Categories.Remove(category);
			_feedContext.SaveChanges();
		}

		public IEnumerable<Category> GetCategories()
		{
			//var cat = _mapper.Map<IEnumerable<Category>>(_feedContext.Categories
			//	.Include(category => category.ChildCategories)
			//	.Include(category => category.Channels)
			//	.ThenInclude(channel => channel.FeedItems)
			//	//.Where(category => String.IsNullOrWhiteSpace(category.ParentCategoryId))
			//	.ToList());

			//var notNullCat = cat.Where(category => category.ParentCategoryId != null);

			var categories = _feedContext.Categories
				.Include(category => category.ChildCategories)
				.Include(category => category.Channels)
				.ToList();

			var channelNewItemsCount = _feedContext.Channels.Select(channel =>
				new ChannelNewItemsCount(channel, _feedContext.FeedItems
					.Where(fi => fi.ChannelId == channel.ChannelId &&
						fi.IsRead == false)
					.Count())
			).ToList();

			//{
			//	channel,
			//	newItemsCount = _feedContext.FeedItems
			//		.Where(fi => fi.ChannelId == channel.ChannelId &&
			//			fi.IsRead == false)
			//		.Count()
			//}

			var categoriesBL = _mapper.Map<IEnumerable<Category>>(categories);

			fillChannelNewItemsCount(categoriesBL, channelNewItemsCount);

			return categoriesBL;

			//return _mapper.Map<IEnumerable<Category>>(_feedContext.Categories
			//	.Include(category => category.ChildCategories)
			//	.Include(category => category.Channels)
			//	.ThenInclude(channel => channel.FeedItems)
			//	.ToList())
			//	.Where(category => category.ParentCategoryId == null);
		}

		private class ChannelNewItemsCount
		{
			public ChannelNewItemsCount(DataAccess.Models.Channel channel, int newItemsCounts)
			{
				Channel = channel;
				NewItemsCounts = newItemsCounts;
			}

			public DataAccess.Models.Channel Channel { get; set; }
			public int NewItemsCounts { get; set; }

		}

		void fillChannelNewItemsCount(IEnumerable<Category> categories,
			IEnumerable<ChannelNewItemsCount> channelNewItemsCounts)
		{
			foreach (var category in categories)
			{
				fillChannelNewItemsCount(category.ChildCategories, channelNewItemsCounts);

				foreach (var channel in category.Channels)
				{
					var channelCount = channelNewItemsCounts
						.FirstOrDefault(c => c.Channel.ChannelId == channel.ChannelId);

					if (channelCount != null)
					{
						channel.NewFeedItemsCount = channelCount.NewItemsCounts;
					}
				}
			}
		}


		public IEnumerable<Category> GetCategoriesWithFeeds()
		{
			return _mapper.Map<IEnumerable<Category>>(_feedContext.Categories
				.Include(category => category.ChildCategories)
				.Include(category => category.Channels)
				.ThenInclude(channel => channel.FeedItems)
				.ToList());
		}

		public Category GetCategory(int categoryId)
		{
			return _mapper.Map<Category>(_feedContext.Categories.Find(categoryId));
		}

		public Category UpdateCategory(Category category)
		{
			var savedCategory = _feedContext.Categories.Find(category.CategoryId);

			var newCategory = _mapper.Map<DataAccess.Models.Category>(category);
			_feedContext.Entry(savedCategory).CurrentValues.SetValues(newCategory);
			_feedContext.SaveChanges();
			_feedContext.Entry(savedCategory).Reload();

			return _mapper.Map<Category>(savedCategory);

		}
	}
}
