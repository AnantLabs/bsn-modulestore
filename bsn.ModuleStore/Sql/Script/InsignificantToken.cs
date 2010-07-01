﻿using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	[Terminal("(EOF)")]
	[Terminal("(Error)")]
	[Terminal("(Whitespace)")]
	[Terminal("(Comment End)")]
	[Terminal("(Comment Line)")]
	[Terminal("(Comment Start)")]
	[Terminal("(")]
	[Terminal(")")]
	[Terminal(".")]
	[Terminal(",")]
	[Terminal(";")]
	[Terminal("_RETURNS")]
	[Terminal("ADD")]
	[Terminal("ADD_PERSISTED")]
	[Terminal("ALL")]
	[Terminal("ALTER")]
	[Terminal("AS")]
	[Terminal("ASC")]
	[Terminal("BEGIN")]
	[Terminal("BEGIN_CATCH")]
	[Terminal("BEGIN_TRY")]
	[Terminal("BETWEEN")]
	[Terminal("BREAK")]
	[Terminal("BROWSE")]
	[Terminal("BY")]
	[Terminal("CALLED_ON_NULL_INPUT")]
	[Terminal("CASCADE")]
	[Terminal("CASE")]
	[Terminal("CAST_")]
	[Terminal("CHECK")]
	[Terminal("CLOSE")]
	[Terminal("CLUSTERED")]
	[Terminal("COALESCE")]
	[Terminal("COLLATE")]
	[Terminal("COLUMN")]
	[Terminal("CONSTRAINT")]
	[Terminal("CONTINUE")]
	[Terminal("COUNT_")]
	[Terminal("CREATE")]
	[Terminal("CROSS")]
	[Terminal("CURSOR")]
	[Terminal("DEALLOCATE")]
	[Terminal("DECLARE")]
	[Terminal("DEFAULT")]
	[Terminal("DELETE")]
	[Terminal("DESC")]
	[Terminal("DISABLE_TRIGGER")]
	[Terminal("DISTINCT")]
	[Terminal("DROP")]
	[Terminal("DROP_PERSISTED")]
	[Terminal("ELSE")]
	[Terminal("ENABLE_TRIGGER")]
	[Terminal("END")]
	[Terminal("END_CATCH")]
	[Terminal("END_TRY")]
	[Terminal("ESCAPE")]
	[Terminal("EXECUTE")]
	[Terminal("EXISTS")]
	[Terminal("FOR")]
	[Terminal("FOR_PATH")]
	[Terminal("FOR_PROPERTY")]
	[Terminal("FOR_VALUE")]
	[Terminal("FOR_XML")]
	[Terminal("FOREIGN")]
	[Terminal("FROM")]
	[Terminal("FULL")]
	[Terminal("FULLTEXT_INDEX")]
	[Terminal("FUNCTION")]
	[Terminal("GOTO")]
	[Terminal("GROUP")]
	[Terminal("HAVING")]
	[Terminal("IDENTITY")]
	[Terminal("IDENTITY_INSERT")]
	[Terminal("IF")]
	[Terminal("IN")]
	[Terminal("INCLUDE_")]
	[Terminal("INDEX")]
	[Terminal("INNER")]
	[Terminal("INSERT")]
	[Terminal("INSTEAD_OF")]
	[Terminal("INTO")]
	[Terminal("IS")]
	[Terminal("JOIN")]
	[Terminal("KEY")]
	[Terminal("LANGUAGE_LCID")]
	[Terminal("LEFT")]
	[Terminal("LIKE")]
	[Terminal("NO_ACTION")]
	[Terminal("NO_POPULATION")]
	[Terminal("NOCHECK")]
	[Terminal("NONCLUSTERED")]
	[Terminal("NULL")]
	[Terminal("OF")]
	[Terminal("OFF")]
	[Terminal("ON")]
	[Terminal("OPEN")]
	[Terminal("OPENXML")]
	[Terminal("ORDER")]
	[Terminal("OUTER")]
	[Terminal("OUTPUT")]
	[Terminal("OVER")]
	[Terminal("PARTITION_BY")]
	[Terminal("PERCENT")]
	[Terminal("PRIMARY")]
	[Terminal("PRINT")]
	[Terminal("PROCEDURE")]
	[Terminal("PUBLIC")]
	[Terminal("RAISERROR")]
	[Terminal("READ_ONLY")]
	[Terminal("REFERENCES")]
	[Terminal("REPLICATION")]
	[Terminal("RETURN")]
	[Terminal("RETURNS_NULL_ON_NULL_INPUT")]
	[Terminal("RIGHT")]
	[Terminal("ROWGUIDCOL")]
	[Terminal("SELECT")]
	[Terminal("SET")]
	[Terminal("TABLE")]
	[Terminal("THEN")]
	[Terminal("TOP")]
	[Terminal("TRIGGER")]
	[Terminal("TYPE_COLUMN")]
	[Terminal("UNION")]
	[Terminal("UNIQUE")]
	[Terminal("UPDATE")]
	[Terminal("USING_XML_INDEX")]
	[Terminal("VALUES")]
	[Terminal("VARYING")]
	[Terminal("VIEW")]
	[Terminal("WAITFOR")]
	[Terminal("WHEN")]
	[Terminal("WHERE")]
	[Terminal("WHILE")]
	[Terminal("WITH")]
	[Terminal("WITH_CHANGE_TRACKING")]
	[Terminal("WITH_CHECK_OPTION")]
	[Terminal("WITH_FILLFACTOR")]
	[Terminal("WITH_RECOMPILE")]
	[Terminal("WITH_VIEW_METADATA")]
	[Terminal("XML_INDEX")]
	[Terminal("XML_SCHEMA_COLLECTION")]
	public class InsignificantToken: SqlToken {
		[Rule("<OptionalAs> ::= AS", AllowTruncation = true)]
		[Rule("<OptionalAs> ::=")]
		[Rule("<Terminator> ::= <Terminator> ';'", AllowTruncation=true)]
		public InsignificantToken() {
		}
	}
}

