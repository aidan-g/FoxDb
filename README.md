# FoxDb

## A really simple ORM.

* Configure by convention, annotation or fluent API (or a combination of each).
* Can be config-less.
* Supports three flavors of relationship; `1:1` `1:*` and `*:*` (with mapping table).
* Has LINQ provider and high level query "dom".
* Currently only SQLite with auto incrementing keys but providers are easy to create.
* Not at all optimized, although it isn't slow.
* Low memory usage, stateless.

```C#
//Add 3 records to Test001.
var provider = new SQLiteProvider(Path.Combine(CurrentDirectory, "test.db"));
var database = new Database(provider);
using (var transaction = database.Connection.BeginTransaction())
{
    var set = database.Set<Test001>(transaction);
    set.Clear();
    set.AddOrUpdate(new[]
    {
        new Test001() { Field1 = "1_1", Field2 = "1_2", Field3 = "1_3" },
        new Test001() { Field1 = "2_1", Field2 = "2_2", Field3 = "2_3" },
        new Test001() { Field1 = "3_1", Field2 = "3_2", Field3 = "3_3" }
    });
    transaction.Commit();
}
```

* Columns and relations are auto discovered based on conventions (which can be configured).

```C#
public static class Conventions
{
    public static Func<Type, string> TableName = type => Pluralization.Pluralize(type.Name);

    public static Func<ITableConfig, ITableConfig, string> RelationTableName = (table1, table2) => string.Format(
        "{0}_{1}", 
        Pluralization.Singularize(table1.TableName), 
        Pluralization.Singularize(table2.TableName)
    );

    public static string KeyColumn = "Id";

    public static Func<ITableConfig, string> RelationColumn = table => string.Format(
        "{0}_{1}", 
        Pluralization.Singularize(table.TableName), 
        KeyColumn
    );
```

* Eager loading and relational persistence is enabled by default.
* Some LINQ functions are supported, the provider falls back to in-memory query when unsupported.

```C#
var set = database.Set<Test002>();
set.AddOrUpdate(new[]
{
    new Test002() { Name = "1_1", Test004 = new List<Test004>() { new Test004() { Name = "1_2" }, new Test004() { Name = "1_3" } } },
    new Test002() { Name = "2_1", Test004 = new List<Test004>() { new Test004() { Name = "2_2" }, new Test004() { Name = "2_3" } } },
    new Test002() { Name = "3_1", Test004 = new List<Test004>() { new Test004() { Name = "3_2" }, new Test004() { Name = "3_3" } } },
});
var query = database.AsQueryable<Test002>(transaction);
query.Where(element => element.Id == data[2].Id); //Record 2.
query.Where(element => element.Id == data[2].Id && element.Test004.Any(child => child.Id == data[2].Test004.First().Id)); //Also record 2.
```

* See the test project for more examples.
