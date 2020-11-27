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
			//TODO: Need to see if parameter name is case insensitive
			list.Any( p => p.ParameterName.Equals( value ) );
		public override void CopyTo( Array array, int index ) =>
			list.CopyTo( array.OfType<DbParameter>().ToArray(), index );
		public override IEnumerator GetEnumerator() =>
			list.GetEnumerator();
		public override int IndexOf( object value ) =>
			list.IndexOf( value as DbParameter );
		public override int IndexOf( string parameterName ) =>
			//TODO: Need to see if parameter name is case insensitive
			IndexOf( list.FirstOrDefault( p => p.ParameterName.Equals( parameterName ) ) );

		public override void Insert( int index, object value ) =>
			list.Insert( index, value as DbParameter );
		public override void Remove( object value ) =>
			list.Remove( value as DbParameter );
		public override void RemoveAt( int index ) =>
			list.RemoveAt( index );

		// These are the ones that needed special treatment
		public override void RemoveAt( string parameterName ) =>
			list.RemoveAt( this.IndexOf( parameterName ) );
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
