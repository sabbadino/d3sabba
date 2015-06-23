using System;
using System.Collections.Generic;
using Geographic.Countries.ServiceModel.Types;

namespace EntityDiffSample
{
	public interface ICountryRepository : IRepository<CountryAggregate>
	{
		CountryAggregate FindByCode(string code);

		void Add(CountryWebSite localEntity);
		void Update(CountryWebSite localEntity);
		void Delete(CountryWebSite localEntity);

		void Add(CountryRisk localEntity);
		void Update(CountryRisk localEntity);
		void Delete(CountryRisk localEntity);
	}

	public class CountryRepository : ICountryRepository
	{
		private readonly IEntityDiffExtractor _entityDiffExtractor;

		public CountryRepository(IEntityDiffExtractor entityDiffExtractor)
		{
			_entityDiffExtractor = entityDiffExtractor;
		}

		public CountryAggregate Find(string key)
		{
			// we could use a per-request storage (constructor-dependency) 
			// to avoid multiple queries within the same request.
			throw new NotImplementedException();
		}

		public CountryAggregate FindByCode(string code)
		{
			throw new NotImplementedException();
		}

		public CountryAggregate FindByIso3(string isocode)
		{
			throw new NotImplementedException();
		}

		public void Add(CountryAggregate aggregate)
		{
			throw new NotImplementedException();
		}

		public void Update(CountryAggregate aggregate)
		{
			var currentVersion = Find(aggregate.Key);
			//_entityDiffExtractor.Persist(aggregate.Current, aggregate.Data, this);
			_entityDiffExtractor.Persist(currentVersion.Data, aggregate.Data, this);
		}

		public void Add(CountryRisk localEntity)
		{
			
		}

		public void Update(CountryRisk localEntity)
		{

		}

		public void Delete(CountryRisk localEntity)
		{

		}

		public void Add(CountryWebSite localEntity)
		{

		}

		public void Update(CountryWebSite localEntity)
		{

		}

		public void Delete(CountryWebSite localEntity)
		{

		}
	}
}
