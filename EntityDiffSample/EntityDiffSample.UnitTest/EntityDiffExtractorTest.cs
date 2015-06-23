using FluentAssertions;
using Geographic.Countries.ServiceModel.Types;
using Moq;
using NUnit.Framework;

namespace EntityDiffSample.UnitTest
{
	[TestFixture]
	public class EntityDiffExtractorTest
	{
		[Test]
		public void GetAggregateChildreenEntititiesDiff()
		{
			Country existingCountry;
			Country nextCountry;
			DataHelper.CreateCountries(out existingCountry, out nextCountry);

			var entityDiffExtractor = new EntityDiffExtractor();
			//var diff = entityDiffExtractor.Extract( 
			//	new CountryAggregate(existingCountry), 
			//	new CountryAggregate(nextCountry));
			var diff = entityDiffExtractor.Extract(
				existingCountry,
				nextCountry);

			diff.ToBeAdded.Should().HaveCount(1);
			var adds = diff.ToBeAdded;
			adds[0].ShouldBeEquivalentTo(nextCountry.Risks[0]);

			diff.ToBeUpdated.Should().HaveCount(2);
			var updates = diff.ToBeUpdated;
			updates[0].ShouldBeEquivalentTo(nextCountry.Risks[1]);
			updates[1].ShouldBeEquivalentTo(nextCountry.WebSites[0]);

			diff.ToBeDeleted.Should().HaveCount(2);
			var deletes = diff.ToBeDeleted;
			deletes[0].ShouldBeEquivalentTo(existingCountry.Risks[0]);
			deletes[1].ShouldBeEquivalentTo(existingCountry.WebSites[0]);
		}

		[Test]
		public void PersistAggregateChildreenEntities()
		{
			Country existingCountry;
			Country nextCountry;
			DataHelper.CreateCountries(out existingCountry, out nextCountry);

			var entityDiffExtractor = new EntityDiffExtractor();
			var repository = new Mock<ICountryRepository>();
			entityDiffExtractor.Persist(
				existingCountry,
				nextCountry,
				repository.Object);

			repository.Verify(x => x.Add(It.IsAny<CountryRisk>()), Times.Once);
			repository.Verify(x => x.Update(It.IsAny<CountryRisk>()), Times.Once);
			repository.Verify(x => x.Update(It.IsAny<CountryWebSite>()), Times.Once);
			repository.Verify(x => x.Delete(It.IsAny<CountryRisk>()), Times.Once);
			repository.Verify(x => x.Delete(It.IsAny<CountryWebSite>()), Times.Exactly(1));
		}
	}
}