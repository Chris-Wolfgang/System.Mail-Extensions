namespace Wolfgang.Extensions.Mail;


/// <summary>
/// Controls how <see cref="EmlParser"/> handles malformed constructs it
/// encounters while parsing EML/MIME content.
/// </summary>
/// <remarks>
/// By default the parser is lenient: malformed addresses, undecodable
/// transfer encodings, and malformed RFC 2047 encoded words are skipped (or
/// kept verbatim) rather than throwing. Set <see cref="Strict"/> to reject
/// such input instead, or use
/// <see cref="EmlParser.ParseWithDiagnostics(string, EmlParserOptions?)"/> to
/// see what a lenient parse dropped.
/// </remarks>
public sealed class EmlParserOptions
{

    /// <summary>
    /// When <c>true</c>, the parser throws an <see cref="EmlParseException"/> on
    /// the first malformed construct instead of skipping it. When <c>false</c>
    /// (the default), malformed constructs are skipped and parsing continues.
    /// </summary>
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    // ReSharper disable once MemberCanBePrivate.Global
    public bool Strict { get; set; }
}
