using System;
using System.Collections.Generic;
using System.Text;

namespace RSSReader.DataAccess.Models
{
	public class Category
	{
		public int CategoryId { get; set; }
		public string Title { get; set; }
		public int? ParentCategoryId { get; set; }

		public Category ParentCategory { get; set; }
		public IEnumerable<Category> ChildCategories { get; set; }
		public IEnumerable<Channel> Channels { get; set; }
	}
}
