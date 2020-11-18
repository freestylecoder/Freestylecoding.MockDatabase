using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;

namespace Freestylecoding.MockDatabase {
	public class MockConnection : DbConnection {
		public MockConnection() : this( string.Empty ) { }

		public MockConnection( string connectionString ) : base() {
			this.ConnectionString = connectionString;
		}

		#region DbConnection Stuff
		public override string ConnectionString { get; set; }

		private string database = string.Empty;
		public override string Database => database;

		private string dataSource = string.Empty;
		public override string DataSource => dataSource;

		public override string ServerVersion =>
			Assembly
				.GetExecutingAssembly()
				.GetName()
				.Version
				.ToString();

		private ConnectionState state = ConnectionState.Closed;
		public override ConnectionState State => state;

		public override void ChangeDatabase( string databaseName ) =>
			database = databaseName;
		public override void Close() =>
			state = ConnectionState.Closed;			
		public override void Open() =>
			state = ConnectionState.Open;
		protected override DbTransaction BeginDbTransaction( IsolationLevel isolationLevel ) =>
			new MockTransaction( this, isolationLevel );
		protected override DbCommand CreateDbCommand() {
			commands.Add( new MockCommand( this ) );
			return commands.Last();
		}
		#endregion

		private List<MockCommand> commands = new List<MockCommand>();
		public IEnumerable<MockCommand> Commands => commands;

		internal Queue<DataTable> Results = new Queue<DataTable>();

		public void AddResult( DataTable table ) =>
			Results.Enqueue( table );

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
			List<string> chucks = new List<string>();
			while( string.IsNullOrWhiteSpace( json ) ) {
				chucks.Add( json.Substring( 0, Math.Min( json.Length, 2000 ) ) );
				json = json.Substring( Math.Min( json.Length, 2000 ) );
			}
			AddResult(
				CreateDataTable(
					chucks,
					new[] { new DataColumn( "Json", typeof( string ) ) },
					( s ) => new object[] { s }
				)
			);
		}

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
