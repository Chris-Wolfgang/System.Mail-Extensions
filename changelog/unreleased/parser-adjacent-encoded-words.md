type: fix

`EmlParser` now drops the whitespace between adjacent RFC 2047 encoded words (RFC 2047 section 6.2) and decodes the bytes of adjacent words in the same charset together, so a long non-ASCII subject or display name that was folded into several encoded words no longer gains spaces or replacement characters.
