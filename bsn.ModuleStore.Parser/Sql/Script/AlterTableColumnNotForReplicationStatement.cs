using System;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class AlterTableColumnNotForReplicationStatement: AlterTableColumnAttributeStatement {
		[Rule("<AlterTableStatement> ::= ~ALTER ~TABLE <TableNameQualified> ~ALTER ~COLUMN <ColumnName> ADD ~NOT ~FOR_REPLICATION")]
		[Rule("<AlterTableStatement> ::= ~ALTER ~TABLE <TableNameQualified> ~ALTER ~COLUMN <ColumnName> DROP ~NOT ~FOR_REPLICATION")]
		public AlterTableColumnNotForReplicationStatement(Qualified<SchemaName, TableName> tableName, ColumnName columnName, DdlOperationToken ddlOperationToken): base(tableName, columnName, ddlOperationToken) {}

		public override void WriteTo(SqlWriter writer) {
			base.WriteTo(writer);
			writer.Write("NOT FOR REPLICATION");
		}
	}
}