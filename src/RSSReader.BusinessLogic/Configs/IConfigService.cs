using System;
using System.Collections.Generic;
using System.Text;

namespace RSSReader.BusinessLogic.Configs
{
	public interface IConfigService
	{
		Config GetConfig();
		Config UpdateCongid(Config config);
	}
}
