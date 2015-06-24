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
			Country originalCountry;
			Country newCountry;
			DataHelper.CreateCountries(out originalCountry, out newCountry);

			var entityDiffExtractor = new EntityDiffExtractor2();
			//var diff = entityDiffExtractor.Extract( 
			//	new CountryAggregate(existingCountry), 
			//	new CountryAggregate(nextCountry));
			var diff = entityDiffExtractor.Extract(newCountry, originalCountry);



			
			diff.WrappedChildren["Risks"].Where(r => r.PersistAction == PersistAction.ToAdd).Should().HaveCount(1);
			var adds = diff.WrappedChildren["Risks"].Where(r => r.PersistAction == PersistAction.ToAdd).ToList();
			adds[0].NewLocalEntity.ShouldBeEquivalentTo(newCountry.Risks[0]);

			diff.WrappedChildren["Risks"].Where(r => r.PersistAction == PersistAction.ToUpdate).Should().HaveCount(1);
			diff.WrappedChildren["WebSites"].Where(r => r.PersistAction == PersistAction.ToUpdate).Should().HaveCount(1);
			var updatesRisks = diff.WrappedChildren["Risks"].Where(r => r.PersistAction == PersistAction.ToUpdate).ToList();
			updatesRisks[0].NewLocalEntity.ShouldBeEquivalentTo(newCountry.Risks[1]);
			var updatesWeb = diff.WrappedChildren["WebSites"].Where(r => r.PersistAction == PersistAction.ToUpdate).ToList();
			updatesWeb[0].NewLocalEntity.ShouldBeEquivalentTo(newCountry.WebSites[0]);

			var deleteRisks = diff.WrappedChildren["Risks"].Where(r => r.PersistAction == PersistAction.ToDelete).ToList();
			deleteRisks.Should().HaveCount(1);
			deleteRisks[0].OriginalEntity.ShouldBeEquivalentTo(originalCountry.Risks[0]);


			var deleteWeb = diff.WrappedChildren["WebSites"].Where(r => r.PersistAction == PersistAction.ToDelete).ToList();
			deleteWeb.Should().HaveCount(1);
			deleteWeb[0].OriginalEntity.ShouldBeEquivalentTo(originalCountry.WebSites[0]);

			var updateRisks = diff.WrappedChildren["Risks"].Where(r => r.PersistAction == PersistAction.ToUpdate).ToList();
			updateRisks.Should().HaveCount(1);
			var addchildrenofrisk = updateRisks[0].WrappedChildren["ChildrenOfCountryRisk"].Where(r => r.PersistAction == PersistAction.ToAdd);
			addchildrenofrisk.Should().HaveCount(1);
			var updatechildrenofrisk = updateRisks[0].WrappedChildren["ChildrenOfCountryRisk"].Where(r => r.PersistAction == PersistAction.ToUpdate).ToList();
			updatechildrenofrisk.Should().HaveCount(1);

			var updatechildrenofchildrenofrisk = updatechildrenofrisk[0].WrappedChildren["ChildrenOfChildrenOfCountryRisk"].Where(r => r.PersistAction == PersistAction.ToUpdate);
			updatechildrenofchildrenofrisk.Should().HaveCount(1);
			var newchildrenofchildrenofrisk = updatechildrenofrisk[0].WrappedChildren["ChildrenOfChildrenOfCountryRisk"].Where(r => r.PersistAction == PersistAction.ToAdd);
			newchildrenofchildrenofrisk.Should().HaveCount(1);
			var dwletechildrenofchildrenofrisk = updatechildrenofrisk[0].WrappedChildren["ChildrenOfChildrenOfCountryRisk"].Where(r => r.PersistAction == PersistAction.ToDelete);
			dwletechildrenofchildrenofrisk.Should().HaveCount(2);

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
				nextCountry,existingCountry,
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