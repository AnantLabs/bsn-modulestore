﻿using System;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class CaseWhen<T>: SqlToken, IScriptable where T: SqlComputable {
		private readonly T condition;
		private readonly Expression valueExpression;

		[Rule("<CaseWhenExpression> ::= WHEN <Expression> THEN <Expression>", typeof(Expression), ConstructorParameterMapping = new[] {1, 3})]
		[Rule("<CaseWhenPredicate> ::= WHEN <Predicate> THEN <Expression>", typeof(Predicate), ConstructorParameterMapping = new[] {1, 3})]
		public CaseWhen(T condition, Expression valueExpression) {
			this.condition = condition;
			this.valueExpression = valueExpression;
		}

		public T Condition {
			get {
				return condition;
			}
		}

		public Expression ValueExpression {
			get {
				return valueExpression;
			}
		}

		public void WriteTo(TextWriter writer) {
			writer.Write("WHEN ");
			writer.WriteScript(condition);
			writer.Write(" THEN ");
			writer.WriteScript(valueExpression);
		}
	}
}