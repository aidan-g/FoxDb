# FoxDb

## A really simple ORM.

* Configure by convention, annotation or fluent API (or a combination of each).
* Can be config-less.
* Supports three flavors of relationship; `1:1` `1:*` and `*:*` (with mapping table).
* Can configure complex relations manually including multiple select paths.
* Has LINQ provider and high level query "dom".
* Supports SQLite, SqlCe and SqlServer >=2008 or >=2012. The 2012 version uses improved paging functions.
* Low memory usage, stateless.

```C#
//Add 3 records to Test001.
var provider = new SQLiteProvider(Path.Combine(CurrentDirectory, "test.db"));
var database = new Database(provider);
using (var transaction = database.Connection.BeginTransaction())
{
    var set = database.Set<Test001>(transaction);
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

* Tables, relations and indexes can be created and deleted from an object model however updates are not supported.

```C#
//A contrived table creation example.
//The fields Field1, Field2 and Field3 are indexed with an IS NOT NULL criteria.
public class Test001 : TestData, IEntityConfiguration
{
    public long Id { get; set; }

    [Index(Name = "Fields", Flags = IndexFlags.Unique)]
    public virtual string Field1 { get; set; }

    [Index(Name = "Fields", Flags = IndexFlags.Unique)]
    public virtual string Field2 { get; set; }

    [Index(Name = "Fields", Flags = IndexFlags.Unique)]
    public virtual string Field3 { get; set; }

    [Column(Flags = ColumnFlags.ConcurrencyCheck)]
    public int Version { get; set; }

    public void Configure(IConfig config, ITableConfig table)
    {
        var index = table.GetIndex(IndexConfig.By(new[] { "Field1", "Field2", "Field3" }));
        if (index != null)
        {
            index.Expression = index.Expression.Combine(
                QueryOperator.AndAlso,
                index.Columns.Select(column => index.CreateConstraint().With(expression =>
                {
                    expression.Left = expression.CreateColumn(column);
                    expression.Operator = expression.CreateOperator(QueryOperator.Is);
                    expression.Right = expression.CreateUnary(QueryOperator.Not, expression.CreateOperator(QueryOperator.Null));
                })).ToArray()
            );
        }
    }
}

//Columns and relations will be detected automatically.
var table = this.Database.Config.Transient.CreateTable(
    TableConfig.By(typeof(Test001), TableFlags.AutoColumns | TableFlags.AutoIndexes)
).With(table =>
{
	//Any fields that aren't in the object model can be added manually here or by implementing IEntityConfiguration.
    table.CreateColumn(ColumnConfig.By("Field4", ColumnFlags.None)).With(column =>
    {
        column.ColumnType = Factories.Type.Create(TypeConfig.By(DbType.Int32, isNullable: true));
    });
    table.CreateColumn(ColumnConfig.By("Field5", ColumnFlags.None)).With(column =>
    {
        column.ColumnType = Factories.Type.Create(TypeConfig.By(DbType.Double, isNullable: true));
    });
});

//Generate the CREATE TABLE statement with the default settings.
var query = this.Database.SchemaFactory.Add(table, Defaults.Schema.Flags).Build();
//Execute it.
this.Database.Execute(query);
```

* Creating a relation is as simple as exposing a property of ICollection<T>.
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

* Supports concurrency with numeric or binary versioning.
* Many asynchronous operations including enumerators and readers.

* See the test project for more examples.
