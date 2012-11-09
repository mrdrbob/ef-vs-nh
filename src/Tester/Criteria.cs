using System;
using System.Collections.Generic;

namespace PageOfBob.Comparison {
	public class Criteria {
		public Guid? ID { get; set; }
		public int? Take { get; set; }
		public bool? Deleted { get; set; }
		
		public Criteria() {
			Deleted = false;
		}
	}
	
	public class UserCriteria : Criteria {
		public string Username { get; set; }
		public bool JoinThings { get; set; }
		public bool JoinEvents { get; set; }
		public bool JoinWork { get; set; }
	}
	
	public class ThingCriteria : Criteria {
		public string Name { get; set; }
		public UserCriteria MatchesUser { get; set; }
	}
}
