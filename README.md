Entity Framework Vs NHibernate
============================

An NHibernate guy's journey into the Entity Framework World.

Outline
=======

Let's talk about you:

* Who's used ORM
	* EF?
	* NH?
	* LINQ2SQL?
	* Code-first?

Ego:

* Me: Bob Davidson
	* https://github.com/mrdrbob
	* @funnybob
	* http://gplus.to/bobdavidson
* Professional .NET developer since 2004.
	* Couple years of ColdFusion, which I've been trying to scrub from my memory so please stop bringing it up.
* Currently a developer at Blend Interactive, since Aug '12.
* Long(ish) time NHibnerate user
* Biased against LINQ as a SQL generator

The Myth:

* ORM = Ignore the DB!
* Umbraco V5 - Used NHibernate through LAYERS of abstraction
	* Allow the view to generate queries - NO! BAD DOG!
	* (N + 1) problem - http://ayende.com/blog/156577/on-umbracorsquo-s-nhibernatersquo-s-pullout
	* Forked to Rebel CMS http://www.rebelcms.com/

The Project:

* Available on github: https://github.com/mrdrbob/ef-vs-nh
* "Thing" maintenance app
	* Thing - Car, House, Cat, etc
	* Event - Went to service station, hired contractor, vet, etc
	* Work - Oil change/alternate replacement, roof rebuilt, kitty fluid replaced, etc
* Delete never, track modifications & creation date on ALL items.
	* BaseObject - ID, IsDeleted, Created date/time, Modified date/time, properties used in Optimistic Concurrency (later) 
* GUID for ids
	* Sequential IDs - Guid.Comb

Mapping:

* Both: Fluent, Data Annotations, Automap.
	* Fluent - Control, separation of concerns, manual work.
	* Annotations - Control, less work, decorates your POCO with DB concerns.
	* Automap - Easy, but defaults to nvarchar(max)
* NHibernate: XML
	* Very similar to Hibernate, bracket tax.
* Entity Framework: XML w/ VS designer.
	* Visual designer, seriously?
	* Code-first specifying Foreign Key name - Only in code.
* NHibernate requires properties be marked virtual.
* Guid Identifiers
	* Guid.Comb - Sequential GUIDs to reduce index fragmentation
* Entity Framework has much cleaner / nicer relationship definitions.
* Collections
	* NHibernate - Initialize in constructor, public get, protected set
		* NHibernate uses Hibernate collections
			* Set - Distinct elements, no order - HashSet (ICollection)
			* Bag - Elements, no order - IList
			* Map - IDictionary
			* List - IList - Elements, order
	* Entity Framework - Do not initialize in constructor, public get, set to allow detached entities to create lists.

Inheritance Types:

* Table per Hierarchy - One table per complete hierarchy - uses a "discriminator" column.  X classes in 1 table, which has columns to cover each property of said classes.
	* Most performant (generally).
	* Cannot enforce NOT NULL constraints on properties that are not shared by every class - inserts null into those columns when not the that type.
	* NHibernate will actually allow you to put NOT NULL constraints on non-shared columns, and will set them as NOT NULL in the DB, which effectively makes it impossible to insert any other type into that table. (!!)
* Table Per Type - Every class (including abstract parent classes) gets a table.
	* Most "normalized," least performant.
* Table per Concrete Class - One table for every class that is instantiatable, but not for abstract classes from which concrete classes descend.
	* Can be seen as a mix of TPH and TPT - a compromise that I tend to favor.
* EF vs NH
	* While NH can use any of these changing only the mapping (the queries and data access code is identical for each strategy), for Entity Framework, potentially all code must change depending on strategy.
		* TPCC/TPT - Use CreateObjectSet<T> where T is the specific type you want.
		* TPH - Use CreateObjectSet<T> where T is base type.

Concurrency:

* None
	* OptimisticLock.None(); or leave blank
* Optimistic - When issuing a request, include either changed fields, or all fields in UPDATE statement.
	* OptimisticLock.Dirty(); - Adds WHERE clause including only changed fields
	* OptimisticLock.All(); - Adds WHERE clause including all fields.
	* Requires DynamicUpdate
* Versioned - Specify a version column and NH will check that in the UPDATE statement.
	* OptimisticLock.Version(); - Adds WHERE clause w/ Version field.
* Pessimistic - Use DB capabilities for transactions / row locking.  If conflict, wait for timeout.

Performance Considerations:

* SELECT * by default
* In NH, using transactions can actually speed up requests.
	* NH automatically adds transactions around individual queries, if not in a transaction
* Batched queries in NH (Future<T>)

NHibernate DynamicUpdate:
	
* With: UPDATE [table] SET [ChangedField] = @value WHERE id = @id;
* Without: UPDATE [table] SET [ChangedField] = @value, [NonChangedField] = @value2 WHERE id = @id;

Query Patterns

* Both - LINQ (with varying degrees of support)
* NHibnerate - QueryOver<T, T>;
	* Joining a table does not change its base type, but does give you access to the joined table.
	* Because of this, you can actually use other built queries against your joined table.
	* Astute viewers will notice this query does not take IsDeleted into account when joining tables.
* EntityFramework - ???
	* Issues w/ how joining a table changes the base type of the query.  A query expression built in PredicateBuilder<User> cannot apply to (that I see) the anonymous type generated by a LINQ join.

