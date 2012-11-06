using System;
using System.Collections.Generic;

using NHibernate.Criterion;

namespace PageOfBob.Comparison.NH.Query {
	public class Query<T, K> where T : BaseObject where K : Criteria {
		public K Criteria { get; private set; }
		protected NHibernate.ISession Session { get; private set; }
		
		public Query(K criteria, NHibernate.ISession session) {
			this.Criteria = criteria;
			this.Session = session;
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

		public IList<T> ToList() {
			return GetQuery()
				.GetExecutableQueryOver(Session)
				.List<T>();
		}

		public T FirstOrDefault() {
			Criteria.Take = 1;

			var list = GetQuery()
				.GetExecutableQueryOver(Session)
				.List<T>();

			if (list.Count == 0)
				return default(T);

			return list[0];
		}

		public int Count() {
			return GetQuery()
				.Select(Projections.RowCount())
				.GetExecutableQueryOver(Session)
				.SingleOrDefault<int>();
		}

		public bool Any() {
			return Count() > 0;
		}
	}
}
