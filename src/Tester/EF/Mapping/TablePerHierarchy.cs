using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;

namespace PageOfBob.Comparison.EF.Mapping {
	public class BaseObjectMapping<T> : EntityTypeConfiguration<T> where T : BaseObject  {
		public BaseObjectMapping() {
			HasKey(x => x.ID);
			Property(x => x.ID)
				.HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
			
			Property(x => x.Created).IsRequired();
			Property(x => x.Modified).IsOptional();
			Property(x => x.Deleted).IsRequired();
			
			Map(x => x.Requires("ClassType").HasValue("BaseObject"));
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
			
			Map(x => x.Requires("ClassType").HasValue("UserMapping"));
		}
	}
}
