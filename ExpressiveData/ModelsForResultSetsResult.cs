using System;
using System.Collections;
using System.Collections.Generic;

namespace ExpressiveData
{
	public class ModelsForResultSetsResult
	{
		private readonly IEnumerable[] _items;

		internal ModelsForResultSetsResult(IEnumerable[] items)
		{
			if (items == null)
				throw new ArgumentNullException("items");

			_items = items;
		}

		public IEnumerable<TModel> GetResult<TModel>(int index)
		{
			return (IEnumerable<TModel>) _items[index];
		}
	}
}
