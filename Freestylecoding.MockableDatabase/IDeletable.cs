using System.Threading;
using System.Threading.Tasks;

namespace Freestylecoding.MockableDatabase {
	public interface IDeletable<TPrimaryKey> {
		bool BlockingDelete( TPrimaryKey key );
		Task<bool> Delete( TPrimaryKey key, CancellationToken token = default );
	}
}
