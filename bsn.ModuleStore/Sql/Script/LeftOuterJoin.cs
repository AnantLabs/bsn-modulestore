﻿using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class LeftOuterJoin: PredicateJoin {
		[Rule("<Join> ::= LEFT JOIN <SourceRowset> ON <Predicate>", ConstructorParameterMapping = new[] {2, 4})]
		[Rule("<Join> ::= LEFT OUTER JOIN <SourceRowset> ON <Predicate>", ConstructorParameterMapping = new[] {3, 5})]
		public LeftOuterJoin(SourceRowset joinRowset, Predicate predicate): base(joinRowset, predicate) {}

		public override JoinKind Kind {
			get {
				return JoinKind.Left;
			}
		}
	}
}