using System;

using FluentNHibernate.Mapping;

// One table per concrete type (no base type tables)

namespace PageOfBob.Comparison.NH.Mapping.TablePerConcreteClass {
	public abstract class BaseObjectMapping<T> : ClassMap<T> where T : BaseObject {
		protected BaseObjectMapping() {
			Id(x => x.ID)
				.GeneratedBy.GuidComb();

			Map(x => x.Created);
			Map(x => x.Modified);
			Map(x => x.Deleted);
		}
	}

	public class UserMapping : BaseObjectMapping<User> {
		public UserMapping() {
			Map(x => x.Username);
			Map(x => x.Salt);
			Map(x => x.Hash);

			HasMany(x => x.Things)
				.Cascade.AllDeleteOrphan()
				.AsSet();
		}
	}

	public class ThingMapping : BaseObjectMapping<Thing> {
		public ThingMapping() {
			Map(x => x.Name);

			References(x => x.Owner);

			HasMany(x => x.Events)
				.Cascade.AllDeleteOrphan()
				.AsSet();
		}
	}

	public class EventMapping : BaseObjectMapping<Event> {
		public EventMapping() {
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

	public class WorkMapping : BaseObjectMapping<Work> {
		public WorkMapping() {
			Map(x => x.Name);
			Map(x => x.EveryCounter);
			Map(x => x.EveryDays);

			HasManyToMany(x => x.Instances)
				.AsSet()
				.Cascade.AllDeleteOrphan();
		}
	}
}
