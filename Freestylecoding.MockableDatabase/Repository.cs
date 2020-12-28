using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Freestylecoding.MockableDatabase {
	public abstract class Repository<TEntity, TPrimaryKey>
		: ISelectable<TEntity, TPrimaryKey>, IInsertable<TEntity>, IUpdatable<TEntity>, IDeletable<TPrimaryKey> {
		private TReturn BlockingCall<TReturn>( Func<Task<TReturn>> func ) {
			Task<TReturn> task = func();
			try {
				task.Wait();
				switch( task.Status ) {
					case TaskStatus.Faulted:
					case TaskStatus.Canceled:
						if( task.Exception is AggregateException )
							throw task.Exception.InnerException;
						else
							throw task.Exception;

					case TaskStatus.RanToCompletion:
					default:
						return task.Result;
				}
			} catch( AggregateException aex ) {
				throw aex.InnerException;
			}
		}

		#region ISelectable<TEntity, TPrimaryKey>
		public virtual TEntity BlockingSelect( TPrimaryKey key ) =>
			BlockingCall( () => Select( key ) );
		public virtual IEnumerable<TEntity> BlockingSelect( IEnumerable<TPrimaryKey> keys ) =>
			BlockingCall( () => Select( keys ) );
		public virtual IEnumerable<TEntity> BlockingSelect() =>
			BlockingCall( () => Select() );
		public abstract Task<TEntity> Select( TPrimaryKey key, CancellationToken token = default );
		public abstract Task<IEnumerable<TEntity>> Select( IEnumerable<TPrimaryKey> keys, CancellationToken token = default );
		public abstract Task<IEnumerable<TEntity>> Select( CancellationToken token = default );
		#endregion

		#region IInsertable<TEntity>
		public virtual TEntity BlockingInsert( TEntity newItem ) =>
			BlockingCall( () => Insert( newItem ) );
		public abstract Task<TEntity> Insert( TEntity newItem, CancellationToken token = default );
		#endregion

		#region IUpdatable<TEntity>
		public virtual TEntity BlockingUpdate( TEntity newItem ) =>
			BlockingCall( () => Update( newItem ) );
		public abstract Task<TEntity> Update( TEntity newItem, CancellationToken token = default );
		#endregion

		#region IDeletable<TPrimaryKey>
		public virtual bool BlockingDelete( TPrimaryKey key ) =>
			BlockingCall( () => Delete( key ) );
		public abstract Task<bool> Delete( TPrimaryKey key, CancellationToken token = default );
		#endregion
	}
}
