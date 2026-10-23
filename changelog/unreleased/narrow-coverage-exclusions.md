type: internal

The MIME serialization path (`ToMimeString`) is measured by the coverage gate again; only the unreachable reflection-lookup guard, the .NET Framework-only writer constructor (not coverage-instrumented) and the never-called read/seek members of the internal stream wrapper stay excluded.
