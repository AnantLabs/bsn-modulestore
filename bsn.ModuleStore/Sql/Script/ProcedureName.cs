using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class ProcedureName: SqlQuotedName {
		[Rule("<ProcedureName> ::= Id")]
		public ProcedureName(Identifier identifier): base(identifier.Value) {}
	}
}