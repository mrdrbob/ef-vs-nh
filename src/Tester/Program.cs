using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using System.Data.SqlClient;

using System.Data.Entity;

namespace PageOfBob.Comparison {
	class Program {
		static void Main(string[] args) {

			Console.Write("Dropping DB");
			using(SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
			using(SqlCommand cmd = new SqlCommand("USE MASTER; DROP DATABASE TestDb; CREATE DATABASE TestDb;", conn)) {
				conn.Open();
				cmd.ExecuteNonQuery();
				conn.Close();
			}
			Console.WriteLine("   [OK]");
			
			DbFactory.InheritenceStrategy = InheritenceStrategy.TablePerType;
			
			#if NHIBERNATE
			NH.SessionFactory.Locking = NH.SessionFactory.LockingStrategy.OptimisticVersionInteger;
			Console.Write("ExportSchema");
			NH.SessionFactory.ExportSchema(DbFactory.Factory);
			Console.WriteLine("   [OK]");
			#endif

			Console.WriteLine("Create Bob");
			using (var session = DbFactory.GetDatabase()) {
				User bob = CreateBob();

				#if ENTITY
				if (DbFactory.InheritenceStrategy == InheritenceStrategy.TablePerHierachy)
					session.Insert((BaseObject)bob);
				else
					session.Insert(bob);
				#endif
				#if NHIBERNATE
				session.Insert(bob);
				#endif
			}

			Console.WriteLine("  [OK]");
			Console.ReadKey();

			Guid theToyota;
			using (var session = DbFactory.GetDatabase()) {
				Console.WriteLine("Query for Bob");
				User bob = session.GetUserQuery(new UserCriteria {
					Username = "Bob"
				}).FirstOrDefault();
				
				Console.WriteLine("  [OK]");
				Console.ReadKey();

				Console.WriteLine("Bob's Things:");
				foreach (Thing t in bob.Things) {
					Console.WriteLine(t.Name);
					foreach (Event e in t.Events) {
						Console.WriteLine("\t{0} - {1} - {2}", e.Name, e.Date, e.Counter);
						foreach (Work w in e.WorkDone) {
							Console.WriteLine("\t\t{0}", w.Name);
						}
					}
				}
				
				theToyota = bob.Things.First().ID;
				Console.WriteLine("  [OK]");
				Console.ReadKey();
			}

			using (var session = DbFactory.GetDatabase()) {
				Console.WriteLine("Query for Bob, joining everything");
				var list = session.GetUserQuery(new UserCriteria {
					Username = "Bob"
				}).Flatten();

				Console.WriteLine("  [OK]");
				Console.ReadKey();

				Console.WriteLine("Bob's Things:");
				foreach (var t in list) {
					Console.WriteLine("{0} {1} {2} {3} {4} {5}", t.UserName, t.ThingName, t.EventName, t.Date, t.Counter, t.WorkDone);
				}
				
				Console.WriteLine("  [OK]");
				Console.ReadKey();
			}

			using (var session = DbFactory.GetDatabase()) {
				var toyota = session.Get<Thing>(theToyota);
				toyota.Name = "2012 Toyota Camery";
				toyota.Modified = DateTime.Now;
				
				Console.WriteLine("Update some work");
				session.FlushChanges();
			}

			

			Console.WriteLine("DONE");
			Console.ReadLine();
		}

		static User CreateBob() {
			User bob = new User {
				Username = "Bob",
				Hash = new Byte[] { 0, 1, 2, 3, 4, 5 },
				Salt = new Byte[] { 6, 7, 8, 9, 10, 0xA }
				#if ENTITY
				,Things = new HashSet<Thing>()
				#endif
			};
			Thing camery;
			bob.Things.Add(camery = new Thing {
				Owner = bob,
				Name = "1992 Toyota Camery"
				#if ENTITY
				,Events = new List<Event>()
				#endif
			});

			Work changeOil = new Work {
				Name = "Oil Change",
				EveryCounter = 3000
				#if ENTITY
				,Instances = new List<Event>()
				#endif
			};
			Work changeAlternator = new Work {
				Name = "Alternator Change"
				#if ENTITY
				,Instances = new List<Event>()
				#endif
			};

			
			camery.Serviced(new Event {
				Name = "Serviced at Roger's",
				Date = DateTime.Now.Date,
				Counter = 100005
				#if ENTITY
				,WorkDone = new List<Work>()
				#endif
			})
			.HadWorkDown(changeOil)
			.HadWorkDown(changeAlternator);

			return bob;
		}
	}
}