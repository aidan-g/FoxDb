﻿using FoxDb.Interfaces;
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
