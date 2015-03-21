# Introduction #

An overview on how to use the `bsn ModuleStore` core functionality: managing database modules in a SQL Server 2005 database.

# Preparations #

Create a new application in VS (2008 SP1 or newer required). Since the `bsn ModuleStore` library is completely interface-agnostic this can be a ASP.NET, WinForms, console or any other type of application or library.

**Note:** The library was developed with C#. It has not been explicitly tested for CLI compliance, so that it may or may not work well with other .NET languages.

# Creating the scripts #

Since each module is expected to be self-contained, you can use an empty, new database instance to develop your database code using any tool you're comfortable with. This is likely SSMS or VS, which doesn't matter. When you're ready to transfer the script files into your application, follow the steps outlined on the [creating scripts](CreatingScripts.md) page. However, make sure to choose a destination directory inside of your project folder.

# Include the files #

In VS, make sure to show hidden files (refresh if necessary) and then include the new script files. Set them to be "Embedded Resources" as build action. Then refer to these files by using the `SqlSetupScriptAttribute` and `SqlProcedureAttribute`:

![http://wiki.bsn-modulestore.googlecode.com/hg/EmbeddingSQL.png](http://wiki.bsn-modulestore.googlecode.com/hg/EmbeddingSQL.png)

# Write the DB glue code #

While any form of DB access is supported, the recommended way is to use the native lightweight ORM (LORM) feature. This feature expects that all DB access is done via stored procedures which have been scripted along with the rest of the DB code.

Defining methods and entities is very easy. Let's assume that we want to get a recordset returned by a SP which requires a `pageIndex` and a `pageSize` parameter:

```
interface IMyDb: IStoredProcedures {
  [SqlProcedure("MyList.sql")]
  ResultSet<MyEntity> MyList(int pageIndex, int pageSize);
}

class MyEntity {
  [SqlColumn("MyId", Identity = true)]
  private int id;

  [SqlColumn("MyString")]
  private int str;

  public int Id {
    get { return id; }
  }

  public int Str {
    get { return str; }
  }
}
```

You can add any number of SPs to interfaces, and you can create any number of different interfaces. The entities can also be nested, and it is possible to hook into the entity creation process in order to implement "singleton" instances or other things. By default, however, entities are just created (and populated) with the .NET serialization facilities (no constructor is called by default, but this can be controlled via the `SqlProcedureAttribute`).

More details on the LORM will be posted in a separate article in this wiki.

# Preparing the database #

Create an empty database, and make sure that the database user which will access the DB via the application has sufficient rights to modify objects in the database.

# Using the database and LORM #

Initialize a (single, global) instance of `ModuleDatabase` (or any descendant of it for special transaction handling etc.):
```
database = new ModuleDatabase(connectionString, true);
```
The second argument controls the automatic update process. If `true`, the `bsn ModuleStore` library will not only check but also upgrade database schemas automatically upon their first use. In production use you may want to control when DDL updates are performed, so that you can disable the automatic update process here.

The database will now contain a new schema `ModuleStore` containing some database objects used for the management of the instances.

Now that the database instance is available, we are ready to use the database. The typical use case is that each database module is instantiated once in a database, but `bsn ModuleStore` allows an arbitrary instance number of the same module. For this application, however, let's assume the single instance case. We get access to our LORM interface like this:
```
IMyDb myDb = database.Get<IMyDb>(true);
```
Here again, the boolean controls the automatic creation of the instance if it doesn't exist. After this call, a new schema will have been created in the database with your database objects in it, and set-up data will have been inserted as well.

Invoking the SP is now just like invoking any other .NET method:
```
var entities = myDb.MyList(0, 20);
```

# Singleton instances and update notifications #
Especially when doing data binding, you may want to avoid creating multiple instances of the same entities. This can easily be controlled by assigning an instance provider to the interface (the generic argument controls the identity type to use, such as `int`, `long` or `Guid`):
```
myDb.Provider = new SingleInstanceProvider<int>();
```

From now on, all instances which haven't been garbage collected yet will be reused, even if they are returned in the same resultset. If you want your entities to dispatch change notifications (such as when implementing `INotifyPropertyChanged`), you can implement `ISqlDeserializationHook` on the entity which will be called whenever the fields have been populated.

# Updating the database #
Now that everything is running, we may want to add more stuff to the database. Adding new objects as well as modifying any non-table objects is managed automatically by `bsn ModuleStore`; it creates the required change script on the fly based on the database objects defined in the setup scripts. If present, `INSERT` statements inserting data directly into new tables will be executed as well.

Any change related to an existing table, however, needs to be coded manually in an update script (`ALTER TABLE ...` along with data transformations if required). Each of those update scripts is also embedded and marked with the `SqlUpdateScriptAttribute`, specifying both the embedded script name as well as the update step.

You only need to start creating change script after the first version of the application has been deployed, and if you want ModuleStore to automatically update the DB schema for the application users. Until then you can just delete objects and they will be re-created automatically during the automatic update.

# Done! #
That's the core working principle of `bsn ModuleStore`! There are many possibilities to plug into the out-of-the-box functionality in order to change behavior. Happy coding!