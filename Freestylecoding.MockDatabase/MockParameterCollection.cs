using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace Freestylecoding.MockDatabase {
	public class MockParameterCollection : DbParameterCollection {
		private readonly List<DbParameter> list = new List<DbParameter>();

		public override int Count =>
			list.Count;
		public override object SyncRoot =>
			( list as ICollection ).SyncRoot;

		public override int Add( object value ) {
			if( value is DbParameter param ) {
				list.Add( param );
				return list.Count - 1;
			}

			throw new InvalidCastException( "The DbParameterCollection only accepts non-null DbParameter type objects, not Object objects." );
		}
		public override void AddRange( Array values ) {
			if( !values.Cast<object>().All( o => o is DbParameter ) )
				throw new InvalidCastException( "The DbParameterCollection only accepts non-null DbParameter type objects, not Object objects." );

			IEnumerable<DbParameter> parameters = values.Cast<DbParameter>();
			if( values.Length != parameters.Distinct().Count() )
				throw new ArgumentException( "The DbParameter is already contained by another DbParameterCollection." );

			if( parameters.Any( p => this.Contains( p ) ) )
				throw new ArgumentException( "The DbParameter is already contained by another DbParameterCollection." );

			list.AddRange( parameters );
		}
		public override void Clear() =>
			list.Clear();

		public override bool Contains( object value ) =>
			list.Contains( value );
		public override bool Contains( string value ) =>
			list.Any( p => p.ParameterName.Equals( value, StringComparison.InvariantCultureIgnoreCase ) );
		public override void CopyTo( Array array, int index ) {
			if( !typeof( DbParameter ).IsAssignableFrom( array.GetType().GetElementType() ) )
				throw new ArgumentException( "Target array type is not compatible with the type of items in the collection." );

			list.CopyTo( array as DbParameter[], index );
		}
		public override IEnumerator GetEnumerator() =>
			list.GetEnumerator();
		public override int IndexOf( object value ) {
			if( null != value )
				if( !typeof( DbParameter ).IsAssignableFrom( value.GetType() ) )
					throw new InvalidCastException( "The DbParameterCollection only accepts non-null DbParameter type objects, not Object objects." );

			return list.IndexOf( value as DbParameter );
		}
		public override int IndexOf( string parameterName ) =>
			IndexOf( list.FirstOrDefault( p => p.ParameterName.Equals( parameterName, StringComparison.InvariantCultureIgnoreCase ) ) );

		public override void Insert( int index, object value ) {
			if( null == value )
				throw new ArgumentNullException( "value", "The DbParameterCollection only accepts non-null DbParameter type objects." );

			if( !typeof( DbParameter ).IsAssignableFrom( value.GetType() ) )
				throw new InvalidCastException( "The DbParameterCollection only accepts non-null DbParameter type objects, not Object objects." );

			if( list.Contains( value ) )
				throw new ArgumentException( "The DbParameter is already contained by another DbParameterCollection." );

			list.Insert( index, value as DbParameter );
		}
		public override void Remove( object value ) {
			if( null == value )
				throw new ArgumentNullException( "value", "The DbParameterCollection only accepts non-null DbParameter type objects." );

			if( !typeof( DbParameter ).IsAssignableFrom( value.GetType() ) )
				throw new InvalidCastException( "The DbParameterCollection only accepts non-null DbParameter type objects, not Object objects." );

			list.Remove( value as DbParameter );
		}
		public override void RemoveAt( int index ) {
			if( index < 0 || index >= list.Count )
				throw new IndexOutOfRangeException();

			list.RemoveAt( index );
		}

		// These are the ones that needed special treatment
		public override void RemoveAt( string parameterName ) =>
			this.RemoveAt( this.IndexOf( parameterName ) );
		protected override DbParameter GetParameter( int index ) =>
			list[index];
		protected override DbParameter GetParameter( string parameterName ) =>
			list.FirstOrDefault( p => p.ParameterName.Equals( parameterName ) );
		protected override void SetParameter( int index, DbParameter value ) =>
			list[index] = value;
		protected override void SetParameter( string parameterName, DbParameter value ) {
			int index = IndexOf( parameterName );
			if( -1 == index )
				list.Add( value );
			else
				SetParameter( index, value );
		}
	}
}
