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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Reflection;

using Microsoft.SqlServer.Server;

namespace bsn.ModuleStore.Mapper.Serialization {
	internal class SqlSerializationTypeMapping {
		private static readonly Dictionary<Type, SqlDbType> dbTypeMapping = new Dictionary<Type, SqlDbType> {
		                                                                                                    		{typeof(long), SqlDbType.BigInt},
		                                                                                                    		{typeof(byte[]), SqlDbType.VarBinary},
		                                                                                                    		{typeof(bool), SqlDbType.Bit},
		                                                                                                    		{typeof(char), SqlDbType.NChar},
		                                                                                                    		{typeof(char[]), SqlDbType.NVarChar},
		                                                                                                    		{typeof(string), SqlDbType.NVarChar},
		                                                                                                    		{typeof(DateTime), SqlDbType.DateTime},
		                                                                                                    		{typeof(DateTimeOffset), SqlDbType.DateTimeOffset},
		                                                                                                    		{typeof(decimal), SqlDbType.Decimal},
		                                                                                                    		{typeof(float), SqlDbType.Real},
		                                                                                                    		{typeof(double), SqlDbType.Float},
		                                                                                                    		{typeof(int), SqlDbType.Int},
		                                                                                                    		{typeof(short), SqlDbType.SmallInt},
		                                                                                                    		{typeof(sbyte), SqlDbType.TinyInt},
		                                                                                                    		{typeof(Guid), SqlDbType.UniqueIdentifier}
		                                                                                                    };

		private static readonly Dictionary<Type, SqlSerializationTypeMapping> mappings = new Dictionary<Type, SqlSerializationTypeMapping>();

		public static SqlSerializationTypeMapping Get(Type type) {
			if (type == null) {
				throw new ArgumentNullException("type");
			}
			SqlSerializationTypeMapping result;
			lock (mappings) {
				if (!mappings.TryGetValue(type, out result)) {
					result = new SqlSerializationTypeMapping(type);
					mappings.Add(type, result);
				}
			}
			return result;
		}

		internal static IEnumerable<MemberInfo> GetAllFieldsAndProperties(Type type) {
			while (type != null) {
				foreach (FieldInfo field in type.GetFields(BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.DeclaredOnly)) {
					yield return field;
				}
				foreach (PropertyInfo property in type.GetProperties(BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.DeclaredOnly)) {
					yield return property;
				}
				type = type.BaseType;
			}
		}

		public static Type GetMemberType(MemberInfo memberInfo) {
			if (memberInfo == null) {
				throw new ArgumentNullException("memberInfo");
			}
			FieldInfo fieldInfo = memberInfo as FieldInfo;
			if (fieldInfo != null) {
				return fieldInfo.FieldType;
			}
			PropertyInfo propertyInfo = memberInfo as PropertyInfo;
			if (propertyInfo != null) {
				return propertyInfo.PropertyType;
			}
			throw new ArgumentException("Only fields and properties are supported", "memberInfo");
		}

		/// <summary>
		/// Get the matching <see cref="DbType"/> for the given type.
		/// </summary>
		/// <param name="type">The .NET type to match.</param>
		/// <returns>The <see cref="DbType"/> best matching the given .NET type, or <see cref="DbType.Object"/> otherwise.</returns>
		public static SqlDbType GetTypeMapping(Type type) {
			if (type != null) {
				if (type.IsByRef && type.HasElementType) {
					type = type.GetElementType();
					Debug.Assert(type != null);
				} else {
					type = Nullable.GetUnderlyingType(type) ?? type;
				}
				SqlDbType result;
				lock (dbTypeMapping) {
					if (dbTypeMapping.TryGetValue(type, out result)) {
						return result;
					}
					if (SqlSerializationTypeInfo.IsXmlType(type)) {
						result = SqlDbType.Xml;
					} else {
						Type dummyType;
						if (SqlSerializationTypeInfo.TryGetIEnumerableElementType(type, out dummyType)) {
							result = SqlDbType.Structured;
						} else {
							if (type.IsDefined(typeof(SqlUserDefinedTypeAttribute), false)) {
								result = SqlDbType.Udt;
							} else {
								result = SqlDbType.Variant;
							}
						}
					}
					dbTypeMapping.Add(type, result);
				}
				return result;
			}
			return SqlDbType.Variant;
		}

		internal static bool IsNativeType(Type type) {
			lock (dbTypeMapping) {
				SqlDbType dbType;
				if (dbTypeMapping.TryGetValue(type, out dbType)) {
					return dbType != SqlDbType.Variant;
				}
			}
			return false;
		}

		private readonly Dictionary<string, SqlColumnInfo> columns = new Dictionary<string, SqlColumnInfo>(StringComparer.OrdinalIgnoreCase);
		private readonly ReadOnlyCollection<MemberConverter> converters;
		private readonly bool hasNestedSerializers;
		private readonly MemberInfo[] members;
		private readonly MembersMethods methods;

		private SqlSerializationTypeMapping(Type type) {
			if (type == null) {
				throw new ArgumentNullException("type");
			}
			List<MemberConverter> memberConverters = new List<MemberConverter>();
			List<MemberInfo> memberInfos = new List<MemberInfo>();
			if (!(type.IsPrimitive || type.IsInterface || (typeof(string) == type))) {
				bool hasIdentity = false;
				foreach (MemberInfo member in GetAllFieldsAndProperties(type)) {
					SqlColumnAttribute columnAttribute = SqlColumnAttribute.GetColumnAttribute(member, false);
					Type memberType = GetMemberType(member);
					if (columnAttribute != null) {
						AssertValidMember(member);
						bool isIdentity = (!hasIdentity) && (hasIdentity |= columnAttribute.Identity);
						MemberConverter memberConverter;
						if (columnAttribute.GetCachedByIdentity) {
							memberConverter = new CachedMemberConverter(memberType, isIdentity, columnAttribute.Name, memberInfos.Count, columnAttribute.DateTimeKind);
						} else {
							memberConverter = MemberConverter.Get(memberType, isIdentity, columnAttribute.Name, memberInfos.Count, columnAttribute.DateTimeKind);
						}
						memberConverters.Add(memberConverter);
						memberInfos.Add(member);
						columns.Add(columnAttribute.Name, new SqlColumnInfo(member, columnAttribute.Name, memberConverter));
					} else if (member.IsDefined(typeof(SqlDeserializeAttribute), true)) {
						AssertValidMember(member);
						NestedMemberConverter nestedMemberConverter;
						if (typeof(IList).IsAssignableFrom(memberType)) {
							nestedMemberConverter = new NestedListMemberConverter(memberType, memberInfos.Count);
						} else {
							nestedMemberConverter = new NestedMemberConverter(memberType, memberInfos.Count);
						}
						memberConverters.Add(nestedMemberConverter);
						memberInfos.Add(member);
						hasNestedSerializers = true;
#warning add support for table valued parameters and SqlDeserializeAttribute (flatten the structure to one "table")
					}
				}
			}
			members = memberInfos.ToArray();
			converters = Array.AsReadOnly(memberConverters.ToArray());
			methods = MembersMethods.Get(members);
		}

		public IDictionary<string, SqlColumnInfo> Columns {
			get {
				return columns;
			}
		}

		public ReadOnlyCollection<MemberConverter> Converters {
			get {
				return converters;
			}
		}

		public bool HasNestedSerializers {
			get {
				return hasNestedSerializers;
			}
		}

		public int MemberCount {
			get {
				return members.Length;
			}
		}

		public object GetMember(object instance, int index) {
			return methods.GetMember(instance, index);
		}

		public void PopulateInstanceMembers(object result, object[] buffer) {
			methods.PopulateMembers(result, buffer);
		}

		private void AssertValidMember(MemberInfo memberInfo) {
			FieldInfo fieldInfo = memberInfo as FieldInfo;
			if (fieldInfo != null) {
				if (fieldInfo.IsInitOnly) {
					throw new InvalidOperationException(String.Format("The field {0}.{1} cannot be used as SQL column because it is readonly", fieldInfo.DeclaringType.FullName, fieldInfo.Name));
				}
			} else {
				PropertyInfo propertyInfo = memberInfo as PropertyInfo;
				if (propertyInfo != null) {
					if (propertyInfo.GetIndexParameters().Length > 0) {
						throw new InvalidOperationException(String.Format("The property {0}.{1} cannot be used as SQL column because it is indexed", propertyInfo.DeclaringType.FullName, propertyInfo.Name));
					}
					if (propertyInfo.GetGetMethod(true) == null) {
						throw new InvalidOperationException(String.Format("The property {0}.{1} cannot be used as SQL column because it has no getter", propertyInfo.DeclaringType.FullName, propertyInfo.Name));
					}
					if (propertyInfo.GetSetMethod(true) == null) {
						throw new InvalidOperationException(String.Format("The property {0}.{1} cannot be used as SQL column because it has no setter", propertyInfo.DeclaringType.FullName, propertyInfo.Name));
					}
				} else {
					throw new ArgumentException("Only fields and properties are supported", "memberInfo");
				}
			}
		}
	}
}
