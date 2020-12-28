using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Freestylecoding.MockableDatabase {
	public interface ISelectable<TEntity, TPrimaryKey> {
		TEntity BlockingSelect( TPrimaryKey key );
		IEnumerable<TEntity> BlockingSelect( IEnumerable<TPrimaryKey> keys );
		IEnumerable<TEntity> BlockingSelect();

		Task<TEntity> Select( TPrimaryKey key, CancellationToken token = default );
		Task<IEnumerable<TEntity>> Select( IEnumerable<TPrimaryKey> keys, CancellationToken token = default );
		Task<IEnumerable<TEntity>> Select( CancellationToken token = default );
	}
}
