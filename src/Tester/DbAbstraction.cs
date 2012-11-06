using System;
using System.Collections.Generic;

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
		internal static Func<IDbAbstraction> GetDatabaseFunc;
		internal static Func<Guid> NewIdFunc;
		
		public static IDbAbstraction GetDatabase() {
			return GetDatabaseFunc();
		}
		
		public static Guid NewGuid() {
			return NewIdFunc();
		}
	}
}
