using System;
using System.Collections;
using System.Collections.Generic;
using Geographic.Countries.ServiceModel.Types;

namespace EntityDiffSample
{
	public class CountryAggregate2 : IAggregateRoot
	 {
		 public string Key { get; private set; }
		 public Country Data { get; private set; }
		 //public Country Current { get; private set; }

		 public static CountryAggregate2 CreateNew()
	    {
			 var aggregate = new CountryAggregate2();
			 aggregate.Key = Guid.NewGuid().ToString();
			 return aggregate;
	    }

	    public CountryAggregate2(Country country)
	    {
		    Key = country.RefId.Id;
			 Data = country;
	    }

		 private CountryAggregate2()
		 {
		 }

		public IList<BrokenRule> CanUpdate(ICountryRepository2Read countryRepository)
		{
			//Current = countryRepository.Find(Key).Data;

			// Verify data consistency for the current aggregate itself

			// Verify against current version
			var aggregate1 = countryRepository.Find(Data.RefId.Id);

			// Verify field(s) uniqueness against other aggregates
			var aggregate2 = countryRepository.FindByCode(Data.Code);


			return new BrokenRule[0];
		}
	 }
}
