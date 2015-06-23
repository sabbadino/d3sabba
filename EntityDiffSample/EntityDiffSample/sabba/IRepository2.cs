using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityDiffSample.sabba
{
	public interface IRepository2REad<TAggregate>
		where TAggregate : IAggregateRoot
	{
		TAggregate Find(string key);
	}

	public interface IRepository2Write<TAggregate>
		where TAggregate : IAggregateRoot
	{
		void Persist(TAggregate aggregate);
	}



}
