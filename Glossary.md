# Module #

A module is a unit managed by `bsn ModuleStore`. There is a strong relations between assemblies and modules: each assembly is seen as one "module" if it contains explicitly declared embedded resources containing SQL scripts (using attributes such as `[SqlProcedure("proc.sql")]`, `[SqlSetupScript("table.sql")]` and `[SqlUpdateScript(', "update1.sql")]`).

It is important to know that all objects defined in a module must belong to one SQL schema. During development, on a dedicated development database used for one module only, this is usually the `dbo` schema. However, since `bsn ModuleStore` uses SQL schemas to logically isolate the different module instances (see below), only be one SQL schema can be used per module.

# Inventory #

An inventory is a set of SQL scripts, consisting of create scripts for all objects, statements for adding initial data during set-up, and update scripts for executing table updates. An inventory is created from all the script files defined in a module, both the inventory as well as each object in the inventory is unambiguously identifiable with a hash value. For more details on the inventory, read the [versioning concept](VersioningConcept.md) page.

# Module Instance #

Each database can contain an arbitrary number of module instances. Each instance is a dedicated SQL schema in the database, which is tracked by `bsn ModuleStore` in a its own module instance (that is, the `bsn ModuleStore` itself is a normal module instance which is just bootstrapped differently; it is using itself to do the versioning of itself).

It is therefore also possible to create multiple instances of one module in the same database. For instance, let's assume that we have a module used for storing localization data. Using `bsn ModuleStore`, it would be possible to have different modules which each uses its own localization module instance, but which are properly isolated in their own SQL schemas so that no conflicts occur.

# Data interface #

While it isn't a requirement to use stored procedures, `bsn ModuleStore` has been designed to use stored procedures extensively in combination with a Lightweight ORM approach. A data interface is a normal .NET interface which inherits from `IStoredProcedures`. Each method on this interface is then decorated with a `[SqlProcedure("proc.sql")]` attribute, which binds the stored procedure defined as `CREATE PROCEDURE` statement to this method in the interface. After obtaining an instance of the interface from `bsn ModuleStore`, calls to the database are as easy as invoking a .NET method an interface.

# Lightweight ORM #

The stored procedures defined in the data interfaces can return complex data types, which are marshalled to and from the database. It natively supports primitives, common structs such as `Guid` and `DateTime`, XML types, return values, by-reference parameters, as well as the deserialization to custom objects (optionally including nested objects), just to name a few of its capabilities. However, the generated entities are not tied to specific rows in database tables, which is one of the main differences between common ORMs and the Lightweight ORM .