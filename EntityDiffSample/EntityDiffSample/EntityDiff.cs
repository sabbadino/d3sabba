using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EntityDiffSample
{
	public class EntityDiff
	{
		public IList<ILocalEntity> ToBeAdded { get; set; }
		public IList<ILocalEntity> ToBeUpdated { get; set; }
		public IList<ILocalEntity> ToBeDeleted { get; set; }

		public EntityDiff()
		{
			ToBeAdded = new List<ILocalEntity>();
			ToBeUpdated = new List<ILocalEntity>();
			ToBeDeleted = new List<ILocalEntity>();
		}
	}

	public interface IEntityDiffExtractor
	{
		EntityDiff Extract<T>(T original, T nextVersion)
			where T : class ;
		void Persist<T>(T original, T nextVersion, object repository)
			where T : class ;
	}

	public class EntityDiffExtractor : IEntityDiffExtractor
	{
		public EntityDiff Extract<T>(
			T original, 
			T nextVersion)
			where T : class
		{
			var entityDiff = new EntityDiff();

			foreach (PropertyInfo p in original.GetType().GetProperties())
			{
				// TOODO: how to check somenthing like typeof(IList<ILocalEntity>).IsAssignableFrom(p.PropertyType)
				if (typeof(IList).IsAssignableFrom(p.PropertyType))
				{
					//TODO: make recursive in case some local entity may have a property of type IList<ILocalEntity>

					var originalList = ((IList)p.GetValue(original, null))
						.Cast<ILocalEntity>();
					//var originalList = p.GetValue(p, null) as IEnumerable<ILocalEntity>;
					var nextList = ((IList)nextVersion.GetType().GetProperty(p.Name).GetValue(nextVersion, null))
						.Cast<ILocalEntity>();

					var originalIds = originalList.Select(x => x.LocalId).ToList();
					var nextIds = nextList.Select(x => x.LocalId).ToList();

					var toDelete = originalIds.Except(nextIds);
					var toUpdate = originalIds.Intersect(nextIds);

					foreach (var x in originalList.Where(x => toDelete.Contains(x.LocalId)))
					{
						entityDiff.ToBeDeleted.Add(x);
					}

					foreach (var x in nextList.Where(x => toUpdate.Contains(x.LocalId)))
					{
						entityDiff.ToBeUpdated.Add(x);
					}

					foreach (var x in nextList.Where(x =>x.LocalId==Constants.NEW_ENTITY_ID))
					{
						entityDiff.ToBeAdded.Add(x);
					}
					
				}
			}

			return entityDiff;
		}

		public void Persist<T>(
			T original,
			T nextVersion,
			object repository)
			where T : class 
		{
			var diff = Extract(original, nextVersion);

			foreach (var localEntity in diff.ToBeAdded)
			{
				((dynamic)repository).Add(((dynamic)localEntity));
			}

			foreach (var localEntity in diff.ToBeUpdated)
			{
				((dynamic)repository).Update(((dynamic)localEntity));
			}

			foreach (var localEntity in diff.ToBeDeleted)
			{
				((dynamic)repository).Delete(((dynamic)localEntity));
			}
		}
	}
}