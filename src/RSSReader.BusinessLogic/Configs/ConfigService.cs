using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RSSReader.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RSSReader.BusinessLogic.Configs
{
	public class ConfigService : IConfigService
	{
		private readonly DbContextOptions<FeedContext> _feedContextOptions;
		private readonly IMapper _mapper;

		public ConfigService(DbContextOptions<FeedContext> feedContextOptions, IMapper mapper)
		{
			_feedContextOptions = feedContextOptions;
			_mapper = mapper;
		}

		public Config GetConfig()
		{
			using var feedContext = new FeedContext(_feedContextOptions);
			var config = feedContext.Configs.First();
			return _mapper.Map<Config>(config);
		}

		public Config UpdateCongid(Config config)
		{
			using var feedContext = new FeedContext(_feedContextOptions);
			var savedConfig = feedContext.Configs.First();

			var newConfig = _mapper.Map<DataAccess.Models.Config>(config);
			feedContext.Entry(savedConfig).CurrentValues.SetValues(newConfig);
			feedContext.SaveChanges();
			feedContext.Entry(savedConfig).Reload();

			return _mapper.Map<Config>(savedConfig);
		}
	}
}
