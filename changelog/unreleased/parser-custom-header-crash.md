type: fix

`EmlParser` no longer throws on a custom header with an empty value (it is skipped, as `MailMessage.Headers` cannot hold one) or with a non-ASCII name (skipped and reported through `ParseWithDiagnostics`; `Strict` mode throws `EmlParseException`).
