using System;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	[Terminal("ANY")]
	[Terminal("AUTHORIZATION")]
	[Terminal("BACKUP")]
	[Terminal("BULK")]
	[Terminal("CHECKPOINT")]
	[Terminal("COMMIT")]
	[Terminal("COMPUTE")]
	[Terminal("CONTAINS")]
	[Terminal("CONTAINSTABLE")]
	[Terminal("CONVERT")]
	[Terminal("CURRENT")]
	[Terminal("CURRENT_DATE")]
	[Terminal("CURRENT_TIME")]
	[Terminal("CURRENT_TIMESTAMP")]
	[Terminal("CURRENT_USER")]
	[Terminal("DATABASE")]
	[Terminal("DBCC")]
	[Terminal("DENY")]
	[Terminal("DISK")]
	[Terminal("DISTRIBUTED")]
	[Terminal("DOUBLE")]
	[Terminal("DUMP")]
	[Terminal("ERRLVL")]
	[Terminal("EXCEPT")]
	[Terminal("EXIT")]
	[Terminal("EXTERNAL")]
	[Terminal("FETCH")]
	[Terminal("FILE")]
	[Terminal("FILLFACTOR")]
	[Terminal("FREETEXT")]
	[Terminal("FREETEXTTABLE")]
	[Terminal("GRANT")]
	[Terminal("HOLDLOCK")]
	[Terminal("IDENTITYCOL")]
	[Terminal("INTERSECT")]
	[Terminal("KILL")]
	[Terminal("LINENO")]
	[Terminal("LOAD")]
	[Terminal("MERGE")]
	[Terminal("NATIONAL")]
	[Terminal("NULLIF")]
	[Terminal("OFFSETS")]
	[Terminal("OPENDATASOURCE")]
	[Terminal("OPENQUERY")]
	[Terminal("OPENROWSET")]
	[Terminal("OPTION")]
	[Terminal("PIVOT")]
	[Terminal("PLAN")]
	[Terminal("PRECISION")]
	[Terminal("READ")]
	[Terminal("READTEXT")]
	[Terminal("RECONFIGURE")]
	[Terminal("RESTORE")]
	[Terminal("RESTRICT")]
	[Terminal("REVERT")]
	[Terminal("REVOKE")]
	[Terminal("ROLLBACK")]
	[Terminal("ROWCOUNT")]
	[Terminal("RULE")]
	[Terminal("SAVE")]
	[Terminal("SCHEMA")]
	[Terminal("SECURITYAUDIT")]
	[Terminal("SESSION_USER")]
	[Terminal("SETUSER")]
	[Terminal("SHUTDOWN")]
	[Terminal("SOME")]
	[Terminal("STATISTICS")]
	[Terminal("SYSTEM_USER")]
	[Terminal("TABLESAMPLE")]
	[Terminal("TEXTSIZE")]
	[Terminal("TO")]
	[Terminal("TRAN")]
	[Terminal("TRANSACTION")]
	[Terminal("TRUNCATE")]
	[Terminal("TSEQUAL")]
	[Terminal("UNPIVOT")]
	[Terminal("UPDATETEXT")]
	[Terminal("USE")]
	[Terminal("USER")]
	[Terminal("WRITETEXT")]
	public sealed class ReservedWord: SqlToken, IScriptable {
		private readonly string text;

		public ReservedWord(string text) {
			this.text = text.ToUpperInvariant();
		}

		public void WriteTo(TextWriter writer) {
			writer.Write(text);
		}
	}
}