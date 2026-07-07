using System;
using System.IO;
using System.Net.Mail;
using System.Text;
using Wolfgang.Extensions.Mail;
using Wolfgang.Extensions.Mail.Validation;

// Native-AOT smoke: exercise the trim/AOT-safe public surface and confirm it
// runs without MissingMethodException / NotSupportedException once published
// with PublishAot. ToMimeString is intentionally excluded — it is annotated
// [RequiresUnreferencedCode]/[RequiresDynamicCode] and is not AOT-safe.

const string eml =
    "From: sender@example.com\r\n" +
    "To: alice@example.com, bob@example.com\r\n" +
    "Subject: =?utf-8?Q?caf=C3=A9?=\r\n" +
    "Content-Type: text/plain; charset=utf-8\r\n\r\n" +
    "Hello, AOT.\r\n";

// EmlParser: plain, options (strict off), and diagnostics overloads.
using var parsed = EmlParser.Parse(eml);
using var parsedOpts = EmlParser.Parse(eml, new EmlParserOptions { Strict = false });
ParseResult diag = EmlParser.ParseWithDiagnostics(eml);
using var diagMessage = diag.Message;   // ParseResult.Message is an IDisposable MailMessage
Require(parsed.To.Count == 2, "parser: recipient count");
Require(string.Equals(parsed.Subject, "café", StringComparison.Ordinal), "parser: encoded-word subject");
Require(!diag.HasIssues, "parser: clean diagnostics");

// MailMessageBuilder.
using var built = new MailMessageBuilder()
    .From("sender@example.com", "Sender")
    .To("recipient@example.com")
    .Cc("copy@example.com")
    .Subject("Built")
    .PlainTextBody("Body")
    .Attach(new byte[] { 1, 2, 3 }, "data.bin")
    .Build();
Require(built.Attachments.Count == 1, "builder: attachment");

// AttachmentFactory + content-type registry.
AttachmentFactory.RegisterContentType(".aot", "application/x-aot");
Require(string.Equals(AttachmentFactory.InferContentType("f.aot"), "application/x-aot", StringComparison.Ordinal), "factory: registry");
Require(AttachmentFactory.TryGetRegisteredContentType(".aot", out _), "factory: try-get");
using var fromBytes = AttachmentFactory.FromBytes(new byte[] { 4, 5 }, "a.bin");
using var fromB64 = AttachmentFactory.FromBase64(Convert.ToBase64String(new byte[] { 6, 7 }), "b.bin");
using var sourceStream = new MemoryStream(new byte[] { 8 });
using var fromStream = AttachmentFactory.FromStream(sourceStream, "c.bin");

// InlineHtmlBuilder.
using var view = new InlineHtmlBuilder()
    .Html("<p><img src='cid:{0}' /></p>")
    .EmbedImage(Encoding.ASCII.GetBytes("img"), "pixel.png", "image/png")
    .Build();
Require(view.LinkedResources.Count == 1, "inline-html: linked resource");

// MailAddress.TryParse extension.
Require(MailAddress.TryParse("user@example.com", out _), "address: try-parse");

// Collection extensions.
using var msg = new MailMessage("from@example.com", "to@example.com");
msg.To.AddRange("x@example.com", "y@example.com");
Require(msg.To.Count == 3, "collection: address AddRange");
Require(msg.To.ToFormattedString().Length > 0, "collection: ToFormattedString");
msg.Attachments.AddRange(AttachmentFactory.FromBytes(new byte[16], "d.bin"));
_ = msg.Attachments.TotalSize();
_ = msg.Attachments.ExceedsLimit(1);

// Validate + Clone.
ValidationResult result = built.Validate();
Require(result.IsValid, "validate");
using var clone = built.Clone();
Require(clone.Attachments.Count == 1, "clone");

Console.WriteLine("AOT smoke OK");
return 0;

static void Require(bool condition, string what)
{
    if (!condition)
    {
        Console.Error.WriteLine($"AOT smoke FAILED: {what}");
        Environment.Exit(1);
    }
}
