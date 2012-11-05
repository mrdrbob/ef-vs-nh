using System;

using NHibernate;
using NHibernate.Cfg;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate.Tool.hbm2ddl;

namespace PageOfBob.Comparison.NH {
	public class SessionFactory {
		private static object _lock = new object();
		private static Configuration _config;

		public enum InheritenceStrategy {
			TablePerHierachy,
			TablePerType,
			TablePerConcreteClass
		}
		
		public enum LockingStrategy {
			OptimisticDirty,
			OptimisticAll,
			OptimisticVersionTimestamp,
			OptimisticVersionInteger,
			None
		}
		
		public static LockingStrategy Locking = LockingStrategy.OptimisticDirty;

		/// <summary>
		/// Gives a reference to the one ISessionFactory in this application.
		/// </summary>
		public static ISessionFactory Get(InheritenceStrategy strat) {
			var config =
				MsSqlConfiguration.MsSql2008
				.ConnectionString(e => e.FromConnectionStringWithKey("ConnectionString"))
				.ShowSql();

			Action<MappingConfiguration> mapping;
			switch (strat) {
				case InheritenceStrategy.TablePerHierachy:
					mapping = AddTablePerHierarchyMappings;
					break;
				case InheritenceStrategy.TablePerType:
					mapping = AddTablePerTypeMappings;
					break;
				case InheritenceStrategy.TablePerConcreteClass:
					mapping = AddTablePerConcreteClassMappings;
					break;
				default:
					throw new NotImplementedException();
			}

			return Fluently.Configure()
				.Database(config)
				.ExposeConfiguration(x => _config = x)
				.Mappings(mapping)
				.BuildSessionFactory();
		}

		private static void AddTablePerHierarchyMappings(MappingConfiguration conf) {
			conf.FluentMappings.Add<PageOfBob.Comparison.NH.Mapping.TablePerHierachy.BaseObjectMapping>();
			conf.FluentMappings.Add<PageOfBob.Comparison.NH.Mapping.TablePerHierachy.UserMapping>();
			conf.FluentMappings.Add<PageOfBob.Comparison.NH.Mapping.TablePerHierachy.ThingMapping>();
			conf.FluentMappings.Add<PageOfBob.Comparison.NH.Mapping.TablePerHierachy.EventMapping>();
			conf.FluentMappings.Add<PageOfBob.Comparison.NH.Mapping.TablePerHierachy.WorkMapping>();
		}

		private static void AddTablePerTypeMappings(MappingConfiguration conf) {
			conf.FluentMappings.Add<PageOfBob.Comparison.NH.Mapping.TablePerType.BaseObjectMapping>();
			conf.FluentMappings.Add<PageOfBob.Comparison.NH.Mapping.TablePerType.UserMapping>();
			conf.FluentMappings.Add<PageOfBob.Comparison.NH.Mapping.TablePerType.ThingMapping>();
			conf.FluentMappings.Add<PageOfBob.Comparison.NH.Mapping.TablePerType.EventMapping>();
			conf.FluentMappings.Add<PageOfBob.Comparison.NH.Mapping.TablePerType.WorkMapping>();
		}

		private static void AddTablePerConcreteClassMappings(MappingConfiguration conf) {
			conf.FluentMappings.Add<PageOfBob.Comparison.NH.Mapping.TablePerConcreteClass.UserMapping>();
			conf.FluentMappings.Add<PageOfBob.Comparison.NH.Mapping.TablePerConcreteClass.ThingMapping>();
			conf.FluentMappings.Add<PageOfBob.Comparison.NH.Mapping.TablePerConcreteClass.EventMapping>();
			conf.FluentMappings.Add<PageOfBob.Comparison.NH.Mapping.TablePerConcreteClass.WorkMapping>();
		}

		public static void ExportSchema(ISessionFactory factory) {
			var schema = new SchemaExport(_config);
			schema.Create(true, true);
		}
	}
}
