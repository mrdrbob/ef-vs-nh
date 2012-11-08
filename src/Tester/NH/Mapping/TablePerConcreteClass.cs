using System;

using FluentNHibernate.Mapping;

// One table per concrete type (no base type tables)

namespace PageOfBob.Comparison.NH.Mapping.TablePerConcreteClass {
	public abstract class BaseObjectMapping<T> : ClassMap<T> where T : BaseObject {
		protected BaseObjectMapping() {
			Id(x => x.ID)
				.GeneratedBy.GuidComb();

			switch (SessionFactory.Locking) {
				case SessionFactory.LockingStrategy.OptimisticDirty:
					OptimisticLock.Dirty();
					DynamicUpdate();
					break;
				case SessionFactory.LockingStrategy.OptimisticAll:
					OptimisticLock.All();
					DynamicUpdate();
					break;
				case SessionFactory.LockingStrategy.OptimisticVersionTimestamp:
					DynamicUpdate();
					OptimisticLock.Version();
					Version(x => x.TimeStamp)
						.Generated.Always()
						.CustomSqlType("timestamp")
						.UnsavedValue(null);
					break;
				case SessionFactory.LockingStrategy.OptimisticVersionInteger:
					DynamicUpdate();
					OptimisticLock.Version();
					Version(x => x.Version);
					break;
				case SessionFactory.LockingStrategy.None:
					OptimisticLock.None();
					break;
				default:
					throw new Exception("Invalid value for LockingStrategy");
			}
			
			Map(x => x.Created)
				.Not.Nullable();
			Map(x => x.Modified)
				.Nullable();
			Map(x => x.IsDeleted)
				.Not.Nullable();
		}
	}

	public class UserMapping : BaseObjectMapping<User> {
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

	public class ThingMapping : BaseObjectMapping<Thing> {
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

	public class EventMapping : BaseObjectMapping<Event> {
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
				.Inverse();
		}
	}

	public class WorkMapping : BaseObjectMapping<Work> {
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
