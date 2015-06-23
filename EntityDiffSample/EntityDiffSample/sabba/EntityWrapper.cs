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

	public class LocalEntityWrapper<T>
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
		public ILocalEntity ExistingLocalEntity;
		public LocalEntityWrapper(ILocalEntity newLocalEntity, ILocalEntity existingLocalEntity, object parent)
		{
			if (newLocalEntity == null && existingLocalEntity == null) throw new Exception("newLocalEntity == null && existingLocalEntity==null");
			if (parent == null) throw new Exception("parent == null");
			ExistingLocalEntity = existingLocalEntity;
			NewLocalEntity = newLocalEntity;
			ParentEntity = parent;
			if (NewLocalEntity == null) return;
			if (ExistingLocalEntity == null && !NewLocalEntity.LocalId.IsNewIdMagicNumber()) throw new Exception("original localEntity not found but newEntity isn't marked as new");
		}
		public object ParentEntity;
		public Dictionary<string, List<LocalEntityWrapper<T>>> ChildEntities = new Dictionary<string, List<LocalEntityWrapper<T>>>();
	}

	public class AggregateRootWrapper<T>
	{
		public PersistAction PersistAction
		{
			get
			{
				if (NewAaggregateRoot == null)
					return PersistAction.ToDelete;
				else if (_newAaggregateRootId.IsNewIdMagicNumber())
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
			if (newAaggregateRoot == null) return;
			NewAaggregateRoot = newAaggregateRoot;
			ExistingAggregateRoot = existingAggregateRoot;
			_newAaggregateRootId = getAggregateRootId(NewAaggregateRoot);
			if (ExistingAggregateRoot == null && !_newAaggregateRootId.IsNewIdMagicNumber()) throw new Exception("original Aggregate not found but newAggregate isn't marked as new");

		}
		public Dictionary<string, List<LocalEntityWrapper<T>>> ChildEntities = new Dictionary<string, List<LocalEntityWrapper<T>>>();

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
