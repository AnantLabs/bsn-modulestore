# Introduction #
The `bsn ModuleStore` is a non-visual SQL Server and SQL Azure versioning toolset (new T-SQL features of SQL Server 2008 are not yet complete).

It contains a library to be included in applications and an application for managing databases and generating scripts. The library takes care of database set-up, versioning and upgrading, schema check, logical separation of multiple modules in one database, and an optional stored procedure proxy with simple ORM-like capabilities.

# How to... #

  * [Compile the library from the source code](CompileTheLibrary.md)
  * [Use the command line application to script databases and other tasks](CommandLineApplication.md)
  * [Use bsn ModuleStore in your own applications](IntegratingModuleStore.md)

# Concepts #

  * [The versioning and update concept](VersioningConcept.md)
  * [Glossary](Glossary.md)

# Related projects #

  * [bsn CommandLine - Command Line UI](http://code.google.com/p/bsn-commandline/)
  * [bsn GoldParser - .NET Gold Parser Engine used internally for SQL parsing](http://code.google.com/p/bsn-goldparser/)