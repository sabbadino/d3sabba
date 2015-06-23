using System.Linq;
using Geographic.Countries.ServiceModel.Types;

namespace EntityDiffSample
{
	public class CountryService
	{
		private readonly ICountryRepository _countryRepository;

		public CountryService(ICountryRepository countryRepository)
		{
			_countryRepository = countryRepository;
		}

		public void Update(Country country)
		{
			var aggregate = new CountryAggregate(country);
			var brokenRules = aggregate.CanUpdate(_countryRepository);
			if (!brokenRules.Any())
			{
				_countryRepository.Update(aggregate);
			}
		}
	}
}