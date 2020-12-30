using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using Freestylecoding.MockableDatabase;

namespace Freestylecoding.MockDatabase {
	public class MockDatabase : IDatabase {
		public DbParameter CreateParameter( string parameterName, DbType dbType, object value ) => new MockParameter {
			ParameterName = parameterName,
			DbType = dbType,
			Value = value
		};
		public DbParameter CreateParameter( string parameterName, DbType dbType, int size, object value ) => new MockParameter {
			ParameterName = parameterName,
			DbType = dbType,
			Value = value,
			Size = size
		};
		public DbConnection GetConnection() => new MockConnection( this );

		internal List<MockCommand> commands = new List<MockCommand>();
		public IEnumerable<MockCommand> Commands => commands;

		internal Queue<DataTable> Results = new Queue<DataTable>();

		public void AddResult( DataTable table ) =>
			Results.Enqueue( table );

		/// <remarks>
		/// This is equivalent to calling AddResult( null );
		/// It is just more expressing in the test code.
		/// </remarks>
		public void AddEmptyResult() =>
			Results.Enqueue( new DataTable( "Empty" ) );

		public void AddScalarResult( object o ) =>
			AddResult(
				CreateDataTable(
					new[] { o },
					new[] { new DataColumn( "Scalar", o.GetType() ) },
					( d ) => new object[] { d }
				)
			);

		public void AddNonQueryResult( int count ) =>
			AddResult(
				CreateDataTable(
					new[] { count },
					new[] { new DataColumn( "RecordsAffected", count.GetType() ) },
					( d ) => new object[] { d }
				)
			);

		public void AddJsonResult( string json ) {
			List<string> chunks = new List<string>();
			while( !string.IsNullOrWhiteSpace( json ) ) {
				chunks.Add( json.Substring( 0, Math.Min( json.Length, 2000 ) ) );
				json = json.Substring( Math.Min( json.Length, 2000 ) );
			}
			AddResult(
				CreateDataTable(
					chunks,
					new[] { new DataColumn( "Json", typeof( string ) ) },
					( s ) => new object[] { s }
				)
			);
		}

		public void AddSqlException(
			string message = null,
			SqlError[] errorCollection = null,
			Exception innerException = null,
			Guid conId = default
		) =>
			this.Results.Enqueue(
				new SqlExceptionDataTable(
					message,
					errorCollection,
					innerException,
					conId
				)
			);

		public DataTable CreateDataTable<T>(
			IEnumerable<T> data,
			IEnumerable<DataColumn> columns,
			Func<T,object[]> func
		) {
			DataTable table = new DataTable();
			table.Columns.AddRange( columns.ToArray() );

			foreach( T d in data ) {
				DataRow row = table.NewRow();
				row.ItemArray = func( d );
				table.Rows.Add( row );
			}

			return table;
		}
	}
}
