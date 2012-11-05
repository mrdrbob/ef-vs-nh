﻿using System;
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

			NH.SessionFactory.Locking = NH.SessionFactory.LockingStrategy.OptimisticVersionInteger;
			var factory = NH.SessionFactory.Get(NH.SessionFactory.InheritenceStrategy.TablePerConcreteClass);
			
			Console.Write("ExportSchema");
			NH.SessionFactory.ExportSchema(factory);
			Console.WriteLine("   [OK]");

			Console.WriteLine("Create Bob");
			using (var session = factory.OpenSession()) {
				User bob = CreateBob();

				session.Save(bob);
				session.Flush();
			}

			Console.WriteLine("  [OK]");
			Console.ReadKey();

			Guid theToyota;
			using (var session = factory.OpenSession()) {
				Console.WriteLine("Query for Bob");
				User bob = new NH.Query.UserQuery { 
					Username = "Bob",
					// JoinWork = true
				}.FirstOrDefault(session);
				
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

			using (var session = factory.OpenSession()) {
				Console.WriteLine("Query for Bob, joining everything");
				var list = new NH.Query.UserQuery {
					Username = "Bob"
				}.Flatten(session);

				Console.WriteLine("  [OK]");
				Console.ReadKey();

				Console.WriteLine("Bob's Things:");
				foreach (var t in list) {
					Console.WriteLine("{0} {1} {2} {3} {4} {5}", t.UserName, t.ThingName, t.EventName, t.Date, t.Counter, t.WorkDone);
				}
				
				Console.WriteLine("  [OK]");
				Console.ReadKey();
			}

			using (var session = factory.OpenSession()) {
				// using(var trans = session.BeginTransaction()) {
					var toyota = session.Get<Thing>(theToyota);
					toyota.Name = "2012 Toyota Camery";
					toyota.Modified = DateTime.Now;
					
					Console.WriteLine("Update some work");
					// trans.Commit();
					session.Flush();
				// }
			}

			

			Console.WriteLine("DONE");
			Console.ReadLine();
		}

		static User CreateBob() {
			User bob = new User {
				Username = "Bob",
				Hash = new Byte[] { 0, 1, 2, 3, 4, 5 },
				Salt = new Byte[] { 6, 7, 8, 9, 10, 0xA }
			};
			Thing camery;
			bob.Things.Add(camery = new Thing {
				Owner = bob,
				Name = "1992 Toyota Camery"
			});

			Work changeOil = new Work {
				Name = "Oil Change",
				EveryCounter = 3000
			};
			Work changeAlternator = new Work {
				Name = "Alternator Change"
			};

			
			camery.Serviced(new Event {
				Name = "Serviced at Roger's",
				Date = DateTime.Now.Date,
				Counter = 100005
			})
			.HadWorkDown(changeOil)
			.HadWorkDown(changeAlternator);

			/*
			Work changeOil = new Work { Name = "Oil Change" };
			Event oilChanged = new Event {
				Thing = camery,
				Name = "Serviced @ Roger's",
				Counter = 100005,
				Date = DateTime.Now.Date
			};
			camery.Events.Add(oilChanged);

			oilChanged.WorkDone.Add(changeOil);
			changeOil.Instances.Add(oilChanged);
			*/

			return bob;
		}
	}
}