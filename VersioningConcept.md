# Introduction #

One of the main goals of `bsn ModuleStore` is to reduce the effort for verifying and upgrading database objects. Here's how it works.

# The inventory #

In `bsn ModuleStore`, an inventory is a set of object creation scripts plus additional data set-up statements (typically `INSERT` statements adding some base data into the new tables).  Inventories can be created from a set of files, from an existing database schema, or from resources embedded in an assembly.

## Object and inventory hashes ##

A hash is created for each object in the inventory, as well as for the full inventory. This hash is unlikely to collide (SHA1), especially for the same object. Therefore it is suitable to identify changes in objects, especially when comparing two inventories. The hash computation is performed on a cleaned version of the script source code. The cleaning performed is the following:
  * All whitespace is replaced with a deterministic whitespace in order to rule out any differences based on whitespace differences only
  * Comments are removed
  * All keywords are converted to uppercase
  * All quotable identifiers are put into square brackets (`[]`)
  * All schema name references are removed

Finally, the hash is XORed against a static hash, so that empty inventories produce a hash consisting of 0's only.

# Automatic versioning #

## Detecting the state of the module ##

When using a module, the inventory hash of its database schema is compared to the inventory hash of the scripts embedded in the module assembly. If these hashes match, all objects are known or assumed to be equal.

In order to speed up the initial check, the current inventory hash is cached for each module in the `ModuleStore` database schema. Apart from the hash there is also an "update version" which is being tracked. This version denotes the last run update script; if update scripts with higher versions are available, the module is also seen as being not up to date.

Since the cached hash may be outdated if objects were added, deleted or modified externally (that is, not via a `bsn ModuleStore` update process), it is possible to force the recomputation of the hash. This is the default when a debugger is attached, but otherwise the hash is assumed to be correct and a manually triggered forced check is necessary to ensure this.

## Performing updates ##

If a change has been detected (hash difference or lower update version), the database objects need to be updated to the current version. All updates are performed in a transaction, which guarantees consistency in the schema even in the case of failures.

_Note:_ as long as the update version is identical, this may also replace newer object versions of functions, stored procedures etc. with older versions. While this may first seem wrong, it actually is a good thing, since it ensures that the module version running does find the code it was compiled against.

### The update process ###

An inventory of the outdated database schema is then created and each object hash is compared against the hash for the same object in the assembly inventory. If an object is different, it is added to the list of objects to update; if it is missing in the database schema, it is added to the list of object to create; if it is missing in the assembly schema it is added to the list of objects to be dropped. Identical objects are just skipped.

The next step is to create all missing objects and update different non-table objects. Dependencies between objects are taken into account in this step, and to begin only objects are being created and updated which do not have references to tables which are marked as being different. The next step is to run one update script and then to create and update pending objects again. This is repeated until all update scripts were performed and all objects were created or updated. The next step is to drop all objects which are no longer needed.

Finally, the database schema inventory is refreshed, and again compared to the assembly schema. In order to confirm the update as being successful, the hashes must match, otherwise something went wrong and a rollback of the complete update is performed. Otherwise the changes are committed and the cached hash and the update version are stored as well.

### Update scripts ###

Errors in the update process are typically the result of incomplete or incorrect update scripts. Since all changes to tables are considered being "destructive" changes (terminology from Martin Fowler's "Evolutionary Database Design" article). Therefore, everything which changes a table in any way - including changing indexes or adding columns - needs to be performed in the update script. The result **must** be identical to the object scripted in the `CREATE` script.

_Note:_ When writing update scripts, you need to be aware that each statement is executed separately. Therefore, if you need to use local variables, you need to scope them in a `BEGIN` .. `END` block. Also, since no objects have been deleted at the time when the update script runs, data migrations can be performed even if they need to access old tables or functions.