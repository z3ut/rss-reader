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
		private readonly DbContextOptions<FeedContext> _feedContextOptions;
		private readonly IMapper _mapper;

		public CategoryService(DbContextOptions<FeedContext> feedContextOptions, IMapper mapper)
		{
			_feedContextOptions = feedContextOptions;
			_mapper = mapper;
		}

		public Category CreateCategory(Category category)
		{
			using var feedContext = new FeedContext(_feedContextOptions);
			var categoryDA = _mapper.Map<DataAccess.Models.Category>(category);
			feedContext.Categories.Add(categoryDA);
			feedContext.SaveChanges();
			feedContext.Entry(categoryDA).Reload();
			return _mapper.Map<Category>(categoryDA);
		}

		public void DeleteCategory(int categoryId)
		{
			using var feedContext = new FeedContext(_feedContextOptions);
			var category = feedContext.Categories
				.SingleOrDefault(c => c.CategoryId == categoryId);

			if (category == null)
			{
				return;
			}

			feedContext.Categories.Remove(category);
			feedContext.SaveChanges();
		}

		public IEnumerable<Category> GetCategories()
		{
			using var feedContext = new FeedContext(_feedContextOptions);
			var categories = feedContext.Categories
				.Include(category => category.ChildCategories)
				.Include(category => category.Channels)
				.ToList()
				.Where(category => category.ParentCategoryId == null);

			return _mapper.Map<IEnumerable<Category>>(categories);
		}

		public IEnumerable<Category> GetCategoriesWithFeeds()
		{
			using var feedContext = new FeedContext(_feedContextOptions);
			return _mapper.Map<IEnumerable<Category>>(feedContext.Categories
				.Include(category => category.ChildCategories)
				.Include(category => category.Channels)
				.ThenInclude(channel => channel.FeedItems)
				.ToList());
		}

		public Category GetCategory(int categoryId)
		{
			using var feedContext = new FeedContext(_feedContextOptions);
			return _mapper.Map<Category>(feedContext.Categories.Find(categoryId));
		}

		public Category UpdateCategory(Category category)
		{
			using var feedContext = new FeedContext(_feedContextOptions);
			var savedCategory = feedContext.Categories.Find(category.CategoryId);

			var newCategory = _mapper.Map<DataAccess.Models.Category>(category);
			feedContext.Entry(savedCategory).CurrentValues.SetValues(newCategory);
			feedContext.SaveChanges();
			feedContext.Entry(savedCategory).Reload();

			return _mapper.Map<Category>(savedCategory);
		}
	}
}
