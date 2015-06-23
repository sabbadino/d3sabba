using System.Linq;
using Geographic.Countries.ServiceModel.Types;

namespace EntityDiffSample
{
	public class CountryService2
	{
		private readonly ICountryRepository2Read _countryRepositoryRead;
		private readonly ICountryRepository2Write _countryRepository2Write;

		public CountryService2(ICountryRepository2Read countryRepositoryRead, ICountryRepository2Write countryRepository2Write)
		{
			_countryRepositoryRead = countryRepositoryRead;
			_countryRepository2Write = countryRepository2Write;
		}

		public void Update(Country country)
		{
			var aggregate = new CountryAggregate2(country);
			var brokenRules = aggregate.CanUpdate(_countryRepositoryRead);
			if (!brokenRules.Any())
			{
				_countryRepository2Write.Persist(aggregate);
			}
		}
	}
}