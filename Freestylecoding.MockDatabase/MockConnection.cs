using System;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;

namespace Freestylecoding.MockDatabase {
	public class MockConnection : DbConnection {
		public MockConnection( MockDatabase database )
			: this( database, string.Empty ) { }
		public MockConnection( MockDatabase database, string connectionString )
			: base() {
			if( null == database )
				throw new ArgumentNullException( nameof( database ) );

			this.ParentDatabase = database;
			this.ConnectionString = connectionString ?? string.Empty;
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
			ParentDatabase.commands.Add( new MockCommand( this ) );
			return ParentDatabase.commands.Last();
		}
		#endregion

		internal MockDatabase ParentDatabase;
	}
}
