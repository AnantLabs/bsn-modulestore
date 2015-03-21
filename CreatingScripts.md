# Introduction #

The `bsn ModuleStore` can create its scripts itself. In fact, it is recommended that the scripts are created by using the [ModuleStore command line application](CommandLineApplication.md), so that the scripts are identical to the scripting performed internally during the diff with the database schema.

Since version 1.1 there is no longer a dependency to the Microsoft SMO! This means that you can actually script almost any SQL Server 2005 database using `bsn ModuleStore` (as long as only supported functionality is being used) without first installing the SMO objects on the machine. This is especially nice for environment where SMO cannot be deployed, and it circumvents the inherent problems of using SMO (V9 is buggy; V10 scripts tables differently, the SMO cannot be XCOPY-deployed because they require native code assemblies, etc.).

# Details #

Scripting a schema is easy: switch to the database context, connect to the database, choose the schema (it it is different than your default schema), and generate the scripts. Scripting is performed for one schema at a time, assuming that each schema represents an independent database "code block".

Here I scripted the `ModuleStore` schema from a database:
```
modulestore> database

modulestore database> connect
 server: .
 database: MyDatabase
Connected to database MyDatabase on server .

modulestore database> set schema ModuleStore

modulestore database> script
 path [C:\Hg\ModuleStore\ModuleStore\bin\Release]:
 directories [False]:
 delete [False]:
Scripting to C:\Hg\ModuleStore\ModuleStore\bin\Release (Encoding: utf-8)...
* Scripting spModuleAdd.sql
* Scripting spModuleDelete.sql
* Scripting spModuleList.sql
* Scripting spModuleUpdate.sql
* Scripting tblModule.sql
* Scripting vwModule.sql

modulestore database> exit
```

**The generated scripts have the following properties:**
  * They do not contain any "nondeterministic" data, such as scripting date and time. Therefore your source control management system will not detect changes in the files for unchanged database objects. This enables simple integration with any SCM.
  * Each database object is in its own file, except for indexes which are appended to their respective table and view files.
  * The scripts are uniformely formatted and structured, even if the original source (for source-based objects such as views, functions, stored procedures and triggers) was formatted otherwise. This also enhances stability when using a SCM system.
  * All names of (schema-local) objects in the scripts are schema-qualified to the `dbo` schema name, regardless of the original schema (this again enhances stability in SCM). Using qualified names is recommended practice and can actually boost the performance quite a lot, so that this is an added benefit ([this blog entry](http://blogs.msdn.com/b/mssqlisv/archive/2007/03/23/upgrading-to-sql-server-2005-and-default-schema-setting.aspx) explains it in detail).

# Data scripts #

The `bsn ModuleStore` cannot create data scripts currently. I usually use the functionality included in the great (and free!) [Tools Pack http://www.ssmstoolspack.com/](SSMS.md) to generate data scripts for the required setup data, and then put them into a separate `Data.sql` file.

# Planned work #

An add-on for VS and/or SSMS would simplify the creation of the scripts by requiring only a few clicks. However, this currently isn't a priority.