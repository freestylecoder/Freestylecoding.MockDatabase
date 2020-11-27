using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
