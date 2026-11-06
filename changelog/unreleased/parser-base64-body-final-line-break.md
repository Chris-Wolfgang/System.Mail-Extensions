type: fix

`EmlParser` no longer removes a trailing line break from a base64-encoded single-part body; it is part of the content (only quoted-printable and 7bit/8bit bodies end in a writer-added line break).
