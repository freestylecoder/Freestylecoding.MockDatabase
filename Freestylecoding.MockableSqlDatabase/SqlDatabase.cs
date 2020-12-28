using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using Freestylecoding.MockableDatabase;

namespace Freestylecoding.MockableSqlDatabase {
	public class SqlDatabase : IDatabase {
		public SqlDatabase( string connectionString ) {
			this.ConnectionString = connectionString;
		}

		public DbConnection GetConnection() =>
			// TODO: If we decide to do connection pooling
			//	this is where it would go.
			new SqlConnection( this.ConnectionString );
		public DbParameter CreateParameter( string parameterName, DbType dbType, object value ) =>
			new SqlParameter {
				Value = value ?? DBNull.Value,
				ParameterName = parameterName,
				DbType = dbType
			};
		public DbParameter CreateParameter( string parameterName, DbType dbType, int size, object value ) =>
			new SqlParameter {
				Value = value ?? DBNull.Value,
				ParameterName = parameterName,
				DbType = dbType,
				Size = size
			};

		private readonly string ConnectionString;
	}
}
