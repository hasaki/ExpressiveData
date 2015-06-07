using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressiveData
{
	public interface IOrdinalProvider
	{
		int GetOrdinal(string columnName);
	}
}
