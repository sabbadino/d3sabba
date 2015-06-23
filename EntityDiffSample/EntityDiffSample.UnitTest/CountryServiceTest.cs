using Geographic.Countries.ServiceModel.Types;
using Moq;
using NUnit.Framework;

namespace EntityDiffSample.UnitTest
{
	[TestFixture]
	public class CountryServiceTest
	{
		[Test]
		public void UpdateCountry()
		{
			Country existingCountry;
			Country nextCountry;
			DataHelper.CreateCountries(out existingCountry, out nextCountry);

			var countryRepository = new Mock<ICountryRepository>();
			countryRepository
				.Setup(x => x.Find(existingCountry.RefId.Id))
				.Returns(new CountryAggregate(existingCountry));

			var sut = new CountryService(countryRepository.Object);
			sut.Update(nextCountry);

			countryRepository.Verify(m => m.Find(nextCountry.RefId.Id));
			countryRepository.Verify(m => m.FindByCode(nextCountry.Code));
			countryRepository.Verify(m => m.Update(It.Is<CountryAggregate>(x => x.Data == nextCountry)));
		}
	}
}