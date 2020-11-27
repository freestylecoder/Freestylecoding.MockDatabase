using System;
using System.Data;
using System.Data.Common;

namespace Freestylecoding.MockDatabase {
	public class MockTransaction : DbTransaction {
		public bool IsCommitted { get; protected set; }
		public bool IsRolledBack { get; protected set; }

		internal MockTransaction( DbConnection dbConnection, IsolationLevel level )
			: base() {
			this.IsCommitted = false;
			this.IsRolledBack = false;

			this.isolationLevel = level;
			this.connection = dbConnection;
		}

		private IsolationLevel isolationLevel;
		public override IsolationLevel IsolationLevel => isolationLevel;

		private DbConnection connection;
		protected override DbConnection DbConnection => connection;

		public override void Commit() {
			if( IsCommitted || IsRolledBack )
				throw new InvalidOperationException();

			IsCommitted = true;
		}
		public override void Rollback() {
			if( IsCommitted || IsRolledBack )
				throw new InvalidOperationException();

			IsRolledBack = true;
		}
	}
}
