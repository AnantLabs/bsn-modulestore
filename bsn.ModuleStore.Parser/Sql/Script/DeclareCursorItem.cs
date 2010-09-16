using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class DeclareCursorItem: DeclareItem {
		[Rule("<DeclareItem> ::= <VariableName> ~CURSOR")]
		public DeclareCursorItem(VariableName variable): base(variable) {}

		public override void WriteTo(SqlWriter writer) {
			base.WriteTo(writer);
			writer.Write("CURSOR");
		}
	}
}