type: internal

`InlineHtmlBuilder` infers image content types from `AttachmentFactory` (so `RegisterContentType` registrations apply to inline images too), RFC 2047 Q-encoded headers decode with one buffer instead of several allocations per character, and `AttachmentFactory.TryGetRegisteredContentType` is annotated `[NotNullWhen(true)]` on net8.0+/netstandard2.1.
