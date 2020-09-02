using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace RSSReader.BusinessLogic.Configuration
{
	public class BusinessLogicProfile : Profile
	{
		public BusinessLogicProfile()
		{
			CreateMap<Categories.Category, DataAccess.Models.Category>().ReverseMap();
			CreateMap<Channels.Channel, DataAccess.Models.Channel>().ReverseMap();
			CreateMap<Feeds.FeedItem, DataAccess.Models.FeedItem>().ReverseMap();
		}
	}
}
