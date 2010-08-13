using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	[Terminal("COALESCE")]
	[Terminal("CONVERT")]
	[Terminal("NULLIF")]
	public sealed class FunctionName: SqlName {
		private readonly bool systemFunction;

		public FunctionName(string name): base(name.ToUpperInvariant()) {}

		[Rule("<FunctionName> ::= Id")]
		[Rule("<FunctionName> ::= SystemFuncId")]
		public FunctionName(SqlIdentifier identifier): base(identifier.Value) {
			systemFunction = identifier is SysFunctionIdentifier;
		}

		public bool IsSystemFunction {
			get {
				return systemFunction;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			if (systemFunction) {
				writer.Write(Value);
			} else {
				base.WriteTo(writer);
			}
		}
	}
}