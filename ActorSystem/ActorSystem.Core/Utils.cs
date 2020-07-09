using System;
using System.Collections.Generic;

namespace ActorSystem.Core
{
	public static class Utils
	{
		public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
		{
			foreach (var item in source)
			{
				action(item);
			}
		}
	}
}
