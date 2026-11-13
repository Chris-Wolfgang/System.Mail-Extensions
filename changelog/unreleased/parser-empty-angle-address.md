type: fix

Lenient `EmlParser.Parse` no longer throws on an address header whose angle brackets are empty (for example `Cc: <>`); the address is reported as malformed instead.
