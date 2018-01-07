using FoxDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FoxDb
{
    public class SQLiteQueryBuilder : IQueryBuilder
    {
        public SQLiteQueryBuilder(IDatabase database, IQueryGraph graph)
        {
            this.Database = database;
            this.Graph = graph;
        }

        public IDatabase Database { get; private set; }

        public IQueryGraph Graph { get; private set; }

        public IDatabaseQuery Query
        {
            get
            {
                var visitor = new SQLiteQueryBuilderVisitor(this.Database);
                visitor.Visit(this.Graph);
                return visitor.Query;
            }
        }

        protected class SQLiteQueryBuilderVisitor : QueryGraphVisitor
        {
            protected readonly IDictionary<Type, byte> FragmentPriorities = new Dictionary<Type, byte>()
            {
                { typeof(IInsertBuilder), 10 },
                { typeof(IUpdateBuilder), 20 },
                { typeof(IDeleteBuilder), 30 },
                { typeof(ISelectBuilder), 40 },
                { typeof(IFromBuilder), 50 },
                { typeof(IWhereBuilder), 60 },
                { typeof(IOrderByBuilder), 70 }
            };

            private SQLiteQueryBuilderVisitor()
            {
                this.Members = new DynamicMethod(this.GetType());
                this.Builder = new StringBuilder();
                this.ParameterNames = new List<string>();
                this.Targets = new Stack<IFragmentTarget>();
            }

            public SQLiteQueryBuilderVisitor(IDatabase database) : this()
            {
                this.Database = database;
            }

            protected DynamicMethod Members { get; private set; }

            public StringBuilder Builder { get; private set; }

            public ICollection<string> ParameterNames { get; private set; }

            public IDatabase Database { get; private set; }

            public IDatabaseQuery Query
            {
                get
                {
                    return new DatabaseQuery(this.Builder.ToString(), this.ParameterNames.ToArray());
                }
            }

            protected Stack<IFragmentTarget> Targets { get; private set; }

            public IFragmentTarget Peek
            {
                get
                {
                    var target = this.Targets.Peek();
                    if (target == null)
                    {
                        throw new InvalidOperationException("No target to write fragment to.");
                    }
                    return target;
                }
            }

            public IFragmentTarget Push(IFragmentTarget target)
            {
                this.Targets.Push(target);
                return target;
            }

            public IFragmentTarget Pop()
            {
                return this.Targets.Pop();
            }

            protected override IEnumerable<IFragmentBuilder> GetFragments(IQueryGraph graph)
            {
                return base.GetFragments(graph).OrderBy(fragment =>
                {
                    var type = fragment.GetType();
                    foreach (var @interface in type.GetInterfaces())
                    {
                        var priority = default(byte);
                        if (FragmentPriorities.TryGetValue(@interface, out priority))
                        {
                            return priority;
                        }
                    }
                    throw new NotImplementedException();
                });
            }

            protected override void VisitInsert(IInsertBuilder expression)
            {
                this.Push(new SQLiteInsertWriter(this.Database, this, this.Builder, this.ParameterNames));
                this.Peek.Write(expression);
                this.Pop();
            }

            protected override void VisitUpdate(IUpdateBuilder expression)
            {
                this.Push(new SQLiteUpdateWriter(this.Database, this, this.Builder, this.ParameterNames));
                this.Peek.Write(expression);
                this.Pop();
            }

            protected override void VisitDelete(IDeleteBuilder expression)
            {
                this.Push(new SQLiteDeleteWriter(this.Database, this, this.Builder, this.ParameterNames));
                this.Peek.Write(expression);
                this.Pop();
            }

            protected override void VisitSelect(ISelectBuilder expression)
            {
                this.Push(new SQLiteSelectWriter(this.Database, this, this.Builder, this.ParameterNames));
                this.Peek.Write(expression);
                this.Pop();
            }

            protected override void VisitFrom(IFromBuilder expression)
            {
                this.Push(new SQLiteFromWriter(this.Database, this, this.Builder, this.ParameterNames));
                this.Peek.Write(expression);
                this.Pop();
            }

            protected override void VisitWhere(IWhereBuilder expression)
            {
                this.Push(new SQLiteWhereWriter(this.Database, this, this.Builder, this.ParameterNames));
                this.Peek.Write(expression);
                this.Pop();
            }

            protected override void VisitOrderBy(IOrderByBuilder expression)
            {
                this.Push(new SQLiteOrderByWriter(this.Database, this, this.Builder, this.ParameterNames));
                this.Peek.Write(expression);
                this.Pop();
            }
        }
    }
}
