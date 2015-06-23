using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityDiffSample.sabba
{
	public interface IEntityDiffExtractor2
	{
		AggregateRootWrapper<T> Extract<T>(T original, T newVersion)
			where T : class;
		void Persist<T>(T original, T newVersion, object repository)
			where T : class;
	}

	public class EntityDiffExtractor2 : IEntityDiffExtractor2
	{
		public AggregateRootWrapper<T> Extract<T>(T originalAr, T newAr) where T : class
		{
			//wrap the AR
			var aggregateRootWrapper = new AggregateRootWrapper<T>(newAr, originalAr);
			var currentWrapper = aggregateRootWrapper;
			var current = newAr;

			//Search for children
			foreach (var p in currentWrapper.NewAaggregateRoot.GetType().GetProperties())
			{
				if (typeof(IList).IsAssignableFrom(p.PropertyType))
				{
					var localEntitiesWrapperList = new List<LocalEntityWrapper<T>>();
					var newChildren = ((IList)p.GetValue(currentWrapper.NewAaggregateRoot, null)).Cast<ILocalEntity>();
					var originalChildren = ((IList)p.GetValue(currentWrapper.ExistingAggregateRoot, null)).Cast<ILocalEntity>();
					foreach (var newChild in newChildren)
					{
						var originalChild = originalChildren.FirstOrDefault(o => o.LocalId == newChild.LocalId);
						//Wrap each child
						var localEntityWrapper = new LocalEntityWrapper<T>(newChild, originalChild, currentWrapper.NewAaggregateRoot);
						localEntitiesWrapperList.Add(localEntityWrapper);
						//recursive wrap childs of this entity
						wrapEntitiesRecursive(localEntityWrapper);
					}

					//items that are in original and not in new must be deleted
					var localIdsToDelete = originalChildren.Select(o => o.LocalId).Except(newChildren.Select(n => n.LocalId)).ToList();
					originalChildren.Where(o => localIdsToDelete.Contains(o.LocalId)).ToList().ForEach(o =>
					{
						//create a wrapper where new is null and original is the one to be deleted
						var localEntityWrapper = new LocalEntityWrapper<T>(null, o, currentWrapper.NewAaggregateRoot);
						localEntitiesWrapperList.Add(localEntityWrapper);
					});
					// add the list of wrapped children to the element of the dictionary keyed with the property name
					aggregateRootWrapper.ChildEntities.Add(p.Name, localEntitiesWrapperList);
				}
			}
			return aggregateRootWrapper;
		}

		private void wrapEntitiesRecursive<T>(LocalEntityWrapper<T> current)
		{
			foreach (var p in current.NewLocalEntity.GetType().GetProperties())
			{
				if (typeof(IList).IsAssignableFrom(p.PropertyType))
				{
					var localEntitiesWrapperList = new List<LocalEntityWrapper<T>>();

					var newChildren = ((IList)p.GetValue(current.NewLocalEntity, null)).Cast<ILocalEntity>();
					var originalChildren = ((IList)p.GetValue(current.ExistingLocalEntity, null)).Cast<ILocalEntity>();

					foreach (var newChild in newChildren)
					{
						var originalVersionChild = originalChildren.FirstOrDefault(o => o.LocalId == newChild.LocalId);
						var localEntityWrapper = new LocalEntityWrapper<T>(newChild, originalVersionChild, current.NewLocalEntity);
						localEntitiesWrapperList.Add(localEntityWrapper);
						wrapEntitiesRecursive(localEntityWrapper);
					}

					var localIdsToDelete = originalChildren.Select(o => o.LocalId).Except(newChildren.Select(n => n.LocalId))
							.ToList();

					originalChildren.Where(o => localIdsToDelete.Contains(o.LocalId)).ToList().ForEach(o =>
					{
						var localEntityWrapper = new LocalEntityWrapper<T>(null, o, current.NewLocalEntity);
						localEntitiesWrapperList.Add(localEntityWrapper);
  					});

					current.ChildEntities.Add(p.Name, localEntitiesWrapperList);
				}
			}
		}


		public void Persist<T>(T original, T newVersion, object repository) where T : class
		{
			var aggregateRootWrapper = Extract<T>(original, newVersion);
			switch (aggregateRootWrapper.PersistAction)
			{
				case PersistAction.ToAdd:
					((dynamic)repository).Add(((dynamic)(aggregateRootWrapper.NewAaggregateRoot)));
					break;
				case PersistAction.ToUpdate:
					((dynamic)repository).Update(((dynamic)(aggregateRootWrapper.NewAaggregateRoot)));
					break;

			}

			foreach (var wrappedChildEntity in aggregateRootWrapper.ChildEntities.Values)
			{
				persistRecursive<T>(wrappedChildEntity, repository);
			}

			//delete of aggrgvate
			if (aggregateRootWrapper.PersistAction == PersistAction.ToDelete)
			{
				((dynamic)repository).Delete(((dynamic)(aggregateRootWrapper.ExistingAggregateRoot)));
			}
		}

		void persistRecursive<T>(List<LocalEntityWrapper<T>> wrappedEntities, object repository)
		{
			foreach (var wrappedEntity in wrappedEntities.Where(e => e.PersistAction != PersistAction.ToDelete))
			{
				switch (wrappedEntity.PersistAction)
				{
					case PersistAction.ToAdd:
						((dynamic)repository).Add(((dynamic)(wrappedEntity.NewLocalEntity)), ((dynamic)(wrappedEntity.ParentEntity)));
						break;
					case PersistAction.ToUpdate:
						((dynamic)repository).Update(((dynamic)(wrappedEntity.NewLocalEntity)));
						break;
					default:
						throw new Exception("UnExpected entity.PersistAction=" + wrappedEntity.PersistAction);
				}
			}
			foreach (var entity in wrappedEntities)
			{
				foreach (var childEntity in entity.ChildEntities.Values)
				{
					persistRecursive(childEntity, repository);
				}
			}
			//and now the delete
			foreach (var wrappedEntity in wrappedEntities.Where(e => e.PersistAction == PersistAction.ToDelete))
			{
				((dynamic)repository).Delete(((dynamic)(wrappedEntity.ExistingLocalEntity)));
			}
		}

	}


}
