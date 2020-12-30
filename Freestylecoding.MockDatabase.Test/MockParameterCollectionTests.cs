using System;
using System.Linq;
using Xunit;

namespace Freestylecoding.MockDatabase.Test {
	public class MockParameterCollectionTests {
		MockParameterCollection Collection = new MockParameterCollection();

		#region int Add( object value )
		[Fact]
		public void Add() {
			MockParameter input = CreateParameter( nameof( this.Add ), 42 );

			Assert.Equal( 0, Collection.Count );

			Collection.Add( input );
			Assert.Equal( 1, Collection.Count );

			Assert.Equal( input, Collection[nameof( this.Add )] );
		}

		[Fact]
		public void Add_ReturnValue() {
			Assert.Equal( 0, Collection.Add( CreateParameter( "param1", 1 ) ) );
			Assert.Equal( 1, Collection.Add( CreateParameter( "param2", 2 ) ) );
			Assert.Equal( 2, Collection.Add( CreateParameter( "param3", 3 ) ) );

			Collection.RemoveAt( 1 );
			Assert.Equal( 2, Collection.Add( CreateParameter( "param2", 2 ) ) );
		}

		[Fact]
		public void Add_Duplicate() {
			MockParameter input = CreateParameter( nameof( this.Add ), 42 );
			Collection.Add( input );
			Collection.Add( input );
			Assert.Equal( 2, Collection.Count );
		}

		[Fact]
		public void Add_Object() {
			Assert.IsType<InvalidCastException>(
				Record.Exception( () =>
					Collection.Add( new object() )
				)
			);
		}
		#endregion

		#region void AddRange( Array values )
		[Fact]
		public void AddRange() {
			MockParameter[] expected = Enumerable.Range( 1, 5 )
				.Select( i => CreateParameter( $"Param{i}", i ) )
				.ToArray();

			Collection.AddRange( expected );

			Assert.Equal( expected.Length, Collection.Count );
			Assert.Equal( expected, Collection.Cast<System.Data.Common.DbParameter>() );			
		}

		[Fact]
		public void AddRange_Duplicate() {
			MockParameter duplicate = CreateParameter( "Param", 1 );

			Assert.IsType<ArgumentException>(
				Record.Exception( () =>
					Collection.AddRange( new[] { duplicate, duplicate } )
				)
			);
		}

		[Fact]
		public void AddRange_AlreadyAdded() {
			MockParameter duplicate = CreateParameter( "Param", 1 );

			Collection.Add( duplicate );

			Assert.IsType<ArgumentException>(
				Record.Exception( () =>
					Collection.AddRange( new[] { duplicate } )
				)
			);
		}

		[Fact]
		public void AddRange_WrongType() {
			MockParameter duplicate = CreateParameter( "Param", 1 );

			Assert.IsType<InvalidCastException>(
				Record.Exception( () =>
					Collection.AddRange( new[] { duplicate, new object() } )
				)
			);
		}
		#endregion

		#region bool Contains( string value )
		[Fact]
		public void Contains() {
			const string input = nameof( this.Contains );
			Collection.Add( CreateParameter( input, 42 ) );
			Assert.True( this.Collection.Contains( input ) );
		}

		[Fact]
		public void Contains_False() {
			const string input = nameof( this.Contains_False );
			Assert.False( this.Collection.Contains( input ) );
		}

		[Theory]
		[InlineData( "prm" )]
		[InlineData( "prM" )]
		[InlineData( "pRm" )]
		[InlineData( "pRM" )]
		[InlineData( "Prm" )]
		[InlineData( "PrM" )]
		[InlineData( "PRm" )]
		[InlineData( "PRM" )]
		public void Contains_CaseInsensitive( string name ) {
			Collection.Add( CreateParameter( "prm", 42 ) );
			Assert.True( this.Collection.Contains( name ) );
		}

		[Theory]
		[InlineData( "" )]
		[InlineData( " " )]
		[InlineData( "\t" )]
		[InlineData( null )]
		public void Contains_NullOrWhiteSpace( string name ) {
			Assert.False( this.Collection.Contains( name ) );
		}
		#endregion

		#region void CopyTo( Array array, int index )
		[Fact]
		public void CopyTo() {
			MockParameter[] original = Enumerable.Range( 1, 4 )
				.Select( i => CreateParameter( $"Param{i}", i ) )
				.ToArray();

			MockParameter[] actual = new MockParameter[5];

			Collection.AddRange( original );
			Collection.CopyTo( actual, 1 );

			Assert.Null( actual[0] );
			Assert.Equal( original, actual.Skip(1) );
		}

		[Fact]
		public void CopyTo_BadIndex() {
			MockParameter[] original = Enumerable.Range( 1, 4 )
				.Select( i => CreateParameter( $"Param{i}", i ) )
				.ToArray();

			MockParameter[] actual = new MockParameter[4];

			Collection.AddRange( original );
			Assert.IsType<ArgumentOutOfRangeException>(
				Record.Exception( () =>
					Collection.CopyTo( actual, -1 )
				)
			);
		}

		[Fact]
		public void CopyTo_NotLongEnough() {
			MockParameter[] original = Enumerable.Range( 1, 4 )
				.Select( i => CreateParameter( $"Param{i}", i ) )
				.ToArray();

			MockParameter[] actual = new MockParameter[4];

			Collection.AddRange( original );
			Assert.IsType<ArgumentException>(
				Record.Exception( () =>
					Collection.CopyTo( actual, 1 )
				)
			);
		}

		[Fact]
		public void CopyTo_WrongArrayType() {
			MockParameter[] original = Enumerable.Range( 1, 4 )
				.Select( i => CreateParameter( $"Param{i}", i ) )
				.ToArray();

			int[] actual = new int[5];

			Collection.AddRange( original );
			Assert.IsType<ArgumentException>(
				Record.Exception( () =>
					Collection.CopyTo( actual, 1 )
				)
			);
		}
		#endregion

		#region int IndexOf( object value )
		[Fact]
		public void IndexOf_Object() {
			MockParameter[] original = Enumerable.Range( 1, 3 )
				.Select( i => CreateParameter( $"Param{i}", i ) )
				.ToArray();

			Collection.AddRange( original );

			Assert.Equal( 1, Collection.IndexOf( Collection[1] ) );
		}

		[Fact]
		public void IndexOf_Object_NotFound() {
			MockParameter[] original = Enumerable.Range( 1, 4 )
				.Select( i => CreateParameter( $"Param{i}", i ) )
				.ToArray();

			Collection.AddRange( original.Take( 3 ).ToArray() );

			Assert.Equal( -1, Collection.IndexOf( original[3] ) );
		}

		[Fact]
		public void IndexOf_Object_Null() {
			Assert.Equal( -1, Collection.IndexOf( null ) );
		}

		[Fact]
		public void IndexOf_Object_BadType() {
			Assert.IsType<InvalidCastException>( 
				Record.Exception( () =>
					Collection.IndexOf( new object() )
				)
			);
		}
		#endregion

		#region bool IndexOf( string parameterName )
		[Fact]
		public void IndexOf() {
			const string input = nameof( this.IndexOf );
			Collection.Add( CreateParameter( input, 42 ) );
			Collection.Add( CreateParameter( $"Not{input}", 666 ) );
			Assert.Equal( 0, this.Collection.IndexOf( input ) );
		}

		[Fact]
		public void IndexOf_False() {
			const string input = nameof( this.IndexOf_False );
			Assert.Equal( -1, this.Collection.IndexOf( input ) );
		}

		[Fact]
		public void IndexOf_FindsFirst() {
			const string input = nameof( this.IndexOf );
			Collection.Add( CreateParameter( input, 42 ) );
			Collection.Add( CreateParameter( input, 666 ) );
			Assert.Equal( 0, this.Collection.IndexOf( input ) );
		}

		[Theory]
		[InlineData( "prm" )]
		[InlineData( "prM" )]
		[InlineData( "pRm" )]
		[InlineData( "pRM" )]
		[InlineData( "Prm" )]
		[InlineData( "PrM" )]
		[InlineData( "PRm" )]
		[InlineData( "PRM" )]
		public void IndexOf_CaseInsensitive( string name ) {
			Collection.Add( CreateParameter( "Notprm", 666 ) );
			Collection.Add( CreateParameter( "prm", 42 ) );
			Assert.Equal( 1, this.Collection.IndexOf( name ) );
		}

		[Theory]
		[InlineData( "" )]
		[InlineData( " " )]
		[InlineData( "\t" )]
		[InlineData( null )]
		public void IndexOf_NullOrWhiteSpace( string name ) {
			Assert.Equal( -1, this.Collection.IndexOf( name ) );
		}
		#endregion

		#region void Insert( int index, object value )
		[Fact]
		public void Insert() {
			MockParameter[] original = Enumerable.Range( 1, 4 )
				.Select( i => CreateParameter( $"Param{i}", i ) )
				.ToArray();

			Collection.AddRange( original.Skip( 2 ).Prepend( original[0] ).ToArray() );
			Collection.Insert( 1, original[1] );

			Assert.Equal( original, Collection.OfType<MockParameter>() );
		}

		[Fact]
		public void Insert_Prepend() {
			MockParameter[] original = Enumerable.Range( 1, 4 )
				.Select( i => CreateParameter( $"Param{i}", i ) )
				.ToArray();

			Collection.AddRange( original.Skip( 1 ).ToArray() );
			Collection.Insert( 0, original[0] );

			Assert.Equal( original, Collection.OfType<MockParameter>() );
		}

		[Fact]
		public void Insert_Append() {
			MockParameter[] original = Enumerable.Range( 1, 4 )
				.Select( i => CreateParameter( $"Param{i}", i ) )
				.ToArray();

			Collection.AddRange( original.Take( 3 ).ToArray() );
			Collection.Insert( 3, original[3] );

			Assert.Equal( original, Collection.OfType<MockParameter>() );
		}

		[Theory]
		[InlineData( -1 )]
		[InlineData( 10 )]
		public void Insert_OutOfRange( int index ) {
			MockParameter[] original = Enumerable.Range( 1, 4 )
				.Select( i => CreateParameter( $"Param{i}", i ) )
				.ToArray();

			Collection.AddRange( original.Skip( 2 ).Prepend( original[0] ).ToArray() );
			Assert.IsType<ArgumentOutOfRangeException>(
				Record.Exception( () =>
					Collection.Insert( index, original[1] )
				)
			);
		}

		[Fact]
		public void Insert_Duplicate() {
			MockParameter[] original = Enumerable.Range( 1, 4 )
				.Select( i => CreateParameter( $"Param{i}", i ) )
				.ToArray();

			Collection.AddRange( original );
			Assert.IsType<ArgumentException>(
				Record.Exception( () =>
					Collection.Insert( 4, original[3] )
				)
			);
		}

		[Fact]
		public void Insert_Null() {
			MockParameter[] original = Enumerable.Range( 1, 4 )
				.Select( i => CreateParameter( $"Param{i}", i ) )
				.ToArray();

			Collection.AddRange( original );
			Assert.IsType<ArgumentNullException>(
				Record.Exception( () =>
					Collection.Insert( 4, null )
				)
			);
		}

		[Fact]
		public void Insert_WrongType() {
			MockParameter[] original = Enumerable.Range( 1, 4 )
				.Select( i => CreateParameter( $"Param{i}", i ) )
				.ToArray();

			Collection.AddRange( original );
			Assert.IsType<InvalidCastException>(
				Record.Exception( () =>
					Collection.Insert( 4, new object() )
				)
			);
		}
		#endregion

		#region void Remove( object value )
		[Fact]
		public void Remove() {
			MockParameter[] original = Enumerable.Range( 1, 4 )
				.Select( i => CreateParameter( $"Param{i}", i ) )
				.ToArray();

			Collection.AddRange( original );
			Collection.Remove( original[0] );

			Assert.Equal( original.Skip( 1 ), Collection.OfType<MockParameter>() );
		}

		[Fact]
		public void Remove_NotFound() {
			MockParameter[] original = Enumerable.Range( 1, 4 )
				.Select( i => CreateParameter( $"Param{i}", i ) )
				.ToArray();

			Collection.AddRange( original.Take( 3 ).ToArray() );
			Collection.Remove( original[3] );

			Assert.Equal( original.Take( 3 ), Collection.OfType<MockParameter>() );
		}

		[Fact]
		public void Remove_Null() {
			MockParameter[] original = Enumerable.Range( 1, 4 )
				.Select( i => CreateParameter( $"Param{i}", i ) )
				.ToArray();

			Collection.AddRange( original );
			Assert.IsType<ArgumentNullException>( 
				Record.Exception( () =>
					Collection.Remove( null )
				)
			);
		}

		[Fact]
		public void Remove_WrongType() {
			MockParameter[] original = Enumerable.Range( 1, 4 )
				.Select( i => CreateParameter( $"Param{i}", i ) )
				.ToArray();

			Collection.AddRange( original );
			Assert.IsType<InvalidCastException>( 
				Record.Exception( () =>
					Collection.Remove( new object() )
				)
			);
		}
		#endregion

		#region void RemoveAt( string parameterName )
		[Fact]
		public void RemoveAt() {
			MockParameter[] original = Enumerable.Range( 1, 4 )
				.Select( i => CreateParameter( $"Param{i}", i ) )
				.ToArray();

			Collection.AddRange( original );
			Collection.RemoveAt( original[0].ParameterName );

			Assert.Equal( original.Skip( 1 ), Collection.OfType<MockParameter>() );
		}

		[Theory]
		[InlineData( "" )]
		[InlineData( " " )]
		[InlineData( "\t" )]
		[InlineData( null )]
		[InlineData( "BadParam" )]
		public void RemoveAt_NotFound( string name ) {
			MockParameter[] original = Enumerable.Range( 1, 4 )
				.Select( i => CreateParameter( $"Param{i}", i ) )
				.ToArray();

			Collection.AddRange( original );
			Assert.IsType<IndexOutOfRangeException>(
				Record.Exception( () =>
					Collection.RemoveAt( name )
				)
			);
		}
		#endregion

		private static Random random = new Random( Guid.NewGuid().GetHashCode() );
		private static T GetRandomEnum<T>() {
			Array values = Enum.GetValues( typeof( T ) );
			return (T)values.GetValue( random.Next( 0, values.Length ) );
		}
		private static byte GetRandomByte() {
			byte[] buffer = new byte[1];
			random.NextBytes( buffer );
			return buffer[0];
		}
		private static bool GetRandomBoolean() =>
			0.5 < random.NextDouble();

		private MockParameter CreateParameter( string parameterName, object value ) =>
			new MockParameter() {
				DbType = GetRandomEnum<System.Data.DbType>(),
				Direction = GetRandomEnum<System.Data.ParameterDirection>(),
				IsNullable = GetRandomBoolean(),
				ParameterName = parameterName,
				Precision = GetRandomByte(),
				Scale = GetRandomByte(),
				Size = random.Next(),
				SourceColumn = parameterName.ToUpperInvariant(),
				SourceColumnNullMapping = GetRandomBoolean(),
				SourceVersion = GetRandomEnum<System.Data.DataRowVersion>(),
				Value = value
			};
	}
}
