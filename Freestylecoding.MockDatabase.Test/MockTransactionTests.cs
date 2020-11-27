using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Freestylecoding.MockDatabase.Test {
	public class MockTransactionTests {
		private readonly MockConnection Connection;
		private readonly MockTransaction Transaction;

		public MockTransactionTests() {
			this.Connection = new MockConnection();
			this.Transaction = Connection.BeginTransaction() as MockTransaction;
		}

		[Fact]
		public void Defaults() {
			Assert.Equal( Connection, Transaction.Connection );
			Assert.Equal( System.Data.IsolationLevel.Unspecified, Transaction.IsolationLevel );
			Assert.False( Transaction.IsCommitted );
			Assert.False( Transaction.IsRolledBack );
		}

		[Fact]
		public void Commit() {
			Assert.False( Transaction.IsCommitted );
			Assert.False( Transaction.IsRolledBack );

			Transaction.Commit();

			Assert.True( Transaction.IsCommitted );
			Assert.False( Transaction.IsRolledBack );
		}

		[Fact]
		public void Rollback() {
			Assert.False( Transaction.IsCommitted );
			Assert.False( Transaction.IsRolledBack );

			Transaction.Rollback();

			Assert.False( Transaction.IsCommitted );
			Assert.True( Transaction.IsRolledBack );
		}

		[Fact]
		public void CommitCommit() {
			Transaction.Commit();
			Assert.IsType<InvalidOperationException>(
				Record.Exception( () => Transaction.Commit() )
			);
		}

		[Fact]
		public void CommitRollback() {
			Transaction.Commit();
			Assert.IsType<InvalidOperationException>(
				Record.Exception( () => Transaction.Rollback() )
			);
		}

		[Fact]
		public void RollbackCommit() {
			Transaction.Rollback();
			Assert.IsType<InvalidOperationException>(
				Record.Exception( () => Transaction.Commit() )
			);
		}

		[Fact]
		public void RollbackRollback() {
			Transaction.Rollback();
			Assert.IsType<InvalidOperationException>(
				Record.Exception( () => Transaction.Rollback() )
			);
		}
	}
}
