using System;
using System.Diagnostics;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class AlterTableDropColumnStatement: AlterTableStatement {
		private readonly ColumnName columnName;

		[Rule("<AlterTableStatement> ::= ALTER TABLE <TableName> DROP COLUMN <ColumnName>", ConstructorParameterMapping = new[] {2, 5})]
		public AlterTableDropColumnStatement(TableName tableName, ColumnName columnName): base(tableName) {
			Debug.Assert(columnName != null);
			this.columnName = columnName;
		}

		public ColumnName ColumnName {
			get {
				return columnName;
			}
		}

		public override void ApplyTo(CreateTableStatement createTable) {
			throw new NotImplementedException();
		}

		public override void WriteTo(SqlWriter writer) {
			base.WriteTo(writer);
			writer.Write("DROP COLUMN ");
			writer.WriteScript(columnName);
		}
	}
}