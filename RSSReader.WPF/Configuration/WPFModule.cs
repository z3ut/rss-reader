using Autofac;
using AutoMapper;
using RSSReader.BusinessLogic.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace RSSReader.WPF.Configuration
{
	public class WPFModule : Module
	{
		private readonly string _connectionString;

		public WPFModule(string connectionString)
		{
			_connectionString = connectionString;
		}

		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterModule(new BusinessLogicModule(_connectionString));

			var config = new MapperConfiguration(cfg =>
			{
				cfg.AddProfile(new BusinessLogicProfile());
			});
			var mapper = config.CreateMapper();
			builder.RegisterInstance(mapper).As<IMapper>();
		}
	}
}
