type: fix

`EmlParser` now decodes RFC 2047 encoded words in the display names of `From`, `To`, `Cc`, `Bcc` and `Reply-To`, so a non-ASCII name survives a `ToMimeString` round trip; a malformed encoded word is kept verbatim and reported, as in the subject.
