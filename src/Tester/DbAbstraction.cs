using System;
using System.Collections.Generic;

using NHibernate;

namespace PageOfBob.Comparison {
	public interface IDbAbstraction : IDisposable {
		T Get<T>(Guid id) where T : BaseObject;
		void Insert<T>(T obj)  where T : BaseObject;
		void FlushChanges();
		IUserQuery GetUserQuery(UserCriteria criteria);
	}
	
	public interface IQuery<T> where T : BaseObject {
		IList<T> ToList();
		T FirstOrDefault();
		int Count();
		bool Any();
	}
	
	public interface IUserQuery : IQuery<User> {
		IList<FlatView> Flatten();
	}
	
	public static class DbFactory {
		#if NHIBERNATE
		public static ISessionFactory Factory = NH.SessionFactory.Get(NH.SessionFactory.InheritenceStrategy.TablePerConcreteClass);
		#endif
		
		public static IDbAbstraction GetDatabase() {
			#if ENTITY
			return new EF.EfDatabase(new EF.EfContext());
			#endif
			
			#if NHIBERNATE
			return new NH.NHibernateDatabase(Factory);
			#endif
		}
		
		public static Guid NewGuid() {
			#if ENTITY
			return Guid.NewGuid();
			#endif
			#if NHIBERNATE
			return default(Guid);
			#endif
		}
	}
}
