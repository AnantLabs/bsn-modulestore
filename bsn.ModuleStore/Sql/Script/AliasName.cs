﻿using System;
using System.Linq;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class AliasName: SqlQuotedName {
		[Rule("<AliasName> ::= Id")]
		public AliasName(Identifier identifier): base(identifier.Value) {}
	}
}