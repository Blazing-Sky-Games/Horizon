using System;
namespace Horizon.Core.ExtensionMethods
{
	public static class DelegateExtensionMethods
	{
		public static void FireIfNotNull(this EventHandler<EventArgs> del, object sender, EventArgs e)
		{
			if(del != null) del(sender,e);
		}
	}
}

