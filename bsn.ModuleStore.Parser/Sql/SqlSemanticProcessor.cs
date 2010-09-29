﻿using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

using bsn.GoldParser.Grammar;
using bsn.GoldParser.Parser;
using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql {
	internal class SqlSemanticProcessor: SemanticProcessor<SqlToken> {
		private readonly SqlTokenizer tokenizer;
		private readonly Symbol terminatorSymbol;

		public SqlSemanticProcessor(TextReader reader, SemanticActions<SqlToken> actions): this(new SqlTokenizer(reader, actions), actions) {}

		public SqlSemanticProcessor(SqlTokenizer tokenizer, SemanticActions<SqlToken> actions): base(tokenizer, actions) {
			terminatorSymbol = actions.Grammar.GetSymbolByName("';'");
			Debug.Assert(terminatorSymbol != null);
			this.tokenizer = tokenizer;
		}

		protected override bool RetrySyntaxError(ref SqlToken currentToken) {
			if (((IToken)currentToken).Symbol != terminatorSymbol) {
				tokenizer.RepeatToken();
				InsignificantToken terminator = new InsignificantToken();
				terminator.InitializeInternal(terminatorSymbol, ((IToken)currentToken).Position);
				currentToken = terminator;
				return true;
			}
			return false;
		}

		protected override SqlToken CreateReduction(Rule rule, IList<SqlToken> children) {
			SqlToken result = base.CreateReduction(rule, children);
			CommentContainerToken commentToken = result as CommentContainerToken;
			if (commentToken != null) {
				foreach (IToken token in children) {
					if ((token != null) && (token.Position.Line > 0)) {
						commentToken.AddComments(tokenizer.GetComments(token.Position.Index));
						break;
					}
				}
			}
			return result;
		}
	}
}