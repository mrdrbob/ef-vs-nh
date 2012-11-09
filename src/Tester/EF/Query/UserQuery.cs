using System;
using System.Linq;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq.Expressions;

namespace PageOfBob.Comparison.EF.Query {
	public class UserQuery<C> : Query<User, UserCriteria, C>, IUserQuery where C : DbContext {
		public UserQuery(UserCriteria criteria, C context) : base(criteria, context) { }

		protected override Expression<Func<User, bool>> GetQuery() {
			var q = base.GetQuery();
			
			if (!string.IsNullOrEmpty(Criteria.Username)) {
				q = q.And(x => x.Username == Criteria.Username);
			}
			
			return q;
		}
		
		protected override IQueryable<User> GetSet() {
			if (DbFactory.InheritenceStrategy == InheritenceStrategy.TablePerConcreteClass) {
				EfContext ctx = Context as EfContext;
				return ctx.Users;
			} else {
				EfBaseItemContext ctx = Context as EfBaseItemContext;
				return ctx.Objects.OfType<User>();
			}
		}
		
		public System.Collections.Generic.IList<FlatView> Flatten() {
			
			var query = GetQuery();

			var objContext = ((IObjectContextAdapter)Context).ObjectContext;
			var userQuery = objContext.CreateObjectSet<User>();
			var things = objContext.CreateObjectSet<Thing>();
			var events = objContext.CreateObjectSet<Event>();
			var work = objContext.CreateObjectSet<Work>();

			/*
			// This will return most of the join we expect, but without the query applied, and weird query.
			// SELECT 
			// 1 AS [C1], 
			// [Extent1].[Username] AS [Username], 
			// [Extent2].[Name] AS [Name], 
			// [Extent5].[Name] AS [Name1], 
			// [Extent5].[Counter] AS [Counter], 
			// [Extent5].[Date] AS [Date], 
			// [Join7].[Name] AS [Name2]
			// FROM    [dbo].[Users] AS [Extent1]
			// INNER JOIN [dbo].[Things] AS [Extent2] ON  EXISTS (SELECT 
			// 	1 AS [C1]
			// 	FROM    ( SELECT 1 AS X ) AS [SingleRowTable1]
			// 	LEFT OUTER JOIN  (SELECT 
			// 		[Extent3].[ID] AS [ID]
			// 		FROM [dbo].[Users] AS [Extent3]
			// 		WHERE [Extent2].[Owner_ID] = [Extent3].[ID] ) AS [Project1] ON 1 = 1
			// 	LEFT OUTER JOIN  (SELECT 
			// 		[Extent4].[ID] AS [ID]
			// 		FROM [dbo].[Users] AS [Extent4]
			// 		WHERE [Extent2].[Owner_ID] = [Extent4].[ID] ) AS [Project2] ON 1 = 1
			// 	WHERE [Extent1].[ID] = [Project1].[ID]
			// )
			// INNER JOIN [dbo].[Events] AS [Extent5] ON  EXISTS (SELECT 
			// 	1 AS [C1]
			// 	FROM    ( SELECT 1 AS X ) AS [SingleRowTable2]
			// 	LEFT OUTER JOIN  (SELECT 
			// 		[Extent6].[ID] AS [ID]
			// 		FROM [dbo].[Things] AS [Extent6]
			// 		WHERE [Extent5].[Thing_ID] = [Extent6].[ID] ) AS [Project4] ON 1 = 1
			// 	LEFT OUTER JOIN  (SELECT 
			// 		[Extent7].[ID] AS [ID]
			// 		FROM [dbo].[Things] AS [Extent7]
			// 		WHERE [Extent5].[Thing_ID] = [Extent7].[ID] ) AS [Project5] ON 1 = 1
			// 	WHERE [Extent2].[ID] = [Project4].[ID]
			// )
			// INNER JOIN  (SELECT [Extent8].[Event_ID] AS [Event_ID], [Extent9].[Name] AS [Name]
			// 	FROM  [dbo].[WorkEvents] AS [Extent8]
			// 	INNER JOIN [dbo].[Works] AS [Extent9] ON [Extent9].[ID] = [Extent8].[Work_ID] ) AS [Join7] ON [Extent5].[ID] = [Join7].[Event_ID]

			var t = from user in userQuery
					join thing in things on user equals thing.Owner
					join ev in events on thing equals ev.Thing
					from w in ev.WorkDone
					select new FlatView {
						UserName = user.Username,
						ThingName = thing.Name,
						EventName = ev.Name,
						Counter = ev.Counter,
						Date = ev.Date,
						WorkDone = w.Name
					};
			return t.ToList();
			// */

			/*
			// Similar to above
			// SELECT 
			// 1 AS [C1], 
			// [Extent1].[Username] AS [Username], 
			// [Extent2].[Name] AS [Name], 
			// [Extent3].[Name] AS [Name1], 
			// [Extent3].[Counter] AS [Counter], 
			// [Extent3].[Date] AS [Date], 
			// [Join3].[Name] AS [Name2]
			// FROM    [dbo].[Users] AS [Extent1]
			// INNER JOIN [dbo].[Things] AS [Extent2] ON ([Extent1].[ID] = [Extent2].[Owner_ID]) AND ([Extent2].[Owner_ID] = [Extent1].[ID])
			// INNER JOIN [dbo].[Events] AS [Extent3] ON ([Extent2].[ID] = [Extent3].[Thing_ID]) AND ([Extent3].[Thing_ID] = [Extent2].[ID])
			// INNER JOIN  (SELECT [Extent4].[Event_ID] AS [Event_ID], [Extent5].[ID] AS [ID], [Extent5].[Name] AS [Name]
			// 	FROM  [dbo].[WorkEvents] AS [Extent4]
			// 	INNER JOIN [dbo].[Works] AS [Extent5] ON [Extent4].[Work_ID] = [Extent5].[ID] ) AS [Join3] ON [Extent3].[ID] = [Join3].[Event_ID]
			// WHERE  EXISTS (SELECT 
			// 	1 AS [C1]
			// 	FROM [dbo].[WorkEvents] AS [Extent6]
			// 	WHERE ([Extent3].[ID] = [Extent6].[Event_ID]) AND ([Extent6].[Work_ID] = [Join3].[ID])
			// )
			var t = from user in userQuery
					from thing in user.Things
					from ev in thing.Events
					from w in ev.WorkDone
					where user == thing.Owner && thing == ev.Thing && ev.WorkDone.Contains(w)
					select new FlatView {
						UserName = user.Username,
						ThingName = thing.Name,
						EventName = ev.Name,
						Counter = ev.Counter,
						Date = ev.Date,
						WorkDone = w.Name
					};
			
			return t.ToList();
			// */

			/*
			// Try using the query criteria in the query, but
			// this triggers an exception:
			// There is already an open DataReader associated with this Command which must be closed first.
			// When executing the query, it first materializes q, then attempts to 
			// execute the rest of the query, but fails because the DataReader used in
			// q's materialization is still open.
			var q = ToEnumerable();
			var t = from user in q
					from thing in user.Things
					from ev in thing.Events
					from w in ev.WorkDone
					where user == thing.Owner && thing == ev.Thing && ev.WorkDone.Contains(w)
					select new FlatView {
						UserName = user.Username,
						ThingName = thing.Name,
						EventName = ev.Name,
						Counter = ev.Counter,
						Date = ev.Date,
						WorkDone = w.Name
					};


			return t.ToList();
			// */

			// Conclusion: I can't find a way to do this well.

			throw new NotImplementedException();
		}
	}
}
