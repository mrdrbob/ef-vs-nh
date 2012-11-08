using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace PageOfBob.Comparison.EF.Query {
	public class Query <T, K> where T:BaseObject where K : Criteria {
		public K Criteria { get; private set; }
		public EfContext Context { get; private set; }
		
		public Query(K criteria, EfContext context) {
			Criteria = criteria;
			Context = context;
		}
		
		protected virtual Expression<Func<T, bool>> GetQuery() {
			var predicate = PredicateBuilder.True<T>();
			
			if (Criteria.ID.HasValue)
				predicate = predicate.And(x => x.ID == Criteria.ID.Value);
			
			if (Criteria.Deleted.HasValue)
				predicate = predicate.And(x => x.IsDeleted == Criteria.Deleted.Value);
			
			return predicate;
		}
		
		protected System.Data.Objects.ObjectSet<T> GetSet() {
			var objContext = ((IObjectContextAdapter)Context).ObjectContext;
			var dbSet = objContext.CreateObjectSet<T>();
			return dbSet;
		}
		
		protected IEnumerable<T> ToEnumerable() {
			var q = GetQuery();
			
			var dbSet = GetSet();
			var results = dbSet.Where(q);
			
			if (Criteria.Take.HasValue)
				results = results.Take(Criteria.Take.Value);

			return results;
		}
		
		public IList<T> ToList() {
			return ToEnumerable().ToList();
		}

		public T FirstOrDefault() {
			Criteria.Take = 1;

			var list = ToList();

			if (list.Count == 0)
				return default(T);

			return list[0];
		}

		public int Count() {
			var q = GetQuery();
			
			var objContext = ((IObjectContextAdapter)Context).ObjectContext;
			var dbSet = objContext.CreateObjectSet<T>();
			
			var results = dbSet.Where(q);
			
			if (Criteria.Take.HasValue)
				results = results.Take(Criteria.Take.Value);
			
			return results.Count();
		}

		public bool Any() {
			return Count() > 0;
		}
	}
}
