using FluentNHibernate.Mapping;


// One table with a discriminator column
namespace PageOfBob.Comparison.NH.Mapping.TablePerHierachy {
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

			DiscriminateSubClassesOnColumn("ClassType");
		}
	}

	public class UserMapping : SubclassMap<User> {
		public UserMapping() {
			DiscriminatorValue("User");

			Map(x => x.Username)
				.Length(64);
			Map(x => x.Salt)
				.Length(32);
			Map(x => x.Hash)
				.Length(32);

			HasMany(x => x.Things)
				.Cascade.AllDeleteOrphan()
				.AsSet()
				.ForeignKeyConstraintName("FK_User_Things");
		}
	}

	public class ThingMapping : SubclassMap<Thing> {
		public ThingMapping() {
			DiscriminatorValue("Thing");
			
			Map(x => x.Name)
				.Length(256);

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
			DiscriminatorValue("Event");
			
			Map(x => x.Name)
				.Length(256);
			
			Map(x => x.Date);
			
			Map(x => x.Counter)
				.Nullable();
			
			References(x => x.Thing)
				.ForeignKey("FK_Event_Thing");

			HasManyToMany(x => x.WorkDone)
				.AsSet()
				.Cascade.AllDeleteOrphan()
				.Inverse();
		}
	}

	public class WorkMapping : SubclassMap<Work> {
		public WorkMapping() {
			DiscriminatorValue("Work");
			
			Map(x => x.Name)
				.Length(256);
			
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
