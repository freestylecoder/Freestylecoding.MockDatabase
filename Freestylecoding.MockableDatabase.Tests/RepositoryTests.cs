using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Freestylecoding.MockableDatabase.Tests {
	public class RepositoryTests {
		private readonly TestableRepository Repository;

		public RepositoryTests() {
			this.Repository = new TestableRepository();
		}

		[Fact]
		public void SelectOne() {
			IEnumerable<TestEntity> expected = Enumerable.Range( 1, 5 )
				.Select( i => new TestEntity( i, $"Value {i}" ) );

			expected
				.ToList()
				.ForEach( o => this.Repository.Data.Add( o.Id, o ) );

			Assert.Equal(
				expected.First(),
				this.Repository.BlockingSelect( expected.First().Id )
			);
		}

		[Fact]
		public void SelectOne_NotFound() {
			IEnumerable<TestEntity> expected = Enumerable.Range( 1, 5 )
				.Select( i => new TestEntity( i, $"Value {i}" ) );

			expected
				.ToList()
				.ForEach( o => this.Repository.Data.Add( o.Id, o ) );

			Assert.Null(
				this.Repository.BlockingSelect( expected.Last().Id + 1 )
			);
		}

		[Fact]
		public void SelectOne_Exceptional() {
			Assert.IsType<EvilValueException>(
				Record.Exception( () =>
					this.Repository.BlockingSelect( EvilValueException.EvilValue )
				)
			);
		}

		[Fact]
		public void SelectAll() {
			IEnumerable<TestEntity> expected = Enumerable.Range( 1, 5 )
				.Select( i => new TestEntity( i, $"Value {i}" ) );

			expected
				.ToList()
				.ForEach( o => this.Repository.Data.Add( o.Id, o ) );

			Assert.Equal(
				expected,
				this.Repository.BlockingSelect()
			);
		}

		[Fact]
		public void SelectAll_NoneFound() {
			Assert.Empty(
				this.Repository.BlockingSelect()
			);
		}

		[Fact]
		public void SelectAll_Exceptional() {
			this.Repository.SelectAllShoudThrow = true;
			Assert.IsType<EvilValueException>(
				Record.Exception( () =>
					this.Repository.BlockingSelect( EvilValueException.EvilValue )
				)
			);
		}

		[Fact]
		public void SelectSome() {
			IEnumerable<TestEntity> expected = Enumerable.Range( 1, 5 )
				.Select( i => new TestEntity( i, $"Value {i}" ) );

			expected
				.ToList()
				.ForEach( o => this.Repository.Data.Add( o.Id, o ) );

			Assert.Equal(
				expected.Take( 3 ),
				this.Repository.BlockingSelect( expected.Take( 3 ).Select( o => o.Id ) )
			);
		}

		[Fact]
		public void SelectSome_SomeFound() {
			IEnumerable<TestEntity> expected = Enumerable.Range( 1, 5 )
				.Select( i => new TestEntity( i, $"Value {i}" ) );

			expected
				.ToList()
				.ForEach( o => this.Repository.Data.Add( o.Id, o ) );

			Assert.Equal(
				expected.Take( 3 ),
				this.Repository.BlockingSelect( Enumerable.Range( -1, 5 ) )
			);
		}

		[Fact]
		public void SelectSome_NoneFound() {
			IEnumerable<TestEntity> expected = Enumerable.Range( 1, 5 )
				.Select( i => new TestEntity( i, $"Value {i}" ) );

			expected
				.ToList()
				.ForEach( o => this.Repository.Data.Add( o.Id, o ) );

			Assert.Empty(
				this.Repository.BlockingSelect( Enumerable.Range( 11, 5 ) )
			);
		}

		[Fact]
		public void SelectSome_EmptyList() {
			IEnumerable<TestEntity> expected = Enumerable.Range( 1, 5 )
				.Select( i => new TestEntity( i, $"Value {i}" ) );

			expected
				.ToList()
				.ForEach( o => this.Repository.Data.Add( o.Id, o ) );

			Assert.Empty(
				this.Repository.BlockingSelect( Enumerable.Empty<int>() )
			);
		}

		[Fact]
		public void SelectSome_NullList() {
			IEnumerable<TestEntity> expected = Enumerable.Range( 1, 5 )
				.Select( i => new TestEntity( i, $"Value {i}" ) );

			expected
				.ToList()
				.ForEach( o => this.Repository.Data.Add( o.Id, o ) );

			Assert.Empty(
				this.Repository.BlockingSelect( null )
			);
		}

		[Fact]
		public void SelectSome_Exceptional() {
			Assert.IsType<EvilValueException>(
				Record.Exception( () =>
					this.Repository.BlockingSelect( new[] { EvilValueException.EvilValue } )
				)
			);
		}

		[Fact]
		public void Insert() {
			Assert.Empty( Repository.BlockingSelect() );

			TestEntity expected = new TestEntity( 2, "Value 2" );
			TestEntity actual = Repository.BlockingInsert( expected );

			Assert.Equal( actual, Repository.BlockingSelect( expected.Id ) );
		}

		[Fact]
		public void Insert_ReturnValue() {
			Assert.Empty( Repository.BlockingSelect() );

			TestEntity expected = new TestEntity( 2, "Value 2" );
			TestEntity actual = Repository.BlockingInsert( expected );

			Assert.Equal( expected, actual );
		}

		[Fact]
		public void Insert_Duplicate() {
			Assert.Empty( Repository.BlockingSelect() );

			TestEntity expected = new TestEntity( 2, "Value 2" );
			Repository.BlockingInsert( expected );

			Assert.IsType<ArgumentException>(
				Record.Exception( () =>
					this.Repository.BlockingInsert( expected )
				)
			);
		}

		[Fact]
		public void Insert_Null() {
			Assert.IsType<NullReferenceException>(
				Record.Exception( () =>
					this.Repository.BlockingInsert( null )
				)
			);
		}

		[Fact]
		public void Update() {
			Assert.Empty( Repository.BlockingSelect() );

			TestEntity original = new TestEntity( 2, "Value 2" );
			TestEntity expected = new TestEntity( 2, "Value 3" );
			Repository.BlockingInsert( original );

			TestEntity actual = Repository.BlockingUpdate( expected );

			Assert.Equal( actual, Repository.BlockingSelect( expected.Id ) );
		}

		[Fact]
		public void Update_ReturnValue() {
			Assert.Empty( Repository.BlockingSelect() );

			TestEntity original = new TestEntity( 2, "Value 2" );
			TestEntity expected = new TestEntity( 2, "Value 3" );
			Repository.BlockingInsert( original );

			TestEntity actual = Repository.BlockingUpdate( expected );
			Assert.Equal( expected, actual );
		}

		[Fact]
		public void Update_Add() {
			Assert.Empty( Repository.BlockingSelect() );
			TestEntity expected = new TestEntity( 2, "Value 2" );
			TestEntity actual = this.Repository.BlockingUpdate( expected );

			Assert.Equal( expected, actual );
		}

		[Fact]
		public void Update_Null() {
			Assert.IsType<NullReferenceException>(
				Record.Exception( () =>
					this.Repository.BlockingUpdate( null )
				)
			);
		}

		[Fact]
		public void Delete() {
			Assert.Empty( Repository.BlockingSelect() );

			TestEntity expected = new TestEntity( 2, "Value 2" );
			Repository.BlockingInsert( expected );
			Assert.NotEmpty( Repository.BlockingSelect() );

			Repository.BlockingDelete( expected.Id );
			Assert.Empty( Repository.BlockingSelect() );
		}

		[Fact]
		public void Delete_ReturnValue() {
			Assert.Empty( Repository.BlockingSelect() );
			TestEntity expected = new TestEntity( 2, "Value 2" );
			Repository.BlockingInsert( expected );

			Assert.True( Repository.BlockingDelete( expected.Id ) );
		}

		[Fact]
		public void Delete_NotFound() {
			Assert.Empty( Repository.BlockingSelect() );
			TestEntity expected = new TestEntity( 2, "Value 2" );
			Assert.False( Repository.BlockingDelete( expected.Id ) );
		}

		[Fact]
		public void Delete_Exceptional() {
			Assert.IsType<EvilValueException>(
				Record.Exception( () =>
					this.Repository.BlockingDelete( EvilValueException.EvilValue )
				)
			);
		}
	}
}
