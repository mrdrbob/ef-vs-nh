Entity Framework Vs NHibernate
============================

An NHibernate guy's journey into the Entity Framework World.

Random Notes
===========

To be organized later:

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
		* OptimisticLock.Dirty();
		* OptimisticLock.All();
		* Requires DynamicUpdate
	* Versioned - Specify a version column and NH will check that in the UPDATE statement.
		* OptimisticLock.Version();
	* Pessimistic - Use DB capabilities for transactions / row locking.  If conflict, wait for timeout.

NHibernate DynamicUpdate:
	
	* With: UPDATE [table] SET [ChangedField] = @value WHERE id = @id;
	* Without: UPDATE [table] SET [ChangedField] = @value, [NonChangedField] = @value2 WHERE id = @id;

Query Patterns

	* NHibnerate - QueryOver<T, T>;

