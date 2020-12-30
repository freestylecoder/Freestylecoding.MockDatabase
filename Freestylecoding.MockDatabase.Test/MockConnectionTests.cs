using System;
using System.Data;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace Freestylecoding.MockDatabase.Test {
	public class MockConnectionTests {
		private readonly MockDatabase Database;

		public MockConnectionTests() {
			this.Database = new MockDatabase();
		}

		[Fact]
		public void Ctor() {
			MockConnection actual = new MockConnection( Database );

			Assert.Empty( actual.ConnectionString );
			Assert.Equal( 15, actual.ConnectionTimeout );
			Assert.Null( actual.Container );
			Assert.Empty( actual.Database );
			Assert.Empty( actual.DataSource );
			Assert.Equal( "0.9.0.0", actual.ServerVersion );
			Assert.Null( actual.Site );
			Assert.Equal( ConnectionState.Closed, actual.State );

			Assert.Equal(
				actual
					.GetType()
					.GetField( "ParentDatabase", BindingFlags.Instance | BindingFlags.NonPublic )
					.GetValue( actual ),
				Database
			);
		}

		[Fact]
		public void Ctor_NullDatabase() {
			Assert.IsType<ArgumentNullException>(
				Record.Exception( () =>
					new MockConnection( null )
				)
			);
		}

		[Fact]
		public void ConnectionStringCtor() {
			const string connectionString = "This is my Connection StRiNg";
			MockConnection actual = new MockConnection( Database, "This is my Connection StRiNg" );

			Assert.Equal( connectionString, actual.ConnectionString );
			Assert.Equal( 15, actual.ConnectionTimeout );
			Assert.Null( actual.Container );
			Assert.Empty( actual.Database );
			Assert.Empty( actual.DataSource );
			Assert.Equal( "0.9.0.0", actual.ServerVersion );
			Assert.Null( actual.Site );
			Assert.Equal( ConnectionState.Closed, actual.State );

			Assert.Equal(
				actual
					.GetType()
					.GetField( "ParentDatabase", BindingFlags.Instance | BindingFlags.NonPublic )
					.GetValue( actual ),
				Database
			);
		}

		[Fact]
		public void ConnectionStringCtor_NullConnectionString() {
			MockConnection actual = new MockConnection( Database, null );

			Assert.Empty( actual.ConnectionString );
			Assert.Equal( 15, actual.ConnectionTimeout );
			Assert.Null( actual.Container );
			Assert.Empty( actual.Database );
			Assert.Empty( actual.DataSource );
			Assert.Equal( "0.9.0.0", actual.ServerVersion );
			Assert.Null( actual.Site );
			Assert.Equal( ConnectionState.Closed, actual.State );

			Assert.Equal(
				actual
					.GetType()
					.GetField( "ParentDatabase", BindingFlags.Instance | BindingFlags.NonPublic )
					.GetValue( actual ),
				Database
			);
		}

		[Fact]
		public void ConnectionStringCtor_NullDatabase() {
			const string connectionString = "This is my Connection StRiNg";
			Assert.IsType<ArgumentNullException>(
				Record.Exception( () =>
					new MockConnection( null, connectionString )
				)
			);
		}

		[Fact]
		public void Open() {
			MockConnection actual = new MockConnection( Database );
			Assert.NotEqual( ConnectionState.Open, actual.State );

			actual.Open();
			Assert.Equal( ConnectionState.Open, actual.State );
		}

		[Fact]
		public async Task OpenAsync() {
			MockConnection actual = new MockConnection( Database );
			Assert.NotEqual( ConnectionState.Open, actual.State );

			await actual.OpenAsync();
			Assert.Equal( ConnectionState.Open, actual.State );
		}

		[Fact]
		public void Close() {
			MockConnection actual = new MockConnection( Database );

			actual.Open();
			Assert.NotEqual( ConnectionState.Closed, actual.State );

			actual.Close();
			Assert.Equal( ConnectionState.Closed, actual.State );
		}

		[Fact]
		public void ChangeDatabase() {
			const string databaseName = "NewDatabaseName";
			MockConnection actual = new MockConnection( Database );
			Assert.NotEqual( databaseName, actual.Database );

			actual.ChangeDatabase( databaseName );
			Assert.Equal( databaseName, actual.Database );
		}

#if !NET472
		[Fact]
		public async Task CloseAsync() {
			MockConnection actual = new MockConnection( Database );

			actual.Open();
			Assert.NotEqual( ConnectionState.Closed, actual.State );

			await actual.CloseAsync();
			Assert.Equal( ConnectionState.Closed, actual.State );
		}

		[Fact]
		public async Task ChangeDatabaseAsync() {
			const string databaseName = "NewDatabaseName";
			MockConnection actual = new MockConnection( Database );
			Assert.NotEqual( databaseName, actual.Database );

			await actual.ChangeDatabaseAsync( databaseName );
			Assert.Equal( databaseName, actual.Database );
		}
#endif
	}
}
