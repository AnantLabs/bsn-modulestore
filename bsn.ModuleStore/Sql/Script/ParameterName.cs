using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class ParameterName: SqlQuotedName {
		[Rule("<ParameterName> ::= LocalId")]
		public ParameterName(LocalIdentifier identifier): base(identifier.Value) {}
	}
}