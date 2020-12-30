using System;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace Freestylecoding.MockDatabase {
	public class MockDataReader : DbDataReader {
		private DbDataReader ActiveReader;
		private readonly MockCommand Command;

		internal MockDataReader( MockCommand command, DataTable dataTable ) {
			if( dataTable is SqlExceptionDataTable ex )
				ex.Throw();

			this.Command = command;
			this.ActiveReader = ( dataTable ?? new DataTable() ).CreateDataReader();
		}

		public override object this[int ordinal] =>
			ActiveReader[ordinal];
		public override object this[string name] =>
			ActiveReader[name];
		public override int Depth =>
			ActiveReader.Depth;
		public override int FieldCount =>
			ActiveReader.FieldCount;
		public override bool HasRows =>
			ActiveReader.HasRows;
		public override bool IsClosed =>
			ActiveReader.IsClosed;
		public override int RecordsAffected =>
			ActiveReader.RecordsAffected;

		public override bool GetBoolean( int ordinal ) =>
			ActiveReader.GetBoolean( ordinal );
		public override byte GetByte( int ordinal ) =>
			ActiveReader.GetByte( ordinal );
		public override long GetBytes( int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length ) =>
			ActiveReader.GetBytes( ordinal, dataOffset, buffer, bufferOffset, length );
		public override char GetChar( int ordinal ) =>
			ActiveReader.GetChar( ordinal );
		public override long GetChars( int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length ) =>
			ActiveReader.GetChars( ordinal, dataOffset, buffer, bufferOffset, length );
		public override string GetDataTypeName( int ordinal ) =>
			ActiveReader.GetDataTypeName( ordinal );
		public override DateTime GetDateTime( int ordinal ) =>
			ActiveReader.GetDateTime( ordinal );
		public override decimal GetDecimal( int ordinal ) =>
			ActiveReader.GetDecimal( ordinal );
		public override double GetDouble( int ordinal ) =>
			ActiveReader.GetDouble( ordinal );
		public override IEnumerator GetEnumerator() =>
			ActiveReader.GetEnumerator();
		public override Type GetFieldType( int ordinal ) =>
			ActiveReader.GetFieldType( ordinal );
		public override float GetFloat( int ordinal ) =>
			ActiveReader.GetFloat( ordinal );
		public override Guid GetGuid( int ordinal ) =>
			ActiveReader.GetGuid( ordinal );
		public override short GetInt16( int ordinal ) =>
			ActiveReader.GetInt16( ordinal );
		public override int GetInt32( int ordinal ) =>
			ActiveReader.GetInt32( ordinal );
		public override long GetInt64( int ordinal ) =>
			ActiveReader.GetInt64( ordinal );
		public override string GetName( int ordinal ) =>
			ActiveReader.GetName( ordinal );
		public override int GetOrdinal( string name ) =>
			ActiveReader.GetOrdinal( name );
		public override string GetString( int ordinal ) =>
			ActiveReader.GetString( ordinal );
		public override object GetValue( int ordinal ) =>
			ActiveReader.GetValue( ordinal );
		public override int GetValues( object[] values ) =>
			ActiveReader.GetValues( values );
		public override bool IsDBNull( int ordinal ) =>
			ActiveReader.IsDBNull( ordinal );

		public override bool NextResult() {
			if( Command.MockConnection.ParentDatabase.Results.Any() ) {
				DataTable dataTable = Command.MockConnection.ParentDatabase.Results.Dequeue();
				if( dataTable is SqlExceptionDataTable ex )
					ex.Throw();

				this.ActiveReader = dataTable.CreateDataReader();
				return true;
			} else {
				ActiveReader = null;
				return false;
			}
		}
		public override bool Read() =>
			ActiveReader.Read();
	}
}
