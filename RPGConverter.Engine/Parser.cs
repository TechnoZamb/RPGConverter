using System.Text.RegularExpressions;

namespace RPGConverter.Engine;

public class Parser
{
    public void ParsePgm(string text)
    {
        var result = new ParseResult();

        foreach (var (lineIndex, line) in text.Split('\n').Index())
        {
            // numero linea parte da base 1
            var lineNumber = lineIndex + 1;

            // riga vuota
            if (string.IsNullOrWhiteSpace(line) || (line.Length > 5 && line[..5].IsWhiteSpace() && line.Trim().Length < 8))
            {
                result.Statements.Add(new EmptyLineNode());
                continue;
            }

            // riga con soli commenti
            if (line.Length < 6 ||
                line[5] == ' ' ||
                (line.Length > 6 && line[6] == '*') ||
                (line.Length > 80 && line[7..80].IsWhiteSpace()))
            {
                var comment = "";

                // commenti nei primi 5 caratteri
                if (line.Length < 6 || !line[..5].IsWhiteSpace())
                    comment += line[..Math.Min(5, line.Length - 1)];

                // commenti nel resto della riga
                if (line.Length > 7)
                    comment += line[7..].TrimEnd();

                result.Statements.Add(new CommentNode { Comment = comment });
                continue;
            }

            // riga di free
            if (line.Length - line.TrimStart().Length > 6)
            {
                // todo
            }
            else
            {
                // lavoro in base alla specification
                switch (line[5])
                {
                    case 'H' or 'h': // Control

                        // questa specifica contiene solamente le keywords subito dopo la lettera H, separate per spazio
                        // le keywords possono essere standalone (ALTSEQ) oppure parametrizzate (DATEDIT(*YMD))

                        var keywordsAndParams = new Regex(@"(\w+)\s*(\(\s*[^)\s]*\s*(?:\)|(?=\w)))?", RegexOptions.Multiline | RegexOptions.IgnoreCase).Matches(line[6..]);
                        foreach (var match in keywordsAndParams.ToList())
                        {
                            var keyword = match.Groups[1].Value.Trim();
                            var isError = false;

                            // la stessa keyword non può apparire due volte
                            if (result.Statements.FirstOrDefault(s =>
                                    s is ControlNode c && string.Compare(c.KeyWord, keyword, StringComparison.OrdinalIgnoreCase) == 0) != null)
                            {
                                result.Errors.Add(new ParsingError(lineNumber, match.Index + 6, match.Length, ParsingErrorType.DuplicateControlKeyword));
                                isError = true;
                            }

                            string? param = null;
                            if (match.Groups[2].Length > 0)
                            {
                                if (match.Groups[2].Value.Contains('(') && !match.Groups[2].Value.Contains(')'))
                                {
                                    result.Errors.Add(new ParsingError(lineNumber, match.Index + 6, match.Length, ParsingErrorType.UnclosedOpenBracket));
                                    isError = true;
                                }
                                param = match.Groups[2].Value.Replace("(", "").Replace(")", "").Trim();
                            }

                            if (!isError)
                                result.Statements.Add(new ControlNode { KeyWord = keyword, Parameter = param });
                        }

                        break;
                }
            }
        }
    }
}

public record ParseResult
{
    public bool Passed { get; internal set; }
    public List<ParsingError> Errors { get; internal set; } = [];
    public StatementNodeCollection Statements { get; internal set; } = [];
}

public class ParsingError(int line, int? column, int span, ParsingErrorType type)
{
    public int Line { get; internal set; } = line;
    public int? Column { get; internal set; } = column;
    public int Span { get; internal set; } = span;
    public ParsingErrorType Type { get; internal set; } = type;
}

public enum ParsingErrorType
{
    UnrecognizedSpecification,
    DuplicateControlKeyword,
    UnclosedOpenBracket
}
