using System.Data;
using Xunit;

namespace Freestylecoding.MockDatabase.Test {
	public class MockParameterTests {
		[Fact]
		public void Defaults() {
			MockParameter actual = new MockParameter();

			Assert.Equal( DbType.String, actual.DbType );
			Assert.Equal( ParameterDirection.Input, actual.Direction );
			Assert.False( actual.IsNullable );
			Assert.Empty( actual.ParameterName );
			Assert.Equal( 0, actual.Size );
			Assert.Empty( actual.SourceColumn );
			Assert.False( actual.SourceColumnNullMapping );
			Assert.Null( actual.Value );
		}

		[Fact]
		public void ResetDbType() {
			MockParameter actual = new MockParameter();
			Assert.Equal( DbType.String, actual.DbType );

			actual.DbType = DbType.Guid;
			Assert.Equal( DbType.Guid, actual.DbType );
			
			actual.ResetDbType();
			Assert.Equal( DbType.String, actual.DbType );
		}
	}
}
