﻿using System;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class PredicateExists: Predicate {
		private readonly SelectQuery selectQuery;

		[Rule("<PredicateFunction> ::= ~EXISTS ~'(' <SelectQuery> ~')'")]
		public PredicateExists(SelectQuery selectQuery) {
			Debug.Assert(selectQuery != null);
			this.selectQuery = selectQuery;
		}

		public SelectQuery SelectQuery {
			get {
				return selectQuery;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			WriteCommentsTo(writer);
			writer.Write("EXISTS (");
			writer.WriteScript(selectQuery, WhitespacePadding.None);
			writer.Write(')');
		}
	}
}