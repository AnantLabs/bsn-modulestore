﻿using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class ExpressionSearchedCase: ExpressionCase<Predicate> {
		[Rule("<ExpressionCase> ::= CASE <CaseWhenPredicateList> END", ConstructorParameterMapping = new[] {1})]
		public ExpressionSearchedCase(Sequence<CaseWhen<Predicate>> whenItems): this(whenItems, null) {}

		[Rule("<ExpressionCase> ::= CASE <CaseWhenPredicateList> ELSE <Expression> END", ConstructorParameterMapping = new[] {1, 3})]
		public ExpressionSearchedCase(Sequence<CaseWhen<Predicate>> whenItems, Expression elseExpression): base(whenItems, elseExpression) {}

		public override void WriteTo(SqlWriter writer) {
			writer.Write("CASE");
			base.WriteTo(writer);
		}
	}
}