#pragma warning disable S3878 // Example deliberately shows array creation to demonstrate IEnumerable overload
using System.Net.Mail;
using System.Text;
using Wolfgang.Extensions.Mail;

using var mailMessage = new MailMessage();


//  --------- Add Addresses -------

// Adding 1 or more as comma separated parameters
mailMessage.To.AddRange("john.doe@example.com", "jane.doe@example.com");

// Adding 1 or more from an array or any IEnumerable<string>
mailMessage.To.AddRange(new[] { "john.smith@example.com", "jane.smith@example.com" });

// Adding 1 or more using MailAddress objects as comma separated parameters
mailMessage.To.AddRange(new MailAddress("john.jones@example.com"), new MailAddress("jane.jones@example.com"));

// Adding 1 or more using MailAddress objects from an array or any IEnumerable<MailAddress>
var address = new[]
{
	new MailAddress("mike.jones@example.com"),
	new MailAddress("kim.jones@example.com")
};
mailMessage.To.AddRange(address);



// Print addresses
foreach (var entry in mailMessage.To)
{
	Console.WriteLine(entry);
}


//  --------- Add Attachments -------



// Adding attachments using MemoryStream-backed constructors (params overload)
mailMessage.Attachments.AddRange
(
	CreateAttachment("order-summary.txt", "text/plain", "Order summary content."),
	CreateAttachment("packing-slip.txt", "text/plain", "Packing slip content.")
);

// Adding attachments using MemoryStream-backed constructors (IEnumerable overload)
var inlineAttachments = new List<Attachment>
{
	CreateAttachment("invoice.pdf", "application/pdf", "Invoice document content."),
	CreateAttachment("receipt.pdf", "application/pdf", "Receipt document content.")
};
mailMessage.Attachments.AddRange(inlineAttachments);


foreach (var attachment in mailMessage.Attachments)
{
	Console.WriteLine($"Attachment added: {attachment.Name}");
}

static Attachment CreateAttachment(string fileName, string mediaType, string content)
{
	var buffer = Encoding.UTF8.GetBytes(content);
	var stream = new MemoryStream(buffer);
	return new Attachment(stream, fileName, mediaType);
}
