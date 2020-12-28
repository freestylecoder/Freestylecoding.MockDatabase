using System.Threading;
using System.Threading.Tasks;

namespace Freestylecoding.MockableDatabase {
	public interface IUpdatable<TEntity> {
		TEntity BlockingUpdate( TEntity newItem );
		Task<TEntity> Update( TEntity newItem, CancellationToken token = default );
	}
}
