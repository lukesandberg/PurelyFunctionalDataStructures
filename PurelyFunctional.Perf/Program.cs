using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PurelyFunctional;

namespace PurelyFunctional.Perf
{
	class Program
	{
		static void Main(string[] args)
		{
			var q = Queue.NewRT<int>();
			q = q.Enqueue(1).Enqueue(2).Enqueue(3);

		}
	}
}
