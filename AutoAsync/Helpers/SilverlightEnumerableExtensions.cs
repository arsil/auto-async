using System;
using System.Collections.Generic;

namespace AutoAsync.Helpers
{
	/// <summary>
	/// Helper class providing missing extension methods to Silverlight environment
	/// </summary>
	static class SilverlightEnumerableExtensions
	{
		public static int FindIndex<T>(this IEnumerable<T> col, Predicate<T> predicate)
		{
			int index = 0;
			foreach (var t in col)
			{
				if (predicate(t))
					return index;

				index++;
			}
			return -1;
		}
	}
}
