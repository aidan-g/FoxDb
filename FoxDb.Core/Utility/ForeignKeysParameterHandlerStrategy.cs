using FoxDb.Interfaces;
using System;
using System.Linq;

namespace FoxDb
{
    public class ForeignKeysParameterHandlerStrategy : IParameterHandlerStrategy
    {
        public ForeignKeysParameterHandlerStrategy(object parent, object child, IRelationConfig relation)
        {
            this.Parent = parent;
            this.Child = child;
            this.Relation = relation;
        }

        public object Parent { get; private set; }

        public object Child { get; private set; }

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
                return (parameters, phase) =>
                {
                    switch (phase)
                    {
                        case DatabaseParameterPhase.Fetch:
                            var parameter = this.LeftParameter;
                            if (this.Parent != null && !string.IsNullOrEmpty(parameter) && parameters.Contains(parameter))
                            {
                                parameters[parameter] = this.Relation.LeftTable.PrimaryKey.Getter(this.Parent);
                            }
                            break;
                    }
                };
            }
        }

        protected virtual DatabaseParameterHandler RightHandler
        {
            get
            {
                return (parameters, phase) =>
                {
                    switch (phase)
                    {
                        case DatabaseParameterPhase.Fetch:
                            var parameter = this.RightParameter;
                            if (this.Child != null && !string.IsNullOrEmpty(parameter) && parameters.Contains(parameter))
                            {
                                parameters[parameter] = this.Relation.RightTable.PrimaryKey.Getter(this.Child);
                            }
                            break;
                    }
                };
            }
        }

        protected virtual string LeftParameter
        {
            get
            {
                switch (this.Relation.Flags.GetMultiplicity())
                {
                    case RelationFlags.OneToOne:
                    case RelationFlags.OneToMany:
                        var columns = this.Relation.Expression.GetColumnMap();
                        return Conventions.ParameterName(columns[this.Relation.RightTable].First(column => column.IsForeignKey));
                    case RelationFlags.ManyToMany:
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
                switch (this.Relation.Flags.GetMultiplicity())
                {
                    case RelationFlags.OneToOne:
                    case RelationFlags.OneToMany:
                        return string.Empty;
                    case RelationFlags.ManyToMany:
                        return Conventions.ParameterName(this.Relation.MappingTable.RightForeignKey);
                    default:
                        throw new NotImplementedException();
                }
            }
        }
    }
}
