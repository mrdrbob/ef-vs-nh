using System;
using System.Collections.Generic;

namespace PageOfBob.Comparison {
	public abstract class BaseObject {
		public virtual Guid ID { get; set; }
		public virtual bool IsDeleted { get; set; }
		public virtual DateTime Created { get; set; }
		public virtual DateTime? Modified { get; set; }
		public virtual byte[] TimeStamp { get; set; }
		public virtual int Version { get; set; }

		protected BaseObject() {
			Created = DateTime.Now;
			
			// In NH, you DON'T assign an id, but let
			// NH assign one for you.  NH expects this to be an 
			// empty GUID, which is what NewGuid() will do in a 
			// NH setting.  EF, you are responsible for assigning
			// a unique ID.
			ID = DbFactory.NewGuid();
		}
	}

	public class User : BaseObject {
		public User() {
			#if NHIBERNATE
			// W/ NH, you can always assign this value,
			// but this throws an exception in EF if the object
			// comes from the DB.  If you make the Things property on this
			// class non-virtual, you can then assign this in the constructor,
			// but then objects from the DB lose any value that they had.
			// For the purposes of this sample, the solution is to make set
			// public and assume the end-user is going to always intialize
			// the things collection before use.  A better solution might be
			// to provide a static method that does this initialization that
			// users would use, but EF would not.
			Things = new HashSet<Thing>();
			#endif
		}

		public virtual string Username { get; set; }
		public virtual byte[] Salt { get; set; }
		public virtual byte[] Hash { get; set; }

		public virtual ICollection<Thing> Things {
			get; 
			#if NHIBERNATE
			protected set; 
			#endif
			#if ENTITY
			set;
			#endif
		}
	}

	public class Thing : BaseObject {
		public Thing() {
			#if NHIBERNATE
			Events = new HashSet<Event>();
			#endif
		}

		public virtual string Name { get; set; }
		public virtual User Owner { get; set; }
		public virtual ICollection<Event> Events {
			get;
			#if NHIBERNATE
			protected set; 
			#endif
			#if ENTITY
			set;
			#endif
		}

		public virtual Event Serviced(Event service) {
			Events.Add(service);
			service.Thing = this;
			return service;
		}
	}

	public class Event : BaseObject {
		public Event() {
			#if NHIBERNATE
			WorkDone = new HashSet<Work>();
			#endif
		}

		public virtual string Name { get; set; }
		public virtual DateTime Date { get; set; }
		public virtual int? Counter { get; set; }
		public virtual Thing Thing { get; set; }

		public virtual ICollection<Work> WorkDone {
			get; 
			#if NHIBERNATE
			protected set; 
			#endif
			#if ENTITY
			set;
			#endif
		}

		public virtual Event HadWorkDown(Work work) {
			WorkDone.Add(work);
			work.Instances.Add(this);
			return this;
		}
	}

	public class Work : BaseObject {
		public Work() {
			#if NHIBERNATE
			Instances = new HashSet<Event>();
			#endif
		}

		public virtual string Name { get; set; }
		public virtual int? EveryCounter { get; set; }
		public virtual int? EveryDays { get; set; }

		public virtual ICollection<Event> Instances {
			get;
			#if NHIBERNATE
			protected set; 
			#endif
			#if ENTITY
			set;
			#endif
		}
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
