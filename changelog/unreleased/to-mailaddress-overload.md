type: feature

`MailMessageBuilder.To(MailAddress)` adds a recipient with its display name. `To(string address, string displayName)` is now `[Obsolete]`: a call with exactly two strings binds to it and adds one recipient, not two, so the compiler now warns at that call shape. A bare `To(null)` literal is now ambiguous (cast it). The README and XML examples no longer show the two-string call.
