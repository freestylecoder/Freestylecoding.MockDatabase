using System;
using System.Collections.Generic;
using System.Text;

namespace Freestylecoding.MockableDatabase.Tests {
	public class TestEntity : IEquatable<TestEntity> {
		public readonly int Id;
		public readonly string Value;

		public TestEntity (
			int id = default,
			string value = default
		) {
			this.Id = id;
			this.Value = value;
		}

		public TestEntity( TestEntity copy )
			: this( copy.Id, copy.Value ) { }

		public TestEntity WithId( int id ) =>
			new TestEntity( id, this.Value );
		public TestEntity WithValue( string value ) =>
			new TestEntity( this.Id, value );

		public override bool Equals( object obj ) {
			if( obj is TestEntity that )
				return this.Equals( that );

			return base.Equals( obj );
		}

		private int? _hash = null;
		private const int _bigPrime = 25673;
		private const int _littlePrime = 5563;
		public override int GetHashCode() {
			Func<object, int> SafeHashCode = ( obj ) =>
				obj is object ish
				? ish.GetHashCode()
				: 0;

			if( !_hash.HasValue ) {
				unchecked {
					_hash = _bigPrime;

					_hash = _hash * _littlePrime + SafeHashCode( this.Id );
					_hash = _hash * _littlePrime + SafeHashCode( this.Value );
				}
			}

			return _hash.Value;
		}

		public override string ToString() =>
			Value;

		public bool Equals( TestEntity that ) {
			if( ReferenceEquals( that, null ) )
				return false;

			return
				ReferenceEquals( this, that )
				|| (
					this.GetHashCode() == that.GetHashCode()
					&& this.Id == that.Id
					&& this.Value == that.Value
				);
		}

		public static bool operator ==( TestEntity left, TestEntity right ) =>
			ReferenceEquals( left, null )
				? ReferenceEquals( right, null )
				: left.Equals( right );

		public static bool operator !=( TestEntity left, TestEntity right ) =>
			!( left == right );
	}
}