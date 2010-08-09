﻿using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script.Tokens {
	[Terminal("FOR_PROPERTY")]
	public sealed class IndexForPropertyToken: IndexForToken {
		public IndexForPropertyToken() {
		}

		public override IndexFor IndexFor {
			get {
				return IndexFor.Property;
			}
		}
	}
}