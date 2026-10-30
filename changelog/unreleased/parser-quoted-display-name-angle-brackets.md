type: fix

`EmlParser` no longer drops an address whose quoted display name contains `<` (for example `"Team <ops>" <ops@example.com>`); a quoted display name is now read as a whole before the unquoted form is tried.
