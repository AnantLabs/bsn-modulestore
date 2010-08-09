﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class DeleteStatement: SqlStatement {
		private readonly List<CommonTableExpression> ctes;
		private readonly DestinationRowset destinationRowset;
		private readonly FromClause fromClause;
		private readonly OutputClause outputClause;
		private readonly TopExpression topExpression;
		private readonly Predicate whereClause;

		[Rule("<DeleteStatement> ::= <CTEGroup> DELETE <Top> <OptionalFrom> <DestinationRowset> <OutputClause> <OptionalFromClause> <WhereClause>", ConstructorParameterMapping = new[] {0, 2, 4, 5, 6, 7})]
		public DeleteStatement(Optional<Sequence<CommonTableExpression>> ctes, TopExpression topExpression, DestinationRowset destinationRowset, OutputClause outputClause, Optional<FromClause> fromClause, Optional<Predicate> whereClause) {
			this.ctes = ctes.ToList();
			this.topExpression = topExpression;
			this.destinationRowset = destinationRowset;
			this.outputClause = outputClause;
			this.fromClause = fromClause;
			this.whereClause = whereClause;
		}

		public override void WriteTo(TextWriter writer) {
			writer.WriteCommonTableExpressions(ctes);
			writer.Write("DELETE");
			writer.WriteScript(topExpression, " ", null);
			writer.Write(" FROM ");
			writer.WriteScript(destinationRowset);
			writer.WriteScript(outputClause, " ", null);

		}
	}
}