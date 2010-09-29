﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

using bsn.ModuleStore.Sql.Script;

using NUnit.Framework;

namespace bsn.ModuleStore.Sql {
	[TestFixture]
	public class ScriptParserTest: AssertionHelper {
		[TestFixtureSetUp]
		public void Initialize() {
			ScriptParser.GetSemanticActions();
		}

		public List<Statement> ParseWithRoundtrip(string sql, int expectedStatementCount) {
			Stopwatch sw = new Stopwatch();
			sw.Start();
			IEnumerable<Statement> parsedStatements = ScriptParser.Parse(sql);
			sw.Stop();
			long parseTime = sw.ElapsedMilliseconds;
			List<Statement> statements = parsedStatements.ToList();
			Expect(statements.Count, EqualTo(expectedStatementCount));
			sw.Reset();
			sw.Start();
			string sqlGen = GenerateSql(statements);
			sw.Stop();
			Trace.Write(Environment.NewLine+sqlGen, string.Format("Generated SQL (parse: {0}ms | gen: {1}ms)", parseTime, sw.ElapsedMilliseconds));
			sw.Reset();
			sw.Start();
			IEnumerable<Statement> parsedStatementsRoundtrip = ScriptParser.Parse(sqlGen);
			string sqlGenRoundtrip = GenerateSql(parsedStatementsRoundtrip);
			sw.Stop();
			Trace.Write(string.Format("{0}ms", sw.ElapsedMilliseconds), "Roundtrip");
			Expect(sqlGen, EqualTo(sqlGenRoundtrip));
			return statements;
		}

		private string GenerateSql(IEnumerable<Statement> statements) {
			using (StringWriter stringWriter = new StringWriter()) {
				SqlWriter sqlGen = new SqlWriter(stringWriter);
				foreach (Statement statement in statements) {
					statement.WriteTo(sqlGen);
					sqlGen.WriteLine(";");
				}
				return stringWriter.ToString();
			}
		}

		[Test]
		public void GetGrammar() {
			Expect(ScriptParser.GetGrammar(), Not.Null);
		}

		[Test]
		public void GetSemanticActions() {
			Expect(ScriptParser.GetSemanticActions(), Not.Null);
		}

		[Test]
		public void ParseCreateComplexTableFunction() {
			ParseWithRoundtrip(
					@"CREATE FUNCTION SPLIT
(
  @s nvarchar(max),
  @trimPieces bit,
  @returnEmptyStrings bit
)
returns @t table (val nvarchar(max))
as
begin

declare @i int, @j int;
select @i = 0, @j = (len(@s) - len(replace(@s,',','')))

;with cte 
as
(
  select
    i = @i + 1,
    s = @s, 
    n = substring(@s, 0, charindex(',', @s)),
    m = substring(@s, charindex(',', @s)+1, len(@s) - charindex(',', @s))

  union all

  select 
    i = cte.i + 1,
    s = cte.m, 
    n = substring(cte.m, 0, charindex(',', cte.m)),
    m = substring(
      cte.m,
      charindex(',', cte.m) + 1,
      len(cte.m)-charindex(',', cte.m)
    )
  from cte
  where i <= @j
)
insert into @t (val)
select pieces
from 
(
  select 
  case 
    when @trimPieces = 1
    then ltrim(rtrim(case when i <= @j then n else m end))
    else case when i <= @j then n else m end
  end as pieces
  from cte
) t
where
  (@returnEmptyStrings = 0 and len(pieces) > 0)
  or (@returnEmptyStrings = 1)
option (maxrecursion 0);


return;

end",
					1);
		}

		[Test]
		public void ParseCreateProcedure() {
			ParseWithRoundtrip(
					@"CREATE PROCEDURE [dbo].[prcIndicatorStatusSet]
    @uidIndicatorStatus [uniqueidentifier],
    @bEditable [bit],
    @xMetadata [xml]
AS
    BEGIN
        SET NOCOUNT ON;
        SET XACT_ABORT ON;
        IF @uidIndicatorStatus IS NULL BEGIN
            SET @uidIndicatorStatus=NEWID();
            SET ROWCOUNT 0;
        END ELSE BEGIN
            UPDATE [dbo].[tblIndicatorStatus] WITH (UPDLOCK)
                SET [bEditable]=COALESCE(@bEditable, [bEditable]), [xMetadata]=COALESCE(@xMetadata, [xMetadata]) WHERE [tblIndicatorStatus].[uidIndicatorStatus]=@uidIndicatorStatus;
        END;
        IF @@ROWCOUNT=0 BEGIN
            INSERT 
                INTO [dbo].[tblIndicatorStatus] ([uidIndicatorStatus], [bEditable], [xMetadata])
                VALUES (@uidIndicatorStatus, @bEditable, @xMetadata);
        END;
        SELECT *
            FROM [dbo].[vwIndicatorStatus]
            WHERE [vwIndicatorStatus].[uidIndicatorStatus]=@uidIndicatorStatus;
    END;",
					1);
		}

		[Test]
		public void ParseSimpleCTESelect() {
			ParseWithRoundtrip(@"with MyCTE(x)
as
(
select x = convert(varchar(8000),'hello')
union all
select x + 'a' from MyCTE where len(x) < 100
)
select x from MyCTE", 1);
		}

		[Test]
		public void ParseWithComments() {
			ParseWithRoundtrip(@"-- Line comment
BEGIN
/* block comment
on 2 lines */
SELECT * -- Comment inside select
FROM [tbl];
-- Comment may move
END;", 1);
		}

		[Test]
		public void ParseWithMissingTerminator() {
			ParseWithRoundtrip(@"SELECT * FROM [tblA]
SELECT * FROM [tblB]
PRINT 'Cool'", 3);
		}

		[Test]
		public void StatementsWithXmlFunctions() {
			ParseWithRoundtrip(@"DECLARE @tbl TABLE (id xml);
INSERT @tbl (id) SELECT t.x FROM (SELECT NEWID() x FOR XML RAW) t(x);
SELECT id.[query]('data(*/@x)').query('*') FROM @tbl;", 3);
		}

		[Test]
		public void SelectWithCrossApplyAndXmlFunctions() {
			ParseWithRoundtrip(@"SELECT ProductModelID, Locations.value('./@LocationID','int') as LocID,
steps.query('.') as Step       
FROM Production.ProductModel       
CROSS APPLY Instructions.nodes('/MI:root/MI:Location') as T1(Locations)       
CROSS APPLY T1.Locations.nodes('/MI:step ') as T2(steps)       
WHERE ProductModelID=7;", 1);
		}

		[Test]
		public void SelectExcept() {
			ParseWithRoundtrip(@"SELECT * FROM TableA EXCEPT SELECT * FROM TableB", 1);
		}

		[Test]
		public void SelectWithNestedQueryWithXmlFunctions() {
			ParseWithRoundtrip(@"SELECT Fname, count(Fname) FROM    
  (SELECT nref.value('(author/first-name)[1]', 'nvarchar(max)') Fname
   FROM   docs CROSS APPLY xCol.nodes('/book') T(nref)
   WHERE  nref.exist ('author/first-name') = 1) Result
GROUP BY FName
ORDER BY Fname;", 1);
		}

		[Test]
		public void SelectWithXmlnamespaces() {
			ParseWithRoundtrip(@"WITH XMLNAMESPACES (
   'http://schemas.microsoft.com/sqlserver/2004/07/adventure-works/ProductModelManuInstructions' AS MI)
SELECT ProductModelID, 
       Locations.value('./@LocationID','int') as LocID,
       steps.query('.') as Steps
FROM   Production.ProductModel
CROSS APPLY Instructions.nodes('/MI:root/MI:Location') as T1(Locations)
CROSS APPLY T1.Locations.nodes('./MI:step') as T2(steps)
WHERE  ProductModelID=7
AND    steps.exist('./MI:tool') = 1;", 1);
		}

		[Test]
		public void SelectWithXmlnamespacesAndCte() {
			ParseWithRoundtrip(@"WITH XMLNAMESPACES (
   'http://schemas.microsoft.com/sqlserver/2004/07/adventure-works/ProductModelManuInstructions' AS MI),
cteModel AS (SELECt * FROM Production.ProductModel)
SELECT ProductModelID, 
       Locations.value('./@LocationID','int') as LocID,
       steps.query('.') as Steps
FROM   cteModel
CROSS APPLY Instructions.nodes('/MI:root/MI:Location') as T1(Locations)
CROSS APPLY T1.Locations.nodes('./MI:step') as T2(steps)
WHERE  ProductModelID=7
AND    steps.exist('./MI:tool') = 1;", 1);
		}

		[Test]
		public void SelectExceptIntersect() {
			ParseWithRoundtrip(@"SELECT * FROM TableA EXCEPT SELECT * FROM TableB INTERSECT SELECT * FROM TableC", 1);
		}

		[Test]
		public void SelectIntersect() {
			ParseWithRoundtrip(@"SELECT * FROM TableA INTERSECT SELECT * FROM TableB", 1);
		}

		[Test]
		public void BeginTransaction() {
			ParseWithRoundtrip(@"BEGIN TRAN", 1);
		}

		[Test]
		public void BeginTransactionIdentifierName() {
			ParseWithRoundtrip(@"BEGIN TRANSACTION MyTrans", 1);
		}

		[Test]
		public void BeginTransactionVariableName() {
			ParseWithRoundtrip(@"BEGIN TRANSACTION @trans", 1);
		}

		[Test]
		public void BeginTransactionWithMark() {
			ParseWithRoundtrip(@"BEGIN TRANSACTION MyTrans WITH MARK", 1);
		}

		[Test]
		public void BeginTransactionWithMarkNamed() {
			ParseWithRoundtrip(@"BEGIN TRANSACTION MyTrans WITH MARK 'My Trans'", 1);
		}

		[Test]
		public void CommitTransactionLegacy() {
			ParseWithRoundtrip(@"COMMIT", 1);
		}

		[Test]
		public void CommitTransaction() {
			ParseWithRoundtrip(@"COMMIT TRAN", 1);
		}

		[Test]
		public void CommitTransactionIdentifierName() {
			ParseWithRoundtrip(@"COMMIT TRANSACTION MyTrans", 1);
		}

		[Test]
		public void CommitTransactionVariableName() {
			ParseWithRoundtrip(@"COMMIT TRANSACTION @trans", 1);
		}

		[Test]
		public void RollbackTransactionLegacy() {
			ParseWithRoundtrip(@"ROLLBACK", 1);
		}

		[Test]
		public void RollbackTransaction() {
			ParseWithRoundtrip(@"ROLLBACK TRAN", 1);
		}

		[Test]
		public void RollbackTransactionIdentifierName() {
			ParseWithRoundtrip(@"ROLLBACK TRANSACTION MyTrans", 1);
		}

		[Test]
		public void RollbackTransactionVariableName() {
			ParseWithRoundtrip(@"ROLLBACK TRANSACTION @trans", 1);
		}

		[Test]
		public void SaveTransactionIdentifierName() {
			ParseWithRoundtrip(@"SAVE TRANSACTION MyTrans", 1);
		}

		[Test]
		public void SaveTransactionVariableName() {
			ParseWithRoundtrip(@"SAVE TRANSACTION @trans", 1);
		}

		[Test]
		public void SetTransactionIsolationLevel() {
			ParseWithRoundtrip(@"SET TRANSACTION ISOLATION LEVEL SERIALIZABLE", 1);
		}

		[Test]
		public void SyntaxError() {
			Expect(() => ParseWithRoundtrip(@"SELECT * FROM TableA 'Error'", 1), Throws.ArgumentException.With.Message.ContainsSubstring("SyntaxError"));
		}

		[Test]
		public void UpdateWithTableHint() {
			ParseWithRoundtrip(@"UPDATE Production.Product
WITH (TABLOCK)
SET ListPrice = ListPrice * 1.10
WHERE ProductNumber LIKE 'BK-%';", 1);
		}
	}
}