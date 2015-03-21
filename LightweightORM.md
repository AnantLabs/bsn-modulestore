# Lighweight Object Relational Mapper #
The `bsn ModuleStore` LORM does serialize parameters and deserialize results for stored procedure calls. The goal of this functionality is to remove the need for creating the boilerplate DB binding code completely, so that there is no need to create and mnaintain `SqlParameter` objects, assign the correct type and lengths, handle the `DBNull`, or to read the data from a `SqlReader` with column information and conversion the the required target types.

## Parameter serialization ##
  * Supports the built-in SQL types as well as structured types (starting with engine V10, that is, SQL Server 2008 or SQL Azure)
  * Simple type parameters can be input, output or both
  * Return values are supported
  * When using the `XmlDocument` class for XML support, a `XmlNametable` to be used for new XML documents can be supplied
  * Constant parameters can be defined (for instance if this influences the returned result metadata of the SP)

## Scalar invocation ##
  * Whenever the SP method declaration returns a non-collection type which is not a `Int32`, a scalar call is assumed (the first column of the first row is returned)
  * To return an `Int32` as scalar, the behavior can be controlled via the `SqlProcedureAttribute` attribute

## Return value ##
  * If the SP method declaration returns an `Int32`, it is assumed to be the return value of the SP invocation
  * For other types, the behavior can be controlled via the `SqlProcedureAttribute` attribute (so that the return value could be returned as `bool` for instance)

## Single result set ##
  * Whenever a common generic collection type (array types, `IEnumerable<>`, `ICollection<>`, `IList<>` or `List<>`) is specified in the SP method declaration, a collection is instantiated and each row is deserialized as one entry in the collection
  * Alternatively, a `ResultSet` descendant can be supplied (typically the generic `ResultSet<>` provides a good implementation)

## Multiple result sets ##
  * Multiple result sets are supported via `ResultSet` class. The generic `ResultSet<,>` provides a default implementation with support for type-save declaration of any static number of result sets

## Typed and untyped data readers ##
  * For streaming access and support for arbitrary numbers of result sets, `IDataReader` can be used
  * For typed access to columns, interfaces inheriting from `ITypedDataReader` can specify (virtual) properties which are mapped to the data reader columns
  * Note: For of technical reasons it is not possible to mix typed data readers with output parameters