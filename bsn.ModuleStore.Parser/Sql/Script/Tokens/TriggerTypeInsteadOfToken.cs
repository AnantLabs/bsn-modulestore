using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script.Tokens {
	public class TriggerTypeInsteadOfToken: TriggerTypeToken {
		[Rule("<TriggerType> ::= ~INSTEAD_OF")]
		public TriggerTypeInsteadOfToken() {}

		public override TriggerType TriggerType {
			get {
				return TriggerType.InsteadOf;
			}
		}
	}
}