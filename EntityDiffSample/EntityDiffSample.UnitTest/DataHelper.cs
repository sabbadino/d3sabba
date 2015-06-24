using System.Collections.Generic;
using Geographic.Countries.ServiceModel.Types;

namespace EntityDiffSample.UnitTest
{
	public static class DataHelper
	{
		public static void CreateCountries(out Country originalCountry, out Country newCountry)
		{
			originalCountry = new Country();
			originalCountry.RefId = new CountryRef {Id = "1"};
			originalCountry.Risks = new List<CountryRisk>
			{
				new CountryRisk {LocalId = "1"}, //Delete
				new CountryRisk {LocalId = "2"}
			};
			

			originalCountry.WebSites = new List<CountryWebSite>
			{
				new CountryWebSite {LocalId = "3"}, //delete
				new CountryWebSite {LocalId = "5"}
			};

			originalCountry.Risks[1].ChildrenOfCountryRisk = new List<ChildrenOfCountryRisk>
			{
				new ChildrenOfCountryRisk {LocalId = "1", ChildrenOfChildrenOfCountryRisk = new List<ChildrenOfChildrenOfCountryRisk>
				{
					new ChildrenOfChildrenOfCountryRisk() { LocalId = "1"},
					new ChildrenOfChildrenOfCountryRisk() { LocalId = "2"}, //delete
					new ChildrenOfChildrenOfCountryRisk() { LocalId = "3"} // delete
				}}
			};


			newCountry = new Country();
			newCountry.RefId = new CountryRef {Id = "1"};
			newCountry.Risks = new List<CountryRisk>
			{
				new CountryRisk {LocalId = Constants.NEW_ENTITY_ID},
				new CountryRisk {LocalId = "2"}
			};

			newCountry.Risks[1].ChildrenOfCountryRisk = new List<ChildrenOfCountryRisk>
			{
				new ChildrenOfCountryRisk {LocalId = "1", ChildrenOfChildrenOfCountryRisk = new List<ChildrenOfChildrenOfCountryRisk>
				{
					new ChildrenOfChildrenOfCountryRisk() { LocalId = "1"},
					new ChildrenOfChildrenOfCountryRisk() { LocalId = Constants.NEW_ENTITY_ID},
				}},
				new ChildrenOfCountryRisk {LocalId = Constants.NEW_ENTITY_ID, ChildrenOfChildrenOfCountryRisk = new List<ChildrenOfChildrenOfCountryRisk>
				{
					new ChildrenOfChildrenOfCountryRisk() { LocalId = Constants.NEW_ENTITY_ID},
					new ChildrenOfChildrenOfCountryRisk() { LocalId = Constants.NEW_ENTITY_ID},
				}}
			};



			newCountry.WebSites = new List<CountryWebSite>
			{
				new CountryWebSite {LocalId = "5"}
			};
		}
	}
}