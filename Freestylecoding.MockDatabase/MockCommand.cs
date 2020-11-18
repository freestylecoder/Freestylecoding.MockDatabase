using System.Data;
using System.Data.Common;
using System.Linq;

namespace Freestylecoding.MockDatabase {
	public class MockCommand : DbCommand {
		internal MockCommand( MockConnection connection ) : base() {
			this.DbConnection = connection;
			this.MockConnection = connection;

			this.CommandText = string.Empty;
			this.CommandType = CommandType.Text;
			this.DesignTimeVisible = true;
		}

		internal MockConnection MockConnection { get; private set; }

		public override string CommandText { get; set; }
		public override int CommandTimeout { get; set; }
		public override bool DesignTimeVisible { get; set; }
		public override CommandType CommandType { get; set; }
		protected override DbConnection DbConnection { get; set; }
		protected override DbTransaction DbTransaction { get; set; }
		public override UpdateRowSource UpdatedRowSource { get; set; }

		public readonly MockParameterCollection ParameterCollection = new MockParameterCollection();
		protected override DbParameterCollection DbParameterCollection =>
			ParameterCollection;

		public override void Cancel() { return; }
		public override int ExecuteNonQuery() {
			if( MockConnection.Results.Any() ) {
				MockDataReader r = new MockDataReader( this, MockConnection.Results.Dequeue() );
				if( !r.Read() )	return 0;
				return r.GetInt32( 0 );
			}

			return 0;
		}
		public override object ExecuteScalar() {
			if( MockConnection.Results.Any() ) {
				MockDataReader r = new MockDataReader( this, MockConnection.Results.Dequeue() );
				if( !r.Read() )	return null;
				return r.GetValue( 0 );
			}

			return null;
		}
		public override void Prepare() { return; }
		protected override DbParameter CreateDbParameter() =>
			new MockParameter();
		protected override DbDataReader ExecuteDbDataReader( CommandBehavior behavior ) =>
			new MockDataReader( this, MockConnection.Results.Dequeue() );
	}
}
