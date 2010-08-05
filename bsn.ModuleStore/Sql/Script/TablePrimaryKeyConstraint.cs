﻿using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class TablePrimaryKeyConstraint: TableUniqueConstraintBase {
		[Rule("<TableConstraint> ::= PRIMARY KEY <ConstraintCluster> '(' <IndexColumnList> ')' <ConstraintIndex>", ConstructorParameterMapping = new[] {2, 4, 6})]
		public TablePrimaryKeyConstraint(Clustered clustered, Sequence<IndexColumn> indexColumns, ConstraintIndex constraintIndex): this(null, clustered, indexColumns, constraintIndex) {}

		[Rule("<TableConstraint> ::= CONSTRAINT <ConstraintName> PRIMARY KEY <ConstraintCluster> '(' <IndexColumnList> ')' <ConstraintIndex>", ConstructorParameterMapping = new[] {1, 4, 6, 8})]
		public TablePrimaryKeyConstraint(ConstraintName constraintName, Clustered clustered, Sequence<IndexColumn> indexColumns, ConstraintIndex constraintIndex): base(constraintName, clustered, indexColumns, constraintIndex) {}
	}
}