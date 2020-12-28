using System.Threading;
using System.Threading.Tasks;

namespace Freestylecoding.MockableDatabase {
	public interface IInsertable<TEntity> {
		TEntity BlockingInsert( TEntity newItem );
		Task<TEntity> Insert( TEntity newItem, CancellationToken token = default );
	}
}
