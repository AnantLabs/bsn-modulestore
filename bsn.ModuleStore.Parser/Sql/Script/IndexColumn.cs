﻿using System;
using System.Data.SqlClient;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class IndexColumn: SqlScriptableToken {
		private readonly ColumnName columnName;
		private readonly SortOrder order;

		[Rule("<IndexColumn> ::= <ColumnName> <OrderType>")]
		public IndexColumn(ColumnName columnName, OrderTypeToken order) {
			this.columnName = columnName;
			this.order = order.Order;
		}

		public ColumnName ColumnName {
			get {
				return columnName;
			}
		}

		public SortOrder Order {
			get {
				return order;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.WriteScript(columnName, WhitespacePadding.None);
			writer.WriteEnum(order, WhitespacePadding.SpaceBefore);
		}
	}
}