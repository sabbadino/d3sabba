namespace EntityDiffSample
{
	public interface IRepository<TAggregate>
		where TAggregate : IAggregateRoot
	{
		TAggregate Find(string key);
		void Add(TAggregate aggregate);
		void Update(TAggregate aggregate);
	}
}