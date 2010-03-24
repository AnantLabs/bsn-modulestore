﻿// (C) 2010 Arsène von Wyss / bsn
using System;
using System.Runtime.Serialization;

namespace bsn.ModuleStore.Sql.Definitions {
	[Serializable]
	public class View: SqlObject {
		public View(string name): base(name) {}
		protected View(SerializationInfo info, StreamingContext context): base(info, context) {}

		public override SqlObjectKind Kind {
			get {
				return SqlObjectKind.View;
			}
		}
	}
}