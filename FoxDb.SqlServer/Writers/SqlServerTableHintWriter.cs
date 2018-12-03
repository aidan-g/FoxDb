using FoxDb.Interfaces;
using System;
using System.Collections.Generic;

namespace FoxDb
{
    public class SqlServerTableHintWriter : SqlQueryWriter
    {
        public SqlServerTableHintWriter(IFragmentBuilder parent, IQueryGraphBuilder graph, IDatabase database, IQueryGraphVisitor visitor, ICollection<IDatabaseQueryParameter> parameters)
            : base(parent, graph, database, visitor, parameters)
        {
            this.Dialect = this.Database.QueryFactory.Dialect as SqlServerQueryDialect;
        }

        public SqlServerQueryDialect Dialect { get; private set; }

        public override FragmentType FragmentType
        {
            get
            {
                return SqlServerQueryFragment.TableHint;
            }
        }

        protected override T OnWrite<T>(T fragment)
        {
            if (fragment is ITableHintBuilder)
            {
                var expression = fragment as ITableHintBuilder;
                if (expression.LockHints != LockHints.None)
                {
                    this.Builder.AppendFormat("{0} {1} ", this.Dialect.WITH, this.Dialect.OPEN_PARENTHESES);
                    this.VisitLockHints(expression.LockHints);
                    this.Builder.AppendFormat("{0}", this.Dialect.CLOSE_PARENTHESES);
                }
                return fragment;
            }
            throw new NotImplementedException();
        }

        protected virtual void VisitLockHints(LockHints lockHints)
        {
            if (lockHints.HasFlag(LockHints.RowLock))
            {
                this.Builder.AppendFormat("{0}", this.Dialect.ROWLOCK);
            }
            if (lockHints.HasFlag(LockHints.PageLock))
            {
                this.Builder.AppendFormat("{0}", this.Dialect.PAGLOCK);
            }
            if (lockHints.HasFlag(LockHints.TableLock))
            {
                this.Builder.AppendFormat("{0}", this.Dialect.TABLOCK);
            }
            if (lockHints.HasFlag(LockHints.DatabaseLock))
            {
                this.Builder.AppendFormat("{0}", this.Dialect.DBLOCK);
            }
            if (lockHints.HasFlag(LockHints.UpdateLock))
            {
                this.Builder.AppendFormat("{0}", this.Dialect.UPDLOCK);
            }
            if (lockHints.HasFlag(LockHints.ExclusiveLock))
            {
                this.Builder.AppendFormat("{0}", this.Dialect.XLOCK);
            }
            if (lockHints.HasFlag(LockHints.HoldLock))
            {
                this.Builder.AppendFormat("{0}", this.Dialect.HOLDLOCK);
            }
            if (lockHints.HasFlag(LockHints.NoLock))
            {
                this.Builder.AppendFormat("{0}", this.Dialect.NOLOCK);
            }
        }

        public override IFragmentBuilder Clone()
        {
            throw new NotImplementedException();
        }
    }
}
