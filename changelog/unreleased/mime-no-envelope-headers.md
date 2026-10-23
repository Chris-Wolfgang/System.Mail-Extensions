type: fix

`ToMimeString()` no longer writes the SMTP pickup-directory envelope (`X-Sender` and one `X-Receiver` line per recipient), which disclosed Bcc recipients in the generated EML text and added two headers to every parse round trip.
