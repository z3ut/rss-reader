using System;
using System.Collections.Generic;
using System.Text;

namespace RSSReader.BusinessLogic.Categories
{
	public interface ICategoryService
	{
		Category CreateCategory(Category category);
		Category GetCategory(int categoryId);
		Category UpdateCategory(Category category);
		void DeleteCategory(int categoryId);

		IEnumerable<Category> GetCategories();
		IEnumerable<Category> GetCategoriesWithFeeds();
	}
}
