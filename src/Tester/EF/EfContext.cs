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
			base.OnModelCreating(modelBuilder);
			modelBuilder.Configurations.Add(new Mapping.TablePerConcreteClass.UserMapping());
		}
	}
}
