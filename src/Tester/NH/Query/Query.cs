using System;
using System.Collections.Generic;

using NHibernate.Criterion;

namespace PageOfBob.Comparison.NH.Query {
	public class Query<T, K> where T : BaseObject where K : Criteria {
		public K Criteria { get; private set; }
		
		public Query(K criteria) {
			this.Criteria = criteria;
		}

		protected T _alias;
		public virtual QueryOver<T, T> GetQuery() {
			var q = QueryOver.Of<T>(() => _alias);

			if (Criteria.ID.HasValue)
				q.And(() => _alias.ID == Criteria.ID.Value);

			if (Criteria.Deleted.HasValue)
				q.And(() => _alias.Deleted == Criteria.Deleted.Value);

			if (Criteria.Take.HasValue)
				q.Take(Criteria.Take.Value);

			return q;
		}

		public IList<T> ToList(NHibernate.ISession session) {
			return GetQuery()
				.GetExecutableQueryOver(session)
				.List<T>();
		}

		public T FirstOrDefault(NHibernate.ISession session) {
			Criteria.Take = 1;

			var list = GetQuery()
				.GetExecutableQueryOver(session)
				.List<T>();

			if (list.Count == 0)
				return default(T);

			return list[0];
		}

		public int Count(NHibernate.ISession session) {
			return GetQuery()
				.Select(Projections.RowCount())
				.GetExecutableQueryOver(session)
				.SingleOrDefault<int>();
		}

		public bool Any(NHibernate.ISession session) {
			return Count(session) > 0;
		}
	}
}
