
C#
	Exceptions
		- Use "throw new ..." to create a failure, "throw;" to propagate the current failure from a catch block, and "throw new ... (message, ex)" to translate a failure while preserving its cause
		- C# does not have checked exception
		- C# "using" does not have automatic suppression mechanism for exception from Dispose(); an exception from Dispose() can replace the exception already propagating from the body.
	- Resource ownership and lifetimes
		- IDisposable & using
		- Resource ownership vs lifetime
	- Validation
		- input validation vs domain invariant enforcement
		-

| Layer                         | Validation Responsibility                                             | Primary Purpose                                         |
| ----------------------------- | --------------------------------------------------------------------- | ------------------------------------------------------- |
| Frontend                      | Input format, required fields, immediate feedback                     | Help users construct valid requests                     |
| Controller/ API boundary      | Request structure, types, required fields, external input constraints | Reject invalid or untrusted input entering the app      |
| Application service           | Operation-level preconditions and workflow constraints                | Ensure an operation is permitted in its current context |
| Domain object/ domain service | Business invariants and domain rules                                  | Prevent invalid domain states and transitions           |

	- Others
		- readonly keyword