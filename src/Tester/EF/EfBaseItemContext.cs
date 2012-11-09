using System;
using System.Data.Entity;

namespace PageOfBob.Comparison.EF
{
	public class EfBaseItemContext  : DbContext  {
		public EfBaseItemContext() : base("EFConnectionString") { }
		
		public DbSet<BaseObject> Objects { get; set; }
		
		protected override void OnModelCreating(DbModelBuilder modelBuilder) {
			// base.OnModelCreating(modelBuilder);
			switch (DbFactory.InheritenceStrategy) {
				case InheritenceStrategy.TablePerHierachy:
					modelBuilder.Entity<BaseObject>()
						.Map<User>(x => x.Requires("ObjType").HasValue("User"))
						.Map<Thing>(x => x.Requires("ObjType").HasValue("Thing"))
						.Map<Event>(x => x.Requires("ObjType").HasValue("Event"))
						.Map<Work>(x => x.Requires("ObjType").HasValue("Work"));
					break;
				case InheritenceStrategy.TablePerType:
					modelBuilder.Entity<User>().ToTable("User");
					modelBuilder.Entity<Thing>().ToTable("Thing");
					modelBuilder.Entity<Event>().ToTable("Event");
					modelBuilder.Entity<Work>().ToTable("Work");
					base.OnModelCreating(modelBuilder);
					break;
				default:
					throw new NotImplementedException();
			}
			
		}
	}
}
