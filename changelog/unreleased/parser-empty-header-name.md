type: fix

`EmlParser` no longer throws `ArgumentException` on a header line whose field name is only whitespace (for example a line starting with a bare carriage return); the header is skipped and reported through `ParseWithDiagnostics`, and `Strict` mode throws `EmlParseException`.
