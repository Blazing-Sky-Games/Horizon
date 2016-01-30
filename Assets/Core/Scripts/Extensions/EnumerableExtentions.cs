using System;
using System.Collections.Generic;

namespace System.Linq
{
	public static class EnumerableExtentions
	{
		private static Random rand = new Random();

		public static IEnumerable<itemType> RandomOrder<itemType>(this IEnumerable<itemType> items)
		{
			return items.OrderBy(x => rand.NextDouble());
		}
	}
}

