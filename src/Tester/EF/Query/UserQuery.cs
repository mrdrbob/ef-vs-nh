using System;
using System.Linq;

namespace PageOfBob.Comparison.EF.Query {
	public class UserQuery : Query<User, UserCriteria>, IUserQuery {
		public UserQuery(UserCriteria criteria, EfContext context) : base(criteria, context) { }
		
		public System.Collections.Generic.IList<FlatView> Flatten() {
			var enumerable = ToEnumerable();
			throw new NotImplementedException();
		}
	}
}
