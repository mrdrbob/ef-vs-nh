using System;

using FluentNHibernate.Mapping;

// One table for each base type and child types.
namespace PageOfBob.Comparison.NH.Mapping.TablePerType {
	public class BaseObjectMapping : ClassMap<BaseObject> {
		public BaseObjectMapping() {
			Id(x => x.ID)
				.GeneratedBy.GuidComb();

			Map(x => x.Created)
				.Not.Nullable();
			Map(x => x.Modified)
				.Nullable();
			Map(x => x.IsDeleted)
				.Not.Nullable();
		}
	}

	public class UserMapping : SubclassMap<User> {
		public UserMapping() {
			Map(x => x.Username)
				.Length(64)
				.Not.Nullable();
			Map(x => x.Salt)
				.Length(32)
				.Not.Nullable();
			Map(x => x.Hash)
				.Length(32)
				.Not.Nullable();

			HasMany(x => x.Things)
				.Cascade.AllDeleteOrphan()
				.AsSet()
				.ForeignKeyConstraintName("FK_User_Things");
		}
	}

	public class ThingMapping : SubclassMap<Thing> {
		public ThingMapping() {
			Map(x => x.Name)
				.Length(256)
				.Not.Nullable();

			References(x => x.Owner)
				.ForeignKey("FK_Thing_Owner");

			HasMany(x => x.Events)
				.Cascade.AllDeleteOrphan()
				.AsSet()
				.ForeignKeyConstraintName("FK_Thing_User");
		}
	}

	public class EventMapping : SubclassMap<Event> {
		public EventMapping() {
			Map(x => x.Name)
				.Length(256)
				.Not.Nullable();
			
			Map(x => x.Date)
				.Not.Nullable();
			
			Map(x => x.Counter)
				.Nullable();
			
			References(x => x.Thing)
				.ForeignKey("FK_Event_Thing");

			HasManyToMany(x => x.WorkDone)
				.AsSet()
				.Cascade.AllDeleteOrphan()
				.Inverse();		}
	}

	public class WorkMapping : SubclassMap<Work> {
		public WorkMapping() {
			Map(x => x.Name)
				.Length(256)
				.Not.Nullable();
			
			Map(x => x.EveryCounter)
				.Nullable();
			Map(x => x.EveryDays)
				.Nullable();

			HasManyToMany(x => x.Instances)
				.AsSet()
				.Cascade.AllDeleteOrphan()
				.Table("WorkInstance")
				.ForeignKeyConstraintNames("FK_Work_Instanace", "FK_Instance_Work");
		}
	}
}
