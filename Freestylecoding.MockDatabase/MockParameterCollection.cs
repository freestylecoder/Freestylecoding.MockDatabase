using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;

namespace Freestylecoding.MockDatabase {
	public class MockParameterCollection : DbParameterCollection {
		private readonly ArrayList list = new ArrayList();
		private readonly Dictionary<string,int> names = new Dictionary<string, int>( StringComparer.InvariantCultureIgnoreCase );

		public override int Count => list.Count;
		public override object SyncRoot => list.SyncRoot;

		public override int Add( object value ) {
			int index = list.Add( value );
			if( value is DbParameter param )
				names.Add( param.ParameterName, index );
			return index;
		}
		public override void AddRange( Array values ) {
			foreach( object value in values ) {
				list.Add( value );
			}
		}
		public override void Clear() {
			list.Clear();
			names.Clear();
		}

		public override bool Contains( object value ) =>
			list.Contains( value );
		public override bool Contains( string value ) =>
			names.ContainsKey( value );
		public override void CopyTo( Array array, int index ) =>
			list.CopyTo( array, index );
		public override IEnumerator GetEnumerator() =>
			list.GetEnumerator();
		public override int IndexOf( object value ) =>
			list.IndexOf( value );
		public override int IndexOf( string parameterName ) =>
			names.ContainsKey( parameterName )
				? names[parameterName]
				: -1;

		public override void Insert( int index, object value ) {
			list.Insert( index, value );
			RebuildNames();
		}
		public override void Remove( object value ) {
			list.Remove( value );
			RebuildNames();
		}
		public override void RemoveAt( int index ) {
			list.RemoveAt( index );
			RebuildNames();
		}

		// These are the ones that needed special treatment
		public override void RemoveAt( string parameterName ) {
			if( names.ContainsKey( parameterName ) )
				list.RemoveAt( names[parameterName] );
		}
		protected override DbParameter GetParameter( int index ) =>
			list[index] as DbParameter;
		protected override DbParameter GetParameter( string parameterName ) =>
			names.ContainsKey( parameterName )
				? GetParameter( names[parameterName] )
				: null;
		protected override void SetParameter( int index, DbParameter value ) =>
			list[index] = value;
		protected override void SetParameter( string parameterName, DbParameter value ) {
			if( names.ContainsKey( parameterName ) )
				SetParameter( names[parameterName], value );
			else
				Add( value );
		}

		private void RebuildNames() {
			names.Clear();

			for( int index = 0; index < list.Count; ++index )
				if( list[index] is DbParameter param )
					names.Add( param.ParameterName, index );
		}
	}
}
