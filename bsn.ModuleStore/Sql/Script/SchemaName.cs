using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class SchemaName: SqlQuotedName {
		[Rule("<SchemaName> ::= Id")]
		public SchemaName(Identifier identifier): base(identifier.Value) {}
	}
}