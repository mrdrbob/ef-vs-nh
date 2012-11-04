using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NHibernate.Transform;

namespace PageOfBob.Comparison.NH.Query {
	public class UserQuery : Query<User> {
		public string Username { get; set; }
		public bool JoinThings { get; set; }
		public bool JoinEvents { get; set; }
		public bool JoinWork { get; set; }

		protected Thing _thingAlias;
		protected Event _eventAlias;
		protected Work _workAlias;

		public override NHibernate.Criterion.QueryOver<User, User> GetQuery() {
			var q = base.GetQuery();

			if (!string.IsNullOrEmpty(Username))
				q = q.And(x => x.Username == Username);

			if (JoinThings || JoinEvents || JoinWork) {
				q = q.JoinAlias(() => _alias.Things, () => _thingAlias);
			}
			if (JoinEvents || JoinWork) {
				q = q.JoinAlias(() => _thingAlias.Events, () => _eventAlias);
			}
			if (JoinWork) {
				q = q.JoinAlias(() => _eventAlias.WorkDone, () => _workAlias);
			}
			return q;
		}

		public IList<FlatView> Flatten(NHibernate.ISession session) {
			JoinWork = true;

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
