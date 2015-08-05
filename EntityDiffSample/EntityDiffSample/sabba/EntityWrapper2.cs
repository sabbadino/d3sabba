using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;

namespace EntityDiffSample.sabba
{
	public enum PersistAction
	{
		NothingToDo, ToAdd, ToUpdate, ToDelete
	}

	public abstract class EntityWrapperBase
	{

		public Dictionary<string, List<LocalEntityWrapper>> WrappedChildren = new Dictionary<string, List<LocalEntityWrapper>>();
	}

	public class LocalEntityWrapper : EntityWrapperBase
	{
		public PersistAction PersistAction 
		{
			get
			{
				if (NewLocalEntity==null) 
					return PersistAction.ToDelete;
				else if(NewLocalEntity.LocalId.IsNewIdMagicNumber())
					return PersistAction.ToAdd;
				else 
					return PersistAction.ToUpdate;
			}
		}
		public ILocalEntity NewLocalEntity;
		public ILocalEntity OriginalEntity;
		public object NewParent;
		public object OriginalParent;
		public LocalEntityWrapper(ILocalEntity newLocalEntity, ILocalEntity originalEntity, object newParent, object originalParent)
		{
			if (newLocalEntity == null && originalEntity == null) throw new Exception("newLocalEntity == null && originalEntity==null");
			if (originalEntity == null && !newLocalEntity.LocalId.IsNewIdMagicNumber()) 
				throw new Exception("originalEntity not found, but newEntity isn't marked as new. The new entity has type " 
					+ newLocalEntity.GetType().Name + " and id=" + newLocalEntity.LocalId 
					+ ". Likely the entity has been deleted from the database");
			OriginalEntity = originalEntity;
			NewLocalEntity = newLocalEntity;
			NewParent = newParent;
			OriginalParent = originalParent;
			if (NewLocalEntity == null) return;
		}
		
		}

	public class AggregateRootWrapper<T> : EntityWrapperBase
	{
		public PersistAction PersistAction
		{
			get
			{
				//if (NewAaggregateRoot == null)
				//	return PersistAction.ToDelete;
				//else 
				if (_newAaggregateRootId.IsNewIdMagicNumber())
					return PersistAction.ToAdd;
				else
					return PersistAction.ToUpdate;
			}
		}
		public T NewAaggregateRoot;
		public T ExistingAggregateRoot;
		private string _newAaggregateRootId = "";
		public AggregateRootWrapper(T newAaggregateRoot, T existingAggregateRoot)
		{
			if (newAaggregateRoot == null) throw new Exception("newAaggregateRoot == null");
			NewAaggregateRoot = newAaggregateRoot;
			ExistingAggregateRoot = existingAggregateRoot;
			_newAaggregateRootId = getAggregateRootId(NewAaggregateRoot);
			if (ExistingAggregateRoot == null && !_newAaggregateRootId.IsNewIdMagicNumber()) 
				throw new Exception("originalAggregate not found, but newEntity isn't marked as new. The new Aggreagate has type " 
					+ NewAaggregateRoot.GetType().Name + " and id=" + _newAaggregateRootId 
					+ ". Likely the entity has been deleted from the database");
		}
	
		private string getAggregateRootId<T>(T nextVersion)
		{
			var refId = ((dynamic) nextVersion).RefId;
			if (refId == null) return "";
			return refId.Id;
		}
	}


	
	static class MyExtensions
	{
		internal const string NEW_ID_MAGIC_NUMBER1 = "NEW_ID";
		internal const string NEW_ID_MAGIC_NUMBER2 = null;

		internal static bool IsNewIdMagicNumber(this string value)
		{
			return value == NEW_ID_MAGIC_NUMBER1 || value == NEW_ID_MAGIC_NUMBER2;
		}
	}

}
