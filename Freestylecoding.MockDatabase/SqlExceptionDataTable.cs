using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace Freestylecoding.MockDatabase {
	internal class SqlExceptionDataTable : DataTable {
		public readonly string Message;
		public readonly SqlError[] ErrorCollection;
		public readonly Exception InnerException;
		public readonly Guid ConnectionId;

		internal SqlExceptionDataTable(
			string message = null,
			SqlError[] errorCollection = null,
			Exception innerException = null,
			Guid conId = default
		) {
			this.Message = message;
			this.ErrorCollection = errorCollection;
			this.InnerException = innerException;
			this.ConnectionId = conId;
		}

		internal void Throw() {
			throw typeof( SqlException )
				.GetConstructors( BindingFlags.NonPublic | BindingFlags.Instance )
				.Where( z => z.GetParameters().Length == 4 )
				.Single()
				.Invoke( new object[] { Message, ErrorCollection, InnerException, ConnectionId } )
				as SqlException;
		}
	}
}
