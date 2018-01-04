using FoxDb.Interfaces;
using System;

namespace FoxDb
{
    public class ForeignKeysParameterHandlerStrategy<T, TRelation> : IParameterHandlerStrategy
    {
        public ForeignKeysParameterHandlerStrategy(IDatabase database, T parent, TRelation child, IRelationConfig relation)
        {
            this.Database = database;
            this.Parent = parent;
            this.Child = child;
            this.Relation = relation;
        }

        public IDatabase Database { get; private set; }

        public T Parent { get; private set; }

        public TRelation Child { get; private set; }

        public IRelationConfig Relation { get; private set; }

        public DatabaseParameterHandler Handler
        {
            get
            {
                return (DatabaseParameterHandler)Delegate.Combine(this.LeftHandler, this.RightHandler);
            }
        }

        protected virtual DatabaseParameterHandler LeftHandler
        {
            get
            {
                return parameters =>
                {
                    if (this.Parent != null && !string.IsNullOrEmpty(this.LeftParameter) && parameters.Contains(this.LeftParameter))
                    {
                        parameters[this.LeftParameter] = this.Relation.LeftTable.PrimaryKey.Getter(this.Parent);
                    }
                };
            }
        }

        protected virtual DatabaseParameterHandler RightHandler
        {
            get
            {
                return parameters =>
                {
                    if (this.Child != null && !string.IsNullOrEmpty(this.RightParameter) && parameters.Contains(this.RightParameter))
                    {
                        parameters[this.RightParameter] = this.Relation.RightTable.PrimaryKey.Getter(this.Child);
                    }
                };
            }
        }

        protected virtual string LeftParameter
        {
            get
            {
                switch (this.Relation.Multiplicity)
                {
                    case RelationMultiplicity.OneToOne:
                    case RelationMultiplicity.OneToMany:
                        return Conventions.ParameterName(this.Relation.RightTable.ForeignKey);
                    case RelationMultiplicity.ManyToMany:
                        return Conventions.ParameterName(this.Relation.MappingTable.LeftForeignKey);
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        protected virtual string RightParameter
        {
            get
            {
                switch (this.Relation.Multiplicity)
                {
                    case RelationMultiplicity.OneToOne:
                    case RelationMultiplicity.OneToMany:
                        return string.Empty;
                    case RelationMultiplicity.ManyToMany:
                        return Conventions.ParameterName(this.Relation.MappingTable.RightForeignKey);
                    default:
                        throw new NotImplementedException();
                }
            }
        }
    }
}
