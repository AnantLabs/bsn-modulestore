# Introduction #

The `bsn ModuleStore` comes with a command-line application, which allows to perform several operations on databases by using SMO and the `bsn.ModuleStore` assemblies. The command-line application has an interface similar to the Windows `netsh` application.

# Overview #

After starting the application, a prompt is displayed. Help is available for all commands, just type the command followed by a `?`.
```
modulestore> ?
The following commands are available:

Commands in this context:
 ?              - Displays a list of the available commands.
 add            - Adds a configuration entry to a list of entries.
 assembly       - Manage scripts in assemblies.
 bye            - Exits the program.
 database       - Database server operations.
 delete         - Deletes a configuration entry from a list of entries.
 difference     - Shows the difference between the database and the scripts
 dump           - Dump all relevant database objects
 exit           - Exits the program.
 help           - Displays a list of the available commands.
 install        - Generate or execute SQL statements to install a source
 quit           - Exits the program.
 set            - Updates configuration settings.
 show           - Displays configuration information.
```
The most used operations are found in the database and in the assembly context.

## Usage tips ##
  * As long as partial identifiers are unambiguous, the command line will accept them in place of the full identifiers. So if you want to enter the database context, you can just type `da` (instead of `database`), and if you want to dump an assembly you could do this with `du as` (instead of `dump assembly`).
  * If you want to directly execute a command in a context without switching the context, just prefix the command with the context name: `database connect` will execute the `connect` command in the `database` context, without actually switching the context.
  * Arguments for commands (sometimes also called "tags") can be supplied directly after the command name. If you name them (e.g. `server=.`), they will be associated to this tag; if you don't name them they will be associated in the order given.
  * The syntax shown for help is as follows (you can see an example with `database script help`):
    * `[]` optional
    * `<>` value of this type expected
  * When asking for tag values to be entered, a value in the `[]` denotes the default which is used if nothing is entered (you can try it yourself with `dump`).

# `database` context #

Type `database` to enter the database context. Here is a list of the most common commands:

## `connect` ##

Connects to a database on the specified server. You can get some brief information about the database by typing in `show server` at any time.
```
modulestore database> connect localhost Test
Connected to database Test on server localhost

modulestore database> show server
Server: localhost
Database: Test (exists: True)
Connected: True
Connection string: Data Source=localhost;Initial Catalog=Test;Integrated Security=SSPI;
Database Type: Other
```

## `show schema` / `set schema` ##

You can show and change the active schema by using `show schema` or `set schema` (initially uses the default schema of the database when connecting).

## `script` ##

Generate scripts from the currently active schema in the connected database. Those scripts are ready to be embedded into an assembly. The scripting is performed a little different compared to other SQL Server scripting tools; ModuleStore already parses the script, re-formats it (the formatting is _not_ configurable) and it changes schema associations and removes file group and paritioning information, if present.

Only the raw `CREATE` statements for all objects are being scripted; no GOs, no exist checks, no `DROP` statements or anything like this is being scripted. It also does not include any date or time information in the file, so that you can run the scripting repeatedly without getting changing file contents as long as the object remains unchanged.

The scripting does not take into account whitespace, the case of keywords, quotes and `[]` for identifiers or the original schema name, so that the scripted output will always stay in the same format, even if some of the above changed in the database (for instance if a code formatting tool changed the source layout).

**The processing and uniforming of the SQL code makes it much easier to track changes with a SCM, since only relevant modifications will actually change the files.** Even if you were to script from different databases and/or database schemas with different formatting applied, you'll get a completely consistent output in the script files.

Optionally, you can choose to create directories for each object type (default is false), and you can specify the encoding to be used (default is UTF-8). Also, if desired, you can choose which schema name should be used in the scripts (default is `dbo`), or to delete other .sql files then those created now (default is false; don't use this if you created data or update scripts in the same folder, since those would get deleted!). It's usually best to leave those options on their default values.

## `uninstall` ##

This is the counter-piece to `install` (see below). A script for dropping the current schema is generated, which includes all objects in that schema. It does respect dependencies, to that the `DROP` should succeed. Alternatively, the operations can also directly be executed.

# `assembly` context #

The assembly context allows you to load any assembly which uses the `ModuleStore` attributes for embedding files (e.g. `[SqlProcedure("proc.sql")]` and `[SqlSetupScript("table.sql")]`) and then dump or compare the contained scripts against files or the current database.

## `load` ##

Load an assembly. The assembly is loaded for reflection only in a dedicated AppDomain, so that it can easily be unloaded again, and there is no risk of unwanted code execution.

As an example, the `bsn.ModuleStore.dll` can be loaded, since it contains embedded SQL:
```
modulestore assembly> load
 filename: bsn.ModuleStore.dll

modulestore assembly> dump assembly
-- Inventory hash: 8C-95-10-04-A9-0B-D7-A8-84-A1-C1-C7-42-9E-3E-06-4C-F5-A6-1A

-- Object hash: 51-DB-6D-1E-09-12-39-23-48-5A-8A-96-6F-3D-67-08-A2-18-5D-91
CREATE CLUSTERED INDEX [IX_tblModule_Cluster] ON [tblModule] (
...... (several DB objects)
```
# Global commands #

## `dump` ##

Processes and dumps the contents of any source (e.g. the current database schema, an assembly, or files in the current directory). You can use this command to check whether `ModuleStore` can properly process the SQL code, and to retrieve the hashes for the database objects.

Each database object has a SHA1 hash which is computed from the cleaned, schema-neutral SQL code (comments are not included in the hash computation). These hashes are used in `ModuleStore` to identify changes in database objects. The inventory hash is a hash created the same way for the full inventory; an emtpy database will yield a hash of zeros only.

## `difference` ##

The difference compares all database objects of any two sources. Each object will have a difference "None", "SourceOnly", "TargetOnly" or just "Different".

## `install` ##

Generates a script or performs the operations necessary to install objects from a source into a new schema on a database. It uses the current schema name (`set schema`) for the objects, and does resolve object dependencies so that objects are created in the correct order.