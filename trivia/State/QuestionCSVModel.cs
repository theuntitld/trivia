namespace trivia.State;

[FileHelpers.DelimitedRecord(",")]
[FileHelpers.IgnoreFirst()]
public class QuestionCSVModel
{
    [FileHelpers.FieldQuoted(FileHelpers.QuoteMode.OptionalForBoth)]
    public string Category { get; set; } = default!;

    [FileHelpers.FieldQuoted(FileHelpers.QuoteMode.OptionalForBoth)]
    public string Question { get; set; } = default!;

    [FileHelpers.FieldQuoted(FileHelpers.QuoteMode.OptionalForBoth)]
    public string A { get; set; } = default!;

    [FileHelpers.FieldQuoted(FileHelpers.QuoteMode.OptionalForBoth)]
    public string B { get; set; } = default!;

    [FileHelpers.FieldQuoted(FileHelpers.QuoteMode.OptionalForBoth)]
    public string C { get; set; } = default!;

    [FileHelpers.FieldQuoted(FileHelpers.QuoteMode.OptionalForBoth)]
    public string D { get; set; } = default!;
    public string Answer { get; set; } = default!;
}
