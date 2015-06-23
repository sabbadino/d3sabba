using System.Linq;
using EntityDiffSample.sabba;
using FluentAssertions;
using Geographic.Countries.ServiceModel.Types;
using Moq;
using NUnit.Framework;

namespace EntityDiffSample.UnitTest
{
	[TestFixture]
	public class EntityDiffExtractorTest2
	{
		[Test]
		public void GetAggregateChildreenEntititiesDiff()
		{
			Country existingCountry;
			Country nextCountry;
			DataHelper.CreateCountries(out existingCountry, out nextCountry);

			var entityDiffExtractor = new EntityDiffExtractor2();
			//var diff = entityDiffExtractor.Extract( 
			//	new CountryAggregate(existingCountry), 
			//	new CountryAggregate(nextCountry));
			var diff = entityDiffExtractor.Extract(
				existingCountry,
				nextCountry);


			
			diff.ChildEntities["Risks"].Where(r => r.PersistAction == PersistAction.ToAdd).Should().HaveCount(1);
			var adds = diff.ChildEntities["Risks"].Where(r => r.PersistAction == PersistAction.ToAdd).ToList();
			adds[0].NewLocalEntity.ShouldBeEquivalentTo(nextCountry.Risks[0]);

			diff.ChildEntities["Risks"].Where(r => r.PersistAction == PersistAction.ToUpdate).Should().HaveCount(1);
			diff.ChildEntities["WebSites"].Where(r => r.PersistAction == PersistAction.ToUpdate).Should().HaveCount(1);
			var updatesRisks = diff.ChildEntities["Risks"].Where(r => r.PersistAction == PersistAction.ToUpdate).ToList();
			updatesRisks[0].NewLocalEntity.ShouldBeEquivalentTo(nextCountry.Risks[1]);
			var updatesWeb = diff.ChildEntities["WebSites"].Where(r => r.PersistAction == PersistAction.ToUpdate).ToList();
			updatesWeb[0].NewLocalEntity.ShouldBeEquivalentTo(nextCountry.WebSites[0]);

			var deleteRisks = diff.ChildEntities["Risks"].Where(r => r.PersistAction == PersistAction.ToDelete).ToList();
			var deleteWeb = diff.ChildEntities["WebSites"].Where(r => r.PersistAction == PersistAction.ToDelete).ToList();

			deleteRisks.Should().HaveCount(1);
			deleteWeb.Should().HaveCount(1);
			deleteRisks[0].ExistingLocalEntity.ShouldBeEquivalentTo(existingCountry.Risks[0]);
			deleteWeb[0].ExistingLocalEntity.ShouldBeEquivalentTo(existingCountry.WebSites[0]);
		}

		[Test]
		public void PersistAggregateChildreenEntities()
		{
			Country existingCountry;
			Country nextCountry;
			DataHelper.CreateCountries(out existingCountry, out nextCountry);

			var entityDiffExtractor = new EntityDiffExtractor2();
			var repository = new Mock<ICountryRepository2Write>();
			
			entityDiffExtractor.Persist(
				existingCountry,
				nextCountry,
				repository.Object);

			repository.Verify(x => x.Update(It.IsAny<Country>()), Times.Once);

			repository.Verify(x => x.Add(It.IsAny<CountryRisk>(), It.IsAny<Country>()), Times.Once);
			repository.Verify(x => x.Update(It.IsAny<CountryRisk>()), Times.Once);
			repository.Verify(x => x.Update(It.IsAny<CountryWebSite>()), Times.Once);
			repository.Verify(x => x.Delete(It.IsAny<CountryRisk>()), Times.Once);
			repository.Verify(x => x.Delete(It.IsAny<CountryWebSite>()), Times.Exactly(1));
		}
	}
}