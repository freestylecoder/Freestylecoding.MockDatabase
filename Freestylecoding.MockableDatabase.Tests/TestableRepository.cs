using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Freestylecoding.MockableDatabase.Tests {
	internal class TestableRepository : Repository<TestEntity, int> {
		public bool SelectAllShoudThrow = false;
		public readonly IDictionary<int,TestEntity> Data = new Dictionary<int,TestEntity>();

		public override async Task<TestEntity> Select( int key, CancellationToken token = default ) {
			await Task.Delay( 100 );
			if( EvilValueException.EvilValue == key ) throw new EvilValueException();
			return Data.ContainsKey( key ) ? Data[key] : null;
		}
		public override async Task<IEnumerable<TestEntity>> Select( IEnumerable<int> keys, CancellationToken token = default ) {
			await Task.Delay( 100 );
			if( null == keys ) keys = Enumerable.Empty<int>();
			if( keys.Contains( EvilValueException.EvilValue ) ) throw new EvilValueException();
			return Data.Values.Where( o => keys.Contains( o.Id ) );
		}
		public override async Task<IEnumerable<TestEntity>> Select( CancellationToken token = default ) {
			await Task.Delay( 100 );
			if( SelectAllShoudThrow ) throw new EvilValueException();
			return Data.Values.OrderBy( o => o.Id );
		}
		public override async Task<TestEntity> Insert( TestEntity newItem, CancellationToken token = default ) {
			await Task.Delay( 100 );
			Data.Add( newItem.Id, newItem );
			return newItem;
		}
		public override async Task<TestEntity> Update( TestEntity newItem, CancellationToken token = default ) {
			await Task.Delay( 100 );
			Data[newItem.Id] = newItem;
			return newItem;
		}
		public override async Task<bool> Delete( int key, CancellationToken token = default ) {
			await Task.Delay( 100 );
			if( EvilValueException.EvilValue == key ) throw new EvilValueException();
			return Data.Remove( key );
		}
	}
}
