using System;
using System.Linq;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace PageOfBob.Comparison.EF {
	public class EfDatabase : IDbAbstraction {
		private DbContext _context;
		public EfDatabase(DbContext context) {
			_context = context;
		}
		
		public T Get<T>(Guid id) where T : BaseObject {
			var objContext = ((IObjectContextAdapter)_context).ObjectContext;
			return objContext.CreateObjectSet<T>().SingleOrDefault(x => x.ID == id);
		}
		
		public void Insert<T>(T obj) where T : BaseObject {
			var objContext = ((IObjectContextAdapter)_context).ObjectContext;
			objContext.CreateObjectSet<T>().AddObject(obj);
			FlushChanges();
		}
		
		public void FlushChanges() {
			_context.SaveChanges();
		}
		
		public IUserQuery GetUserQuery(UserCriteria criteria) {
			if (DbFactory.InheritenceStrategy == InheritenceStrategy.TablePerConcreteClass)
				return new Query.UserQuery<EfContext>(criteria, (EfContext)_context);
			else
				return new Query.UserQuery<EfBaseItemContext>(criteria, (EfBaseItemContext)_context);
		}
		
		public void Dispose() {
			_context.Dispose();
		}
	}
}
