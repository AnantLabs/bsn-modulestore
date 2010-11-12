﻿// bsn ModuleStore database versioning
// -----------------------------------
// 
// Copyright 2010 by Arsène von Wyss - avw@gmx.ch
// 
// Development has been supported by Sirius Technologies AG, Basel
// 
// Source:
// 
// https://bsn-modulestore.googlecode.com/hg/
// 
// License:
// 
// The library is distributed under the GNU Lesser General Public License:
// http://www.gnu.org/licenses/lgpl.html
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//  
using System;

using bsn.ModuleStore.Mapper;

namespace bsn.ModuleStore.Bootstrapper {
	internal interface IModules: IStoredProcedures {
		[SqlProcedure("spModuleAdd.sql")]
		Module Add(Guid? id, Guid assemblyId, string schemaPrefix, string assemblyName);

		[SqlProcedure("spModuleDelete.sql", UseReturnValue = SqlReturnValue.ReturnValue)]
		bool Delete(Guid id);

		[SqlProcedure("spModuleList.sql")]
		ResultSet<Module> List(Guid assemblyGuid);

		[SqlProcedure("spModuleUpdate.sql", UseReturnValue = SqlReturnValue.ReturnValue)]
		bool Update(Guid id, string assemblyName, byte[] setupHash, int updateVersion);

		[SqlProcedure("spUserObjectList.sql")]
		ResultSet<DatabaseObject, ResultSet<DatabaseIndex, ResultSet<DatabaseXmlSchema>>> UserObjectList(string schemaName);
	}
}
