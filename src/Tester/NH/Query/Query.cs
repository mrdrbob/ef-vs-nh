using System;
using System.Collections.Generic;

using NHibernate.Criterion;

namespace PageOfBob.Comparison.NH.Query {
	public class Query<T> where T : BaseObject {
		public Guid? ID { get; set; }
		public int? Take { get; set; }
		public bool? Deleted { get; set; }

		public Query() { Deleted = false; }

		protected T _alias;
		public virtual QueryOver<T, T> GetQuery() {
			var q = QueryOver.Of<T>(() => _alias);

			if (ID.HasValue)
				q.And(() => _alias.ID == ID.Value);

			if (Deleted.HasValue)
				q.And(() => _alias.Deleted == Deleted.Value);

			if (Take.HasValue)
				q.Take(Take.Value);

			return q;
		}

		public IList<T> ToList(NHibernate.ISession session) {
			return GetQuery()
				.GetExecutableQueryOver(session)
				.List<T>();
		}

		public T FirstOrDefault(NHibernate.ISession session) {
			Take = 1;

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
