using System;
using System.Collections.Generic;
using EntityDiffSample.sabba;
using Geographic.Countries.ServiceModel.Types;

namespace EntityDiffSample
{

	public interface ICountryRepository2Read : IRepository2REad<CountryAggregate2>
	{
		CountryAggregate2 FindByCode(string code);
	}

	public interface ICountryRepository2Write : IRepository2Write<CountryAggregate2>
	{

		void Add(Country parentCountry);
		void Update(Country parentCountry);
		void Delete(Country parentCountry);
		

		void Add(CountryWebSite localEntity, Country parentCountry);
		void Update(CountryWebSite localEntity);
		void Delete(CountryWebSite localEntity);

		void Add(CountryRisk localEntity, Country parentCountry);
		void Update(CountryRisk localEntity);
		void Delete(CountryRisk localEntity);

		void Add(ChildrenOfCountryRisk localEntity, CountryRisk parentCountry);
		void Update(ChildrenOfCountryRisk  localEntity);
		void Delete(ChildrenOfCountryRisk  localEntity);

		void Add(ChildrenOfChildrenOfCountryRisk localEntity, ChildrenOfCountryRisk parentCountry);
		void Update(ChildrenOfChildrenOfCountryRisk localEntity);
		void Delete(ChildrenOfChildrenOfCountryRisk localEntity);


	}

	public class CountryRepository2Read : ICountryRepository2Read
	{
		public CountryRepository2Read()
		{
		}

		public CountryAggregate2 FindByCode(string code)
		{
			throw new NotImplementedException();
		}

		public CountryAggregate2 Find(string key)
		{
			// we could use a per-request storage (constructor-dependency) 
			// to avoid multiple queries within the same request.
			throw new NotImplementedException();
		}


		public CountryAggregate2 FindByIso3(string isocode)
		{
			throw new NotImplementedException();
		}

	}

	public class CountryRepository2Write   : ICountryRepository2Write
	{
		private readonly IEntityDiffExtractor2 _entityDiffExtractor;
		private readonly ICountryRepository2Read _iCountryRepository2Read;
		public CountryRepository2Write(ICountryRepository2Read countryRepository2Read,IEntityDiffExtractor2 entityDiffExtractor)
		{
			_iCountryRepository2Read = countryRepository2Read;
			_entityDiffExtractor = entityDiffExtractor;
		}

		

		public void Persist(CountryAggregate2 aggregate)
		{
			var currentVersion = _iCountryRepository2Read.Find(aggregate.Key);
			//_entityDiffExtractor.Persist(aggregate.Current, aggregate.Data, this);
			_entityDiffExtractor.Persist(currentVersion.Data, aggregate.Data, this);
		}


		public void Add(Country country)
		{

		}

		public void Update(Country country)
		{

		}

		public void Delete(Country country)
		{

		}

		public void Add(CountryRisk localEntity , Country parentCountry )
		{
			
		}

		public void Update(CountryRisk localEntity)
		{

		}

		public void Delete(CountryRisk localEntity)
		{

		}



		public void Add(ChildrenOfCountryRisk localEntity, CountryRisk parentCountryRisk)
		{

		}

		public void Update(ChildrenOfCountryRisk  localEntity)
		{

		}

		public void Delete(ChildrenOfCountryRisk localEntity)
		{

		}


		public void Add(ChildrenOfChildrenOfCountryRisk localEntity, ChildrenOfCountryRisk parentChildrenOfCountryRisk)
		{

		}

		public void Update(ChildrenOfChildrenOfCountryRisk localEntity)
		{

		}

		public void Delete(ChildrenOfChildrenOfCountryRisk localEntity)
		{

		}




		public void Add(CountryWebSite localEntity , Country parentCountry)
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
