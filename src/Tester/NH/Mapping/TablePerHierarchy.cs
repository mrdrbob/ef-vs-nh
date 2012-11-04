using FluentNHibernate.Mapping;


// One table with a discriminator column
namespace PageOfBob.Comparison.NH.Mapping.TablePerHierachy {
	public class BaseObjectMapping : ClassMap<BaseObject> {
		public BaseObjectMapping() {
			Id(x => x.ID)
				.GeneratedBy.GuidComb();

			Map(x => x.Created);
			Map(x => x.Modified);
			Map(x => x.Deleted);

			DiscriminateSubClassesOnColumn("ClassType");
		}
	}

	public class UserMapping : SubclassMap<User> {
		public UserMapping() {
			DiscriminatorValue("User");

			Map(x => x.Username);
			Map(x => x.Salt);
			Map(x => x.Hash);

			HasMany(x => x.Things)
				.Cascade.AllDeleteOrphan()
				.AsSet();
		}
	}

	public class ThingMapping : SubclassMap<Thing> {
		public ThingMapping() {
			DiscriminatorValue("Thing");
			Map(x => x.Name);

			References(x => x.Owner);

			HasMany(x => x.Events)
				.Cascade.AllDeleteOrphan()
				.AsSet();
		}
	}

	public class EventMapping : SubclassMap<Event> {
		public EventMapping() {
			DiscriminatorValue("Event");
			Map(x => x.Name);
			Map(x => x.Date);
			Map(x => x.Counter);
			References(x => x.Thing);

			HasManyToMany(x => x.WorkDone)
				.AsSet()
				.Cascade.AllDeleteOrphan()
				.Inverse();
		}
	}

	public class WorkMapping : SubclassMap<Work> {
		public WorkMapping() {
			DiscriminatorValue("Work");
			Map(x => x.Name);
			Map(x => x.EveryCounter);
			Map(x => x.EveryDays);

			HasManyToMany(x => x.Instances)
				.AsSet()
				.Cascade.AllDeleteOrphan();
		}
	}
}
