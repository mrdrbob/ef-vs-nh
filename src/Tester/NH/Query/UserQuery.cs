using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NHibernate.Transform;

namespace PageOfBob.Comparison.NH.Query {
	public class UserQuery : Query<User, UserCriteria> {

		public UserQuery(UserCriteria criteria) : base(criteria) { }
		
		protected Thing _thingAlias;
		protected Event _eventAlias;
		protected Work _workAlias;

		public override NHibernate.Criterion.QueryOver<User, User> GetQuery() {
			var q = base.GetQuery();

			if (!string.IsNullOrEmpty(Criteria.Username))
				q = q.And(x => x.Username == Criteria.Username);

			if (Criteria.JoinThings || Criteria.JoinEvents || Criteria.JoinWork) {
				q = q.JoinAlias(() => _alias.Things, () => _thingAlias);
			}
			if (Criteria.JoinEvents || Criteria.JoinWork) {
				q = q.JoinAlias(() => _thingAlias.Events, () => _eventAlias);
			}
			if (Criteria.JoinWork) {
				q = q.JoinAlias(() => _eventAlias.WorkDone, () => _workAlias);
			}
			return q;
		}

		public IList<FlatView> Flatten(NHibernate.ISession session) {
			Criteria.JoinWork = true;

			var q = GetQuery();

			FlatView alias = null;
			q.SelectList(l => l
				.Select(x => x.Username).WithAlias(() => alias.UserName)
				.Select(() => _thingAlias.Name).WithAlias(() => alias.ThingName)
				.Select(() => _eventAlias.Name).WithAlias(() => alias.EventName)
				.Select(() => _eventAlias.Date).WithAlias(() => alias.Date)
				.Select(() => _eventAlias.Counter).WithAlias(() => alias.Counter)
				.Select(() => _workAlias.Name).WithAlias(() => alias.WorkDone)
			);

			return q.TransformUsing(Transformers.AliasToBean<FlatView>())
				.GetExecutableQueryOver(session)
				.List<FlatView>();
		}
	}
}
