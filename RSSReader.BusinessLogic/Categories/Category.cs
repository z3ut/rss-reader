using RSSReader.BusinessLogic.Channels;
using RSSReader.BusinessLogic.Feeds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RSSReader.BusinessLogic.Categories
{
	public class Category
	{
		public int CategoryId { get; set; }
		public string Title { get; set; }
		public int? ParentCategoryId { get; set; }

		public Category ParentCategory { get; set; }
		public IEnumerable<Category> ChildCategories { get; set; }
		public IEnumerable<Channel> Channels { get; set; }

		public int NewFeedItemsCount
		{
			get
			{
				var newFeedItemsCount = 0;

				if (Channels != null)
				{
					newFeedItemsCount += Channels
						.Select(c => c.NewFeedItemsCount)
						.Sum();
				}

				if (ChildCategories != null)
				{
					newFeedItemsCount += ChildCategories
						.Select(c => c.NewFeedItemsCount)
						.Sum();
				}

				return newFeedItemsCount;
			}
		}
	}
}
