using System.Data;

namespace FoxDb
{
    public abstract class Parameter : IDbDataParameter
    {
        public Parameter(IDbDataParameter parameter)
        {
            this.InnerParameter = parameter;
        }

        public IDbDataParameter InnerParameter { get; private set; }

        public virtual byte Precision
        {
            get
            {
                return this.InnerParameter.Precision;
            }
            set
            {
                this.InnerParameter.Precision = value;
            }
        }

        public virtual byte Scale
        {
            get
            {
                return this.InnerParameter.Scale;
            }
            set
            {
                this.InnerParameter.Scale = value;
            }
        }

        public virtual int Size
        {
            get
            {
                return this.InnerParameter.Size;
            }
            set
            {
                this.InnerParameter.Size = value;
            }
        }

        public virtual DbType DbType
        {
            get
            {
                return this.InnerParameter.DbType;
            }
            set
            {
                this.InnerParameter.DbType = value;
            }
        }

        public virtual ParameterDirection Direction
        {
            get
            {
                return this.InnerParameter.Direction;
            }
            set
            {
                this.InnerParameter.Direction = value;
            }
        }

        public virtual bool IsNullable
        {
            get
            {
                return this.InnerParameter.IsNullable;
            }
        }

        public virtual string ParameterName
        {
            get
            {
                return this.InnerParameter.ParameterName;
            }
            set
            {
                this.InnerParameter.ParameterName = value;
            }
        }

        public virtual string SourceColumn
        {
            get
            {
                return this.InnerParameter.SourceColumn;
            }
            set
            {
                this.InnerParameter.SourceColumn = value;
            }
        }

        public virtual DataRowVersion SourceVersion
        {
            get
            {
                return this.InnerParameter.SourceVersion;
            }
            set
            {
                this.InnerParameter.SourceVersion = value;
            }
        }

        public virtual object Value
        {
            get
            {
                return this.InnerParameter.Value;
            }
            set
            {
                this.InnerParameter.Value = value;
            }
        }
    }
}
