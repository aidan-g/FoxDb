﻿using FoxDb.Interfaces;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace FoxDb
{
    public partial class EnumerableVisitor
    {
        protected virtual void VisitSkip(MethodCallExpression node)
        {
            var table = default(ITableConfig);
            this.Visit(node.Arguments[0]);
            if (this.TryGetTable(node.Arguments.First(), out table))
            {
                var parameter = default(IParameterBuilder);
                var context = default(CaptureFragmentContext);
                if (this.TryCapture<IParameterBuilder>(null, node.Arguments.Last(), out parameter, out context))
                {
                    var offset = Convert.ToInt32(context.Constants[parameter.Name]);
                    var filter = this.Query.Source.GetTable(table).Filter;
                    filter.Offset = offset;
                    return;
                }
            }
            this.VisitUnsupportedMethodCall(node);
        }
    }
}
