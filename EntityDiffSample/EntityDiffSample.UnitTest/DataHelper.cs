using System.Collections.Generic;
using Geographic.Countries.ServiceModel.Types;

namespace EntityDiffSample.UnitTest
{
	public static class DataHelper
	{
		public static void CreateCountries(out Country existingCountry, out Country nextCountry)
		{
			existingCountry = new Country();
			existingCountry.RefId = new CountryRef {Id = "1"};
			existingCountry.Risks = new List<CountryRisk>
			{
				new CountryRisk {LocalId = "1"},
				new CountryRisk {LocalId = "2"}
			};
			existingCountry.WebSites = new List<CountryWebSite>
			{
				new CountryWebSite {LocalId = "3"},
				new CountryWebSite {LocalId = "5"}
			};

			nextCountry = new Country();
			nextCountry.RefId = new CountryRef {Id = "1"};
			nextCountry.Risks = new List<CountryRisk>
			{
				new CountryRisk {LocalId = Constants.NEW_ENTITY_ID},
				new CountryRisk {LocalId = "2"}
			};
			nextCountry.WebSites = new List<CountryWebSite>
			{
				new CountryWebSite {LocalId = "5"}
			};
		}
	}
}