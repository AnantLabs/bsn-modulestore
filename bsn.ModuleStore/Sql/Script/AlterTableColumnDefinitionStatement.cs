using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class AlterTableColumnDefinitionStatement: AlterTableColumnStatement {
		private readonly ColumnDefinition definition;

		[Rule("<AlterTableStatement> ::= ALTER TABLE <TableName> ALTER COLUMN <ColumnName> <ColumnDefinition>", ConstructorParameterMapping = new[] {2, 5, 6})]
		public AlterTableColumnDefinitionStatement(TableName tableName, ColumnName columnName, ColumnDefinition definition): base(tableName, columnName) {
			if (definition == null) {
				throw new ArgumentNullException("definition");
			}
			this.definition = definition;
		}

		public override void ApplyTo(CreateTableStatement createTable) {
			throw new NotImplementedException();
		}
	}
}