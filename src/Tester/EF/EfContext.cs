using System;
using System.Data.Entity;

namespace PageOfBob.Comparison.EF {
	public class EfContext : DbContext  {
		public EfContext() : base("EFConnectionString") { }
		
		public DbSet<User> Users { get; set; }
		public DbSet<Thing> Things { get; set; }
		public DbSet<Event> Events { get; set; }
		public DbSet<Work> Work { get; set; }
		
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
					break;
				case InheritenceStrategy.TablePerConcreteClass:
					modelBuilder.Entity<User>().Map(x => {
                    	x.MapInheritedProperties();
                    	x.ToTable("User");
                    });
					modelBuilder.Entity<Thing>().Map(x => {
                    	x.MapInheritedProperties();
                    	x.ToTable("Thing");
                    });
					modelBuilder.Entity<Event>().Map(x => {
                    	x.MapInheritedProperties();
                    	x.ToTable("Event");
                    });
					modelBuilder.Entity<Work>().Map(x => {
                    	x.MapInheritedProperties();
                    	x.ToTable("Work");
                    });
					break;
				default:
					throw new Exception("Invalid value for InheritenceStrategy");
			}
			
		}
	}
}
