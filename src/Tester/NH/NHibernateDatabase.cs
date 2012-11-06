using System;
using NHibernate;

namespace PageOfBob.Comparison.NH {
	public class NHibernateDatabase : IDbAbstraction {
		ISession _session;
		public NHibernateDatabase(ISessionFactory factory) {
			_session = factory.OpenSession();
		}
		
		public T Get<T>(Guid id) where T : BaseObject { return _session.Get<T>(id); }
		
		public void Insert<T>(T obj)  where T : BaseObject {
			_session.Save(obj);
			FlushChanges();
		}
		
		public void FlushChanges() { _session.Flush(); }
		
		public void Dispose() { _session.Dispose(); }
		
		public IUserQuery GetUserQuery(UserCriteria criteria) {
			return new Query.UserQuery(criteria, _session);
		}
	}
}
