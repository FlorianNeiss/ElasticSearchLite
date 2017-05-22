﻿using ElasticSearchLite.NetCore.Interfaces;
using ElasticSearchLite.NetCore.Models;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ElasticSearchLite.NetCore.Queries
{
    public abstract class AbstractQuery : IQuery
    {
        internal string IndexName { get; }
        internal string TypeName { get; }
        internal List<ElasticTermCodition> TermConditions { get; } = new List<ElasticTermCodition>();
        internal ElasticMatchCodition MatchCondition { get; set; }
        internal List<ElasticRangeCondition> RangeConditions { get; } = new List<ElasticRangeCondition>();
        internal IElasticPoco Poco { get; set; }

        protected AbstractQuery(IElasticPoco poco)
        {
            Poco = poco ?? throw new ArgumentNullException(nameof(poco));
            IndexName = poco.Index ?? throw new ArgumentNullException(nameof(poco.Index));
            TypeName = poco.Type ?? throw new ArgumentNullException(nameof(poco.Type));
        }

        protected AbstractQuery(string indexName)
        {
            if (string.IsNullOrEmpty(indexName)) { throw new ArgumentNullException(nameof(indexName)); }

            IndexName = indexName;
        }

        protected AbstractQuery(string indexName, string typeName)
        {
            if (string.IsNullOrEmpty(indexName)) { throw new ArgumentNullException(nameof(indexName)); }
            if (string.IsNullOrEmpty(typeName)) { throw new ArgumentNullException(nameof(typeName)); }

            IndexName = indexName;
            TypeName = typeName;
        }

        protected PT CheckParameter<PT>(PT parameter)
        {
            if (parameter == null) { throw new ArgumentNullException(nameof(parameter)); }
            return parameter;
        }

        protected void CheckParameters<PT>(PT[] parameters)
        {
            if (parameters == null) { throw new ArgumentNullException(nameof(parameters)); }
            if (!parameters.Any()) { throw new ArgumentException(nameof(parameters)); }
        }

        protected string GetCorrectPropertyName<T>(Expression<Func<T, Object>> expression)
        {
            if (expression.Body is MemberExpression)
            {
                return ((MemberExpression)expression.Body).Member.Name;
            }
            else
            {
                var op = ((UnaryExpression)expression.Body).Operand;
                return ((MemberExpression)op).Member.Name;
            }
        }
    }
}
