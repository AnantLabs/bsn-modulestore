﻿using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class FullOuterJoin: PredicateJoin {
		[Rule("<Join> ::= FULL JOIN <SourceRowset> ON <Predicate>", ConstructorParameterMapping = new[] {2, 4})]
		[Rule("<Join> ::= FULL OUTER JOIN <SourceRowset> ON <Predicate>", ConstructorParameterMapping = new[] {3, 5})]
		public FullOuterJoin(SourceRowset joinRowset, Predicate predicate): base(joinRowset, predicate) {}

		public override JoinKind Kind {
			get {
				return JoinKind.Full;
			}
		}
	}
}