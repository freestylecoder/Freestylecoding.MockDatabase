using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using Xunit;

namespace Freestylecoding.MockDatabase.Test {
	public class MockEndToEndTests {
		#region void AddResult( DataTable table )
		[Fact]
		public void AddResult() {
			DataTable data = new DataTable();

			data.Columns.AddRange( new[] {
				new DataColumn( "Column0", typeof( int ) ),
				new DataColumn( "Column1", typeof( string ) )
			} );

			Enumerable.Range( 0, 5 )
				.ToList()
				.ForEach( i => {
					DataRow row = data.NewRow();
					row[0] = i;
					row[1] = i.ToString();
					data.Rows.Add( row );
				} );

			MockDatabase database = new MockDatabase();
			database.AddResult( data );

			DbConnection connection = database.GetConnection();
			DbCommand command = connection.CreateCommand();
			DbDataReader reader = command.ExecuteReader();

			foreach( int i in Enumerable.Range( 0, 5 ) ) {
				Assert.True( reader.Read() );
				Assert.Equal( i, reader.GetInt32( 0 ) );
				Assert.Equal( i.ToString(), reader.GetString( 1 ) );
			}

			Assert.False( reader.Read() );
			Assert.False( reader.NextResult() );
		}

		[Fact]
		public void AddResult_Null() {
			MockDatabase database = new MockDatabase();
			database.AddResult( null );

			DbConnection connection = database.GetConnection();
			DbCommand command = connection.CreateCommand();
			DbDataReader reader = command.ExecuteReader();

			Assert.False( reader.Read() );
			Assert.False( reader.NextResult() );
		}

		[Fact]
		public void AddResult_Multiple() {
			Func<MockDatabase,int,DataTable> CreateSimpleDataReader = ( MockDatabase d, int value ) =>
				d.CreateDataTable(
					new[] { value },
					new[] { new DataColumn( "Column0", typeof( int ) ) },
					i => new object[] { i }
				);

			MockDatabase database = new MockDatabase();
			DbConnection connection = database.GetConnection();

			DataTable dt1 = CreateSimpleDataReader( database, 1 );
			DataTable dt2 = CreateSimpleDataReader( database, 2 );
			DataTable dt3 = CreateSimpleDataReader( database, 3 );

			database.AddResult( dt1 );
			database.AddResult( dt2 );
			database.AddResult( dt3 );

			Assert.Equal( 3, GetResults( database ).Count() );
			AssertEqual( dt1, GetResults( database ).Dequeue() );
			AssertEqual( dt2, GetResults( database ).Dequeue() );
			AssertEqual( dt3, GetResults( database ).Dequeue() );
		}

		private Queue<DataTable> GetResults( MockDatabase database ) =>
			database
				?.GetType()
				?.GetField( "Results", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic )
				?.GetValue( database )
				as Queue<DataTable>;
		#endregion

		#region void AddEmptyResult()
		[Fact]
		public void AddEmptyResult_Single_Reader() {
			MockDatabase database = new MockDatabase();
			database.AddEmptyResult();

			DbConnection connection = database.GetConnection();
			DbCommand command = connection.CreateCommand();
			DbDataReader reader = command.ExecuteReader();

			Assert.False( reader.Read() );
			Assert.False( reader.NextResult() );
		}

		[Fact]
		public void AddEmptyResult_Multiple_Reader() {
			MockDatabase database = new MockDatabase();
			database.AddEmptyResult();
			database.AddEmptyResult();

			DbConnection connection = database.GetConnection();
			DbCommand command = connection.CreateCommand();
			DbDataReader reader = command.ExecuteReader();

			Assert.False( reader.Read() );
			Assert.True( reader.NextResult() );

			Assert.False( reader.Read() );
			Assert.False( reader.NextResult() );
		}

		[Fact]
		public void AddEmptyResult_Single_Scalar() {
			MockDatabase database = new MockDatabase();
			database.AddEmptyResult();

			DbConnection connection = database.GetConnection();
			DbCommand command = connection.CreateCommand();
			object actual = command.ExecuteScalar();

			Assert.Null( actual );
		}

		[Fact]
		public void AddEmptyResult_Single_NonQuery() {
			MockDatabase database = new MockDatabase();
			database.AddEmptyResult();

			DbConnection connection = database.GetConnection();
			DbCommand command = connection.CreateCommand();
			int actual = command.ExecuteNonQuery();

			Assert.Equal( 0, actual );
		}
		#endregion

		#region void AddNonQueryResult()
		[Fact]
		public void AddNonQueryResult_Single_Reader() {
			MockDatabase database = new MockDatabase();
			database.AddNonQueryResult( 42 );

			DbConnection connection = database.GetConnection();
			DbCommand command = connection.CreateCommand();
			DbDataReader reader = command.ExecuteReader();

			Assert.True( reader.Read() );
			Assert.Equal( 1, reader.FieldCount );
			Assert.Equal( 42, reader.GetInt32( 0 ) );
			Assert.Equal( 0, reader.GetOrdinal( "RecordsAffected" ) );

			Assert.False( reader.Read() );
			Assert.False( reader.NextResult() );
		}

		[Fact]
		public void AddNonQueryResult_Multiple_Reader() {
			MockDatabase database = new MockDatabase();
			database.AddNonQueryResult( 42 );
			database.AddNonQueryResult( 24 );

			DbConnection connection = database.GetConnection();
			DbCommand command = connection.CreateCommand();
			DbDataReader reader = command.ExecuteReader();

			Assert.True( reader.Read() );
			Assert.Equal( 1, reader.FieldCount );
			Assert.Equal( 42, reader.GetInt32( 0 ) );
			Assert.Equal( 0, reader.GetOrdinal( "RecordsAffected" ) );

			Assert.False( reader.Read() );
			Assert.True( reader.NextResult() );

			Assert.True( reader.Read() );
			Assert.Equal( 1, reader.FieldCount );
			Assert.Equal( 24, reader.GetInt32( 0 ) );
			Assert.Equal( 0, reader.GetOrdinal( "RecordsAffected" ) );

			Assert.False( reader.Read() );
			Assert.False( reader.NextResult() );
		}

		[Fact]
		public void AddNonQueryResult_Single_Scalar() {
			MockDatabase database = new MockDatabase();
			database.AddNonQueryResult( 42 );

			DbConnection connection = database.GetConnection();
			DbCommand command = connection.CreateCommand();
			object actual = command.ExecuteScalar();

			Assert.Equal( 42, actual );
		}

		[Fact]
		public void AddNonQueryResult_Single_NonQuery() {
			MockDatabase database = new MockDatabase();
			database.AddNonQueryResult( 42 );

			DbConnection connection = database.GetConnection();
			DbCommand command = connection.CreateCommand();
			int actual = command.ExecuteNonQuery();

			Assert.Equal( 42, actual );
		}
		#endregion

		#region void AddScalarResult( object o )
		[Fact]
		public void AddScalarResult_Single_Reader() {
			MockDatabase database = new MockDatabase();
			database.AddScalarResult( 42 );

			DbConnection connection = database.GetConnection();
			DbCommand command = connection.CreateCommand();
			DbDataReader reader = command.ExecuteReader();

			Assert.True( reader.Read() );
			Assert.Equal( 1, reader.FieldCount );
			Assert.Equal( 42, reader.GetInt32( 0 ) );
			Assert.Equal( 0, reader.GetOrdinal( "Scalar" ) );

			Assert.False( reader.Read() );
			Assert.False( reader.NextResult() );
		}

		[Fact]
		public void AddScalarResult_Multiple_Reader() {
			MockDatabase database = new MockDatabase();
			database.AddScalarResult( 42 );
			database.AddScalarResult( 24 );

			DbConnection connection = database.GetConnection();
			DbCommand command = connection.CreateCommand();
			DbDataReader reader = command.ExecuteReader();

			Assert.True( reader.Read() );
			Assert.Equal( 1, reader.FieldCount );
			Assert.Equal( 42, reader.GetInt32( 0 ) );
			Assert.Equal( 0, reader.GetOrdinal( "Scalar" ) );

			Assert.False( reader.Read() );
			Assert.True( reader.NextResult() );

			Assert.True( reader.Read() );
			Assert.Equal( 1, reader.FieldCount );
			Assert.Equal( 24, reader.GetInt32( 0 ) );
			Assert.Equal( 0, reader.GetOrdinal( "Scalar" ) );

			Assert.False( reader.Read() );
			Assert.False( reader.NextResult() );
		}

		[Fact]
		public void AddScalarResult_Single_Scalar() {
			MockDatabase database = new MockDatabase();
			database.AddScalarResult( 42 );

			DbConnection connection = database.GetConnection();
			DbCommand command = connection.CreateCommand();
			object actual = command.ExecuteScalar();

			Assert.Equal( 42, actual );
		}

		[Fact]
		public void AddScalarResult_Single_NonQuery() {
			MockDatabase database = new MockDatabase();
			database.AddScalarResult( 42 );

			DbConnection connection = database.GetConnection();
			DbCommand command = connection.CreateCommand();
			int actual = command.ExecuteNonQuery();

			Assert.Equal( 42, actual );
		}

		[Theory]
		[InlineData( 1, typeof( int ) )]
		[InlineData( 1L, typeof( long ) )]
		[InlineData( 1U, typeof( uint ) )]
		[InlineData( 1UL, typeof( ulong ) )]
		[InlineData( 1F, typeof( float ) )]
		[InlineData( 1.0, typeof( double ) )]
		[InlineData( 'A', typeof( char ) )]
		[InlineData( true, typeof( bool ) )]
		[InlineData( "string", typeof( string ) )]
		public void AddScalarResult_TypeCheck( object value, Type type ) {
			MockDatabase database = new MockDatabase();
			database.AddScalarResult( value );

			DbConnection connection = database.GetConnection();
			DbCommand command = connection.CreateCommand();
			object actual = command.ExecuteScalar();

			Assert.Equal( value, actual );
			Assert.Equal( type, actual.GetType() );
		}
		#endregion

		#region void AddJsonResult( string json )
		[Fact]
		public void AddJsonResult() {
			string chunk1 = new string( '1', 2000 );
			string chunk2 = new string( '2', 2000 );
			string chunk3 = new string( '3', 1000 );

			MockDatabase database = new MockDatabase();
			database.AddJsonResult( $"{chunk1}{chunk2}{chunk3}" );

			DbConnection connection = database.GetConnection();
			DbCommand command = connection.CreateCommand();
			DbDataReader reader = command.ExecuteReader();

			Assert.True( reader.Read() );
			Assert.Equal( chunk1, reader.GetString( 0 ) );

			Assert.True( reader.Read() );
			Assert.Equal( chunk2, reader.GetString( 0 ) );

			Assert.True( reader.Read() );
			Assert.Equal( chunk3, reader.GetString( 0 ) );

			Assert.False( reader.Read() );
			Assert.False( reader.NextResult() );
		}

		[Fact]
		public void AddJsonResult_Small() {
			string chunk1 = new string( '1', 1000 );

			MockDatabase database = new MockDatabase();
			database.AddJsonResult( chunk1 );

			DbConnection connection = database.GetConnection();
			DbCommand command = connection.CreateCommand();
			DbDataReader reader = command.ExecuteReader();

			Assert.True( reader.Read() );
			Assert.Equal( chunk1, reader.GetString( 0 ) );

			Assert.False( reader.Read() );
			Assert.False( reader.NextResult() );
		}

		[Fact]
		public void AddJsonResult_Scalar() {
			string chunk1 = new string( '1', 1000 );

			MockDatabase database = new MockDatabase();
			database.AddJsonResult( chunk1 );

			DbConnection connection = database.GetConnection();
			DbCommand command = connection.CreateCommand();
			object actual = command.ExecuteScalar();

			Assert.Equal( chunk1, actual );
		}

		[Fact]
		public void AddJsonResult_Scalar_Large() {
			string chunk1 = new string( '1', 2000 );

			MockDatabase database = new MockDatabase();
			database.AddJsonResult( $"{chunk1}{new string( '2', 2000 )}{new string( '3', 1000 )}" );

			DbConnection connection = database.GetConnection();
			DbCommand command = connection.CreateCommand();
			object actual = command.ExecuteScalar();

			Assert.Equal( chunk1, actual );
		}

		[Theory]
		[InlineData( "" )]
		[InlineData( " " )]
		[InlineData( "\t" )]
		[InlineData( null )]
		public void AddJsonResult_IsNullOrWhitespace( string value ) {
			MockDatabase database = new MockDatabase();
			database.AddJsonResult( value );

			DbConnection connection = database.GetConnection();
			DbCommand command = connection.CreateCommand();
			DbDataReader reader = command.ExecuteReader();

			Assert.False( reader.Read() );
			Assert.False( reader.NextResult() );
		}
		#endregion

		#region void AddSqlException()
		[Fact]
		public void AddSqlException() {
			MockDatabase database = new MockDatabase();
			database.AddSqlException();

			DbConnection connection = database.GetConnection();
			DbCommand command = connection.CreateCommand();
			Assert.IsType<SqlException>(
				Record.Exception( () =>
					command.ExecuteNonQuery()
				)
			);
		}

		[Fact]
		public void AddSqlException_Parameters() {
			Guid conId = Guid.NewGuid();
			const string message = "Test Message";
			Exception expectedEx = new Exception();

			MockDatabase database = new MockDatabase();
			database.AddSqlException(
				message,
				null,
				expectedEx,
				conId
			);

			DbConnection connection = database.GetConnection();
			DbCommand command = connection.CreateCommand();

			Exception actual = Record.Exception( () => command.ExecuteNonQuery() );
			Assert.IsType<SqlException>( actual );

			SqlException ex = actual as SqlException;
			Assert.Equal( message, ex.Message );
			Assert.Empty( ex.Errors );
			Assert.Equal( expectedEx, ex.InnerException );
			Assert.Equal( conId, ex.ClientConnectionId );
		}

		[Fact]
		public void AddSqlException_InChain() {
			MockDatabase database = new MockDatabase();
			database.AddNonQueryResult( 42 );
			database.AddSqlException();

			DbConnection connection = database.GetConnection();
			DbCommand command = connection.CreateCommand();
			Assert.Equal( 42, command.ExecuteNonQuery() );
			Assert.IsType<SqlException>(
				Record.Exception( () =>
					command.ExecuteNonQuery()
				)
			);
		}

		[Fact]
		public void AddSqlException_NextResult() {
			MockDatabase database = new MockDatabase();
			database.AddResult( null );
			database.AddSqlException();

			DbConnection connection = database.GetConnection();
			DbCommand command = connection.CreateCommand();
			DbDataReader reader = command.ExecuteReader();
			Assert.IsType<SqlException>(
				Record.Exception( () =>
					reader.NextResult()
				)
			);
		}
		#endregion

		#region DataTable CreateDataTable<T>( ... )
		[Fact]
		public void CreateDataTable() {
			DataTable data = new DataTable();

			data.Columns.AddRange( new[] {
				new DataColumn( "Column0", typeof( int ) ),
				new DataColumn( "Column1", typeof( string ) )
			} );

			Enumerable.Range( 1, 5 )
				.ToList()
				.ForEach( i => {
					DataRow row = data.NewRow();
					row[0] = i;
					row[1] = i.ToString();
					data.Rows.Add( row );
				} );

			MockDatabase database = new MockDatabase();
			DataTable actual = database.CreateDataTable(
				Enumerable.Range( 1, 5 ),
				new[] {
					new DataColumn( "Column0", typeof( int ) ),
					new DataColumn( "Column1", typeof( string ) )
				},
				i => new object[] { i, i.ToString() }
			);

			AssertEqual( data, actual );
		}

		[Fact]
		public void CreateDataTable_NoData() {
			DataTable data = new DataTable();

			data.Columns.AddRange( new[] {
				new DataColumn( "Column0", typeof( int ) ),
				new DataColumn( "Column1", typeof( string ) )
			} );

			MockDatabase database = new MockDatabase();
			DataTable actual = database.CreateDataTable(
				Enumerable.Empty<int>(),
				new[] {
					new DataColumn( "Column0", typeof( int ) ),
					new DataColumn( "Column1", typeof( string ) )
				},
				i => new object[] { i, i.ToString() }
			);

			AssertEqual( data, actual );
		}

		[Fact]
		public void CreateDataTable_NoColumns() {
			MockDatabase database = new MockDatabase();
			Assert.IsType<ArgumentException>(
				Record.Exception( () =>
					database.CreateDataTable(
						Enumerable.Range( 1, 5 ),
						Enumerable.Empty<DataColumn>(),
						i => new object[] { i, i.ToString() }
					)
				)
			);
		}

		[Fact]
		public void CreateDataTable_TooFewColumns() {
			MockDatabase database = new MockDatabase();
			Assert.IsType<ArgumentException>(
				Record.Exception( () =>
					database.CreateDataTable(
						Enumerable.Range( 1, 5 ),
						new[] {
							new DataColumn( "Column0", typeof( int ) )
						},
						i => new object[] { i, i.ToString() }
					)
				)
			);
		}

		[Fact]
		public void CreateDataTable_TooManyColumns_NotNullable() {
			MockDatabase database = new MockDatabase();
			Assert.IsType<NoNullAllowedException>(
				Record.Exception( () =>
					database.CreateDataTable(
						Enumerable.Range( 1, 5 ),
						new[] {
							new DataColumn( "Column0", typeof( int ) ),
							new DataColumn( "Column1", typeof( string ) ),
							new DataColumn( "Column2", typeof( string ) ) { AllowDBNull = false }
						},
						i => new object[] { i, i.ToString() }
					)
				)
			);
		}

		[Fact]
		public void CreateDataTable_TooManyColumns_Nullable() {
			DataTable data = new DataTable();

			data.Columns.AddRange( new[] {
				new DataColumn( "Column0", typeof( int ) ),
				new DataColumn( "Column1", typeof( string ) ),
				new DataColumn( "Column2", typeof( string ) ) { AllowDBNull = true }
			} );

			Enumerable.Range( 1, 5 )
				.ToList()
				.ForEach( i => {
					DataRow row = data.NewRow();
					row[0] = i;
					row[1] = i.ToString();
					row[2] = null;
					data.Rows.Add( row );
				} );

			MockDatabase database = new MockDatabase();
			DataTable actual = database.CreateDataTable(
				Enumerable.Range( 1, 5 ),
				new[] {
					new DataColumn( "Column0", typeof( int ) ),
					new DataColumn( "Column1", typeof( string ) ),
					new DataColumn( "Column2", typeof( string ) ) { AllowDBNull = true }
				},
				i => new object[] { i, i.ToString() }
			);

			AssertEqual( data, actual );
		}

		[Fact]
		public void CreateDataTable_WrongType() {
			MockDatabase database = new MockDatabase();
			Assert.IsType<ArgumentException>(
				Record.Exception( () =>
					database.CreateDataTable(
						Enumerable.Range( 1, 5 ),
						new[] {
							new DataColumn( "Column0", typeof( Guid ) ),
							new DataColumn( "Column1", typeof( string ) )
						},
						i => new object[] { this, i.ToString() }
					)
				)
			);
		}

		[Fact]
		public void CreateDataTable_ValueTooBig() {
			MockDatabase database = new MockDatabase();
			Assert.IsType<ArgumentException>(
				Record.Exception( () =>
					database.CreateDataTable(
						Enumerable.Range( 1, 5 ),
						new[] {
							new DataColumn( "Column0", typeof( int ) ),
							new DataColumn( "Column1", typeof( string ) ) { MaxLength = 3 }
						},
						i => new object[] { i, new string( '1', i ) }
					)
				)
			);
		}

		[Fact]
		public void CreateDataTable_NullList() {
			MockDatabase database = new MockDatabase();
			Assert.IsType<NullReferenceException>(
				Record.Exception( () =>
					database.CreateDataTable(
						(IEnumerable<int>)null,
						new[] {
							new DataColumn( "Column0", typeof( int ) ),
							new DataColumn( "Column1", typeof( string ) )
						},
						i => new object[] { i, new string( '1', i ) }
					)
				)
			);
		}

		[Fact]
		public void CreateDataTable_NullColumns() {
			MockDatabase database = new MockDatabase();
			Assert.IsType<ArgumentNullException>(
				Record.Exception( () =>
					database.CreateDataTable(
						Enumerable.Range( 1, 5 ),
						null,
						i => new object[] { i, new string( '1', i ) }
					)
				)
			);
		}

		[Fact]
		public void CreateDataTable_NullFunc() {
			MockDatabase database = new MockDatabase();
			Assert.IsType<NullReferenceException>(
				Record.Exception( () =>
					database.CreateDataTable(
						Enumerable.Range( 1, 5 ),
						new[] {
							new DataColumn( "Column0", typeof( int ) ),
							new DataColumn( "Column1", typeof( string ) ) { MaxLength = 3 }
						},
						null
					)
				)
			);
		}

		[Fact]
		public void CreateDataTable_FuncReturnsNull() {
			MockDatabase database = new MockDatabase();
			Assert.IsType<ArgumentNullException>(
				Record.Exception( () =>
					database.CreateDataTable(
						Enumerable.Range( 1, 5 ),
						new[] {
							new DataColumn( "Column0", typeof( int ) ),
							new DataColumn( "Column1", typeof( string ) ) { MaxLength = 3 }
						},
						i => null
					)
				)
			);
		}
		#endregion

		private void AssertEqual( DataTable expected, DataTable actual ) {
			if( expected == actual )
				return;

			Assert.Equal( expected.Columns.Count, actual.Columns.Count );
			for( int i = 0; i < expected.Columns.Count; ++i ) {
				Assert.Equal( expected.Columns[i].ColumnName, actual.Columns[i].ColumnName );
				Assert.Equal( expected.Columns[i].DataType, actual.Columns[i].DataType );
			}

			Assert.Equal( expected.Rows.Count, actual.Rows.Count );
			for( int i = 0; i < expected.Rows.Count; ++i ) {
				for( int j = 0; j < expected.Columns.Count; ++j ) {
					Assert.Equal( expected.Rows[i][j], actual.Rows[i][j] );
				}
			}
		}
	}
}
