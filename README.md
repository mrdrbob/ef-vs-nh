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
* Professional .NET developer since 2004.
	* Couple years of ColdFusion, which I've been trying to scrub from my memory so please stop bringing it up.
* Currently a developer at Blend Interactive, since Aug '12.
* Long(ish) time NHibnerate user

The Project:

* "Thing" maintenance app
* POCOs - BaseItem, Thing, User, etc.

Mapping:

* Both: Fluent, Data Annotations, Automap.
	* Fluent - Control, separation of concerns, manual work.
	* Annotations - Control, less work, decorates your POCO with DB concerns.
	* Automap - Easy, but defaults to nvarchar(max)
* NHibernate: XML
	* Very similar to Hibernate, bracket tax.
* Entity Framework: XML w/ VS designer.
	* Visual designer, seriously?
* NHibernate requires properties be marked virtual.
* NHibernate uses Hibernate collections
	* Set - Distinct elements, no order - HashSet (ICollection)
	* Bag - Elements, no order - IList
	* Map - IDictionary
	* List - IList - Elements, order
* Guid Identifiers
	* Guid.Comb - Sequential GUIDs to reduce index fragmentation

Inheritance Types:

* Table per Hierarchy - One table per complete hierarchy - uses a "discriminator" column.  X classes in 1 table, which has columns to cover each property of said classes.
	* Most performant (generally).
	* Cannot enforce NOT NULL constraints on properties that are not shared by every class - inserts null into those columns when not the that type.
	* NHibernate will actually allow you to put NOT NULL constraints on non-shared columns, and will set them as NOT NULL in the DB, which effectively makes it impossible to insert any other type into that table. (!!)
* Table Per Type - Every class (including abstract parent classes) gets a table.
	* Most "normalized," least performant.
* Table per Concrete Class - One table for every class that is instantiatable, but not for abstract classes from which concrete classes descend.
	* Can be seen as a mix of TPH and TPT - a compromise that I tend to favor.

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
	* NH automatically adds transactions around queries
* Batched queries (Future)

NHibernate DynamicUpdate:
	
* With: UPDATE [table] SET [ChangedField] = @value WHERE id = @id;
* Without: UPDATE [table] SET [ChangedField] = @value, [NonChangedField] = @value2 WHERE id = @id;

Query Patterns

* Both - LINQ
* NHibnerate - QueryOver<T, T>;

