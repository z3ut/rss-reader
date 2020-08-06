using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RSSReader.BusinessLogic.Updater
{
	public interface IFeedUpdater
	{
		Task UpdateFeedsAsync();
		Task UpdateFeedAsync(int channelId);
	}
}
