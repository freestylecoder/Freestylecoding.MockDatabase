using System.Data;
using Xunit;

namespace Freestylecoding.MockDatabase.Test {
	public class MockCommandTests {
		private readonly MockConnection Connection;
		private readonly MockDatabase Database;
		private readonly MockCommand Command;

		public MockCommandTests() {
			this.Database = new MockDatabase();
			this.Connection = new MockConnection( Database );
			this.Command = Connection.CreateCommand() as MockCommand;
		}

		[Fact]
		public void Defaults() {
			Assert.Empty( Command.CommandText );
			Assert.Equal( 0, Command.CommandTimeout );
			Assert.Equal( CommandType.Text, Command.CommandType );
			Assert.Equal( Connection, Command.Connection );
			Assert.True( Command.DesignTimeVisible );
			Assert.NotNull( Command.Parameters );
			Assert.IsType<MockParameterCollection>( Command.Parameters );
			Assert.Null( Command.Site );
			Assert.Null( Command.Transaction );
			Assert.Equal( UpdateRowSource.None, Command.UpdatedRowSource );
		}
	}
}
