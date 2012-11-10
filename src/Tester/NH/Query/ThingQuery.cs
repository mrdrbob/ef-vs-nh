using System;

using NHibernate;
using NHibernate.Criterion;

namespace PageOfBob.Comparison.NH.Query {
	public class ThingQuery : Query<Thing, ThingCriteria> {
		public ThingQuery(ThingCriteria criteria, ISession session) : base(criteria, session) { }
		
		public override QueryOver<Thing, Thing> GetQuery() {
			var q = base.GetQuery();
			
			if (!string.IsNullOrEmpty(Criteria.Name))
				q.And(() => _alias.Name == Criteria.Name);
			
			if (Criteria.BelongsTo != null) {
				var uq = new UserQuery(Criteria.BelongsTo, Session)
					.GetSubquery();
				
				q = q.WithSubquery
					.WhereProperty(() => _alias.Owner.ID).In(uq);
			}
			
			return q;
		}
	}
}
