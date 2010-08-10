﻿using System;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class ColumnDefaultParensConstraint: ColumnDefaultConstraint {
		[Rule("<ColumnConstraint> ::= DEFAULT '(' <FunctionCall> ')'", ConstructorParameterMapping = new[] {2})]
		public ColumnDefaultParensConstraint(Expression defaultValue): base(defaultValue) {}

		public override void WriteTo(TextWriter writer) {
			writer.Write("DEFAULT (");
			writer.WriteScript(DefaultValue);
			writer.Write(')');
		}
	}
}