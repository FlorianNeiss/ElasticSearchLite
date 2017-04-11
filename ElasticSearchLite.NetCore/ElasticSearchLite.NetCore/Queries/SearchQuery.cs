﻿using ElasticSearchLite.NetCore.Interfaces;
using ElasticSearchLite.NetCore.Models;
using System.Collections.Generic;
using System.Linq;

namespace ElasticSearchLite.NetCore.Queries
{
    public abstract class SearchQuery : AbstractQuery
    {
        internal List<ElasticField> Fields { get; } = new List<ElasticField>();

        protected SearchQuery(IElasticPoco poco) : base(poco) { }

        protected SearchQuery(string indexName, string typeName) : base(indexName, typeName) { }

        protected override void ClearAllConditions()
        {
            TermCondition = null;
            RangeCondition = null;
            MatchCondition = null;
        }
    }
    public class SearchQuery<T> : SearchQuery where T : IElasticPoco
    {
        protected SearchQuery(string indexName, string typeName) : base(indexName, typeName) { }

        public static SearchQuery<T> Create(string indexName, string typeName)
        {
            return new SearchQuery<T>(indexName, typeName);
        }
        /// <summary>
        /// Include certain fields to a query
        /// </summary>
        /// <param name="incluededFields"></param>
        /// <returns></returns>
        public SearchQuery<T> Include(params ElasticField[] incluededFields)
        {
            CheckParameters(incluededFields);
            var include = incluededFields.Where(inc => !Fields.Select(f => f.Name).Contains(inc.Name));
            Fields.AddRange(include);

            return this;
        }
        /// <summary>
        /// Exclude certain fields from a query
        /// </summary>
        /// <param name="excludeFields"></param>
        /// <returns></returns>
        public SearchQuery<T> Exclude(params ElasticField[] excludeFields)
        {
            CheckParameters(excludeFields);
            var exclude = excludeFields.Where(ex => !Fields.Select(f => f.Name).Contains(ex.Name));
            Fields.Clear();
            Fields.AddRange(exclude);

            return this;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="matchAll"></param>
        /// <returns></returns>
        public SearchQuery<T> MatchAll(bool matchAll)
        {
            IsMatchAll = matchAll;
            ClearAllConditions();

            return this;
        }
        /// <summary>
        /// Match Query
        /// </summary>
        /// <param name="field">Field name</param>
        /// <param name="value">Value matching the field</param>
        /// <returns></returns>
        public SearchQuery<T> Match(string field, object value)
        {
            var condition = new ElasticCodition
            {
                Field = new ElasticField { Name = field },
                Value = value
            };

            return Match(condition);
        }
        /// <summary>
        /// Match Query
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public SearchQuery<T> Match(ElasticCodition condition)
        {
            ClearAllConditions();
            MatchCondition = CheckParameter(condition);

            return this;
        }
        /// <summary>
        /// Term Query finds documents that contain the exact term specified in the inverted index.
        /// </summary>
        /// <param name="field">Field name</param>
        /// <param name="value">Value which should equal the field content</param>
        /// <returns></returns>
        public SearchQuery<T> Term(string field, object value)
        {
            var condition = new ElasticCodition
            {
                Field = new ElasticField { Name = field },
                Value = value
            };

            return Term(condition);
        }
        /// <summary>
        /// Term Query finds documents that contain the exact term specified in the inverted index.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public SearchQuery<T> Term(ElasticCodition condition)
        {
            ClearAllConditions();
            TermCondition = CheckParameter(condition);

            return this;
        }
        /// <summary>
        /// Returns document for a range
        /// </summary>
        /// <param name="field">Field name</param>
        /// <param name="op">Range operator</param>
        /// <param name="value"></param>
        /// <returns></returns>
        public SearchQuery<T> Range(string field, RangeOperations op, object value)
        {
            var condition = new ElasticRangeCondition
            {
                Field = new ElasticField { Name = field },
                Operation = op,
                Value = value
            };

            return Range(condition);
        }
        /// <summary>
        /// Returns document for a range
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public SearchQuery<T> Range(ElasticRangeCondition condition)
        {
            ClearAllConditions();
            RangeCondition = CheckParameter(condition);

            return this;
        }
    }
}
