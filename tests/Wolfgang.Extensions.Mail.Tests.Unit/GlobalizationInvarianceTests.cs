using System.Collections.Generic;
using System.Globalization;
using System.Net.Mail;
using Xunit;
using Assert = Xunit.Assert;

#pragma warning disable CA1707

namespace Wolfgang.Extensions.Mail.Tests.Unit;


/// <summary>
/// Verifies the public surface is culture-invariant. Every operation is run
/// under hostile cultures — Turkish dotted-I, German decimal comma, Chinese
/// collation, Arabic RTL/Hindi digits, Japanese full-width digits — and must
/// produce the same result as en-US.
/// </summary>
/// <remarks>
/// Intentionally culture-sensitive public methods: <b>none</b>. The library
/// case-folds and compares with <c>InvariantCulture</c> / <c>OrdinalIgnoreCase</c>
/// throughout, so every public method is culture-invariant by contract. If a
/// method ever becomes deliberately culture-sensitive, exclude it here and say
/// why.
/// </remarks>
public class GlobalizationInvarianceTests
{

    public static IEnumerable<object[]> HostileCultures => new[]
    {
        new object[] { "en-US" },
        new object[] { "tr-TR" },   // dotted / dotless I
        new object[] { "de-DE" },   // decimal comma
        new object[] { "zh-CN" },   // collation + simplified Chinese
        new object[] { "ar-SA" },   // RTL + Hindi-Arabic digits
        new object[] { "ja-JP" },   // full-width digits
    };



    [Theory]
    [MemberData(nameof(HostileCultures))]
    public void Parse_is_culture_invariant(string culture)
    {
        // Uppercase header names + MIME type + transfer encoding exercise the
        // case-insensitive matching. "PRINTABLE" and the content type contain
        // 'I', so a culture-sensitive lower-case (tr-TR) would break decoding.
        const string eml =
            "From: sender@example.com\r\n" +
            "To: alice@example.com\r\n" +
            "Subject: =?utf-8?Q?caf=C3=A9?=\r\n" +
            "Content-Type: TEXT/PLAIN; charset=utf-8\r\n" +
            "Content-Transfer-Encoding: QUOTED-PRINTABLE\r\n\r\n" +
            "H=C3=A9llo\r\n";

        InCulture(culture, () =>
        {
            using var message = EmlParser.Parse(eml);

            Assert.Equal("sender@example.com", message.From!.Address);
            Assert.Equal("alice@example.com", message.To.Single().Address);
            Assert.Equal("café", message.Subject);
            Assert.Equal("Héllo", message.Body);
        });
    }



    [Theory]
    [MemberData(nameof(HostileCultures))]
    public void InferContentType_is_case_insensitive_regardless_of_culture(string culture)
    {
        // The extensions contain 'I' (dotted/dotless acid test). A
        // culture-sensitive fold under tr-TR would turn ".GIF" into ".gıf" and
        // miss the registry entry.
        InCulture(culture, () =>
        {
            Assert.Equal("image/gif", AttachmentFactory.InferContentType("banner.GIF"));
            Assert.Equal("image/x-icon", AttachmentFactory.InferContentType("favicon.ICO"));
            Assert.Equal("text/html", AttachmentFactory.InferContentType("page.HTML"));
        });
    }



    [Theory]
    [MemberData(nameof(HostileCultures))]
    public void RegisterContentType_matches_across_case_regardless_of_culture(string culture)
    {
        InCulture(culture, () =>
        {
            // Unique per invocation so this can't collide in the process-wide
            // registry with other tests or the other culture rows. The 'I' in
            // "XI" becomes dotless under tr-TR; register uppercase and infer the
            // lowercase form — OrdinalIgnoreCase must match both regardless of
            // culture.
            var ext = $".XI{Guid.NewGuid():N}";
            AttachmentFactory.RegisterContentType(ext, "application/x-culture");

            Assert.Equal("application/x-culture", AttachmentFactory.InferContentType($"file{ext.ToLowerInvariant()}"));
        });
    }



    [Theory]
    [MemberData(nameof(HostileCultures))]
    public void Build_and_ToFormattedString_are_culture_invariant(string culture)
    {
        InCulture(culture, () =>
        {
            using var message = new MailMessageBuilder()
                .From("sender@example.com", "Sender")
                .To("alice@example.com")
                .Subject("Report")
                .PlainTextBody("Body")
                .Build();
            message.CC.Add(new MailAddress("bob@example.com", "Bob"));

            Assert.Equal("Report", message.Subject);
            Assert.Equal("\"Bob\" <bob@example.com>", message.CC.ToFormattedString());
        });
    }



    [Theory]
    [MemberData(nameof(HostileCultures))]
    public void EncodedWord_B_and_Q_decode_the_same_regardless_of_culture(string culture)
    {
        var bEncoded = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("Ünïcödé"));
        var eml =
            "From: a@example.com\r\nTo: b@example.com\r\n" +
            $"Subject: =?utf-8?B?{bEncoded}?= and =?utf-8?Q?caf=C3=A9?=\r\n\r\nBody.\r\n";

        InCulture(culture, () =>
        {
            using var message = EmlParser.Parse(eml);
            Assert.Equal("Ünïcödé and café", message.Subject);
        });
    }



    private static void InCulture
    (
        string name,
        Action action
    )
    {
        var originalCulture = CultureInfo.CurrentCulture;
        var originalUiCulture = CultureInfo.CurrentUICulture;
        try
        {
            var target = CultureInfo.GetCultureInfo(name);
            CultureInfo.CurrentCulture = target;
            CultureInfo.CurrentUICulture = target;
            action();
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
            CultureInfo.CurrentUICulture = originalUiCulture;
        }
    }
}
