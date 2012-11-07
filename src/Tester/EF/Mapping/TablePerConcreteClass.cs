﻿using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;

namespace PageOfBob.Comparison.EF.Mapping.TablePerConcreteClass {
	public class BaseObjectMapping<T> : EntityTypeConfiguration<T> where T : BaseObject  {
		public BaseObjectMapping() {
			HasKey(x => x.ID);
			Property(x => x.ID)
				.HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
			
			Property(x => x.Created).IsRequired();
			Property(x => x.Modified).IsOptional();
			Property(x => x.Deleted).IsRequired();
		}
	}
	
	public class UserMapping : BaseObjectMapping<User> {
		public UserMapping() {
			Property(x => x.Username)
				.HasMaxLength(64)
				.IsRequired();
			Property(x => x.Salt)
				.HasMaxLength(32)
				.IsRequired();
			Property(x => x.Hash)
				.HasMaxLength(32)
				.IsRequired();
			
			HasMany(x => x.Things)
				.WithRequired(i => i.Owner)
				/* .Map(x => x.MapKey("FK_Thing_User")) */;
		}
	}
	
	public class ThingMapping : BaseObjectMapping<Thing> {
		public ThingMapping() {
			Property(x => x.Name)
				.HasMaxLength(256);

			HasRequired(x => x.Owner);

			HasMany(x => x.Events)
				.WithRequired(i => i.Thing);
		}
	}
	
	public class EventMapping : BaseObjectMapping<Event> {
		public EventMapping() {
			Property(x => x.Name)
				.HasMaxLength(256);
			
			Property(x => x.Date);
			
			Property(x => x.Counter)
				.IsOptional();
			
			HasRequired(x => x.Thing);

			HasMany(x => x.WorkDone)
				.WithMany(x => x.Instances);
		}
	}
}
