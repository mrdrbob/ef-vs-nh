using System;
using System.Collections.Generic;

namespace PageOfBob.Comparison {
	public abstract class BaseObject {
		public virtual Guid ID { get; set; }
		public virtual bool Deleted { get; set; }
		public virtual DateTime Created { get; set; }
		public virtual DateTime? Modified { get; set; }
		public virtual byte[] TimeStamp { get; set; }
		public virtual int Version { get; set; }

		protected BaseObject() { Created = DateTime.Now; }
	}

	public class User : BaseObject {
		public User() { Things = new HashSet<Thing>(); }

		public virtual string Username { get; set; }
		public virtual byte[] Salt { get; set; }
		public virtual byte[] Hash { get; set; }

		public virtual ICollection<Thing> Things { get; protected set; }
	}

	public class Thing : BaseObject {
		public Thing() { Events = new HashSet<Event>(); }

		public virtual string Name { get; set; }
		public virtual User Owner { get; set; }
		public virtual ICollection<Event> Events { get; protected set; }

		public virtual Event Serviced(Event service) {
			Events.Add(service);
			service.Thing = this;
			return service;
		}
	}

	public class Event : BaseObject {
		public Event() { WorkDone = new HashSet<Work>(); }

		public virtual string Name { get; set; }
		public virtual DateTime Date { get; set; }
		public virtual int? Counter { get; set; }
		public virtual Thing Thing { get; set; }

		public virtual ICollection<Work> WorkDone { get; protected set; }

		public virtual Event HadWorkDown(Work work) {
			WorkDone.Add(work);
			work.Instances.Add(this);
			return this;
		}
	}

	public class Work : BaseObject {
		public Work() { Instances = new HashSet<Event>(); }

		public virtual string Name { get; set; }
		public virtual int? EveryCounter { get; set; }
		public virtual int? EveryDays { get; set; }

		public virtual ICollection<Event> Instances { get; protected set; }
	}

	public class FlatView {
		public string UserName { get; set; }
		public string ThingName { get; set; }
		public string EventName { get; set; }
		public string WorkDone { get; set; }
		public DateTime Date { get; set; }
		public int? Counter { get; set; }
	}
}
