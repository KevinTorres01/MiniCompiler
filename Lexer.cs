using System.Text.RegularExpressions;

public class Token
{
    public TokenType Type;
    public string Value;
    Location location;

    public Token(TokenType type, string value, Location loc)
    {
        Type = type;
        Value = value;
        location = loc;
    }

    public static Dictionary<TokenType, string> pairs = new Dictionary<TokenType, string>()
    {
        { TokenType.NUMS, @"^\d+(\.\d+)?\b" },
        { TokenType.COMMA, "^\\," },
        { TokenType.BOOLEANS, "^true|^false" },
        { TokenType.INCREMENT,"^\\+\\+"},
        { TokenType.DECREMENT,"^\\--"},
        { TokenType.PLUS, "^\\+" },
        { TokenType.MINUS, "^-" },
        { TokenType.MUL, "^\\*" },
        { TokenType.DIV, "^/" },
        { TokenType.AND, "^&&" },
        { TokenType.GREATEREQUALS,"^>="},
        { TokenType.LESSEQUALS, "^<=" },
        { TokenType.EQUALS, "^==" },
        { TokenType.ASIGNMENT, "^=" },
        { TokenType.CONCATWHITHSPACE,"^@@"},
        { TokenType.CONCATWHITHOUTSPACE,"^@"},
        { TokenType.OR, "^\\|\\|" },
        { TokenType.LESS, "^<" },
        { TokenType.GREATER, "^>"},
        { TokenType.STRING, "^\"[^\"]*\"" },
        { TokenType.LEFTPARENT, "^\\(" },
        { TokenType.RIGHTPARENT, "^\\)" },
        { TokenType.LEFTBRAKETS, "^\\[" },
        { TokenType.RIGHTBRAKETS, "^\\]" },
        { TokenType.LEFTBRACES, "^\\{" },
        { TokenType.RIGHTBRACES, "^\\}" },
        { TokenType.KEYWORD, "^effect$?(?![a-zA-Z0-9])|^card$?(?![a-zA-Z0-9])|^Name$?(?![a-zA-Z0-9])|^while$?(?![a-zA-Z0-9])|^for$?(?![a-zA-Z0-9])|^in$?(?![a-zA-Z0-9])|^String$?(?![a-zA-Z0-9])|^Params$?(?![a-zA-Z0-9])|^Number$?(?![a-zA-Z0-9])|^Bool$?(?![a-zA-Z0-9])|^Action$?|^Type$?(?![a-zA-Z0-9])|^Faction$?(?![a-zA-Z0-9])|^Power$?(?![a-zA-Z0-9])|^Range$?(?![a-zA-Z0-9])|^OnActivation$?(?![a-zA-Z0-9])|^Effect$?(?![a-zA-Z0-9])|^Selector$?(?![a-zA-Z0-9])|^Source$?(?![a-zA-Z0-9])|^Single$?(?![a-zA-Z0-9])|^Predicate$?(?![a-zA-Z0-9])|^PostAction$(?![a-zA-Z0-9])" },
        { TokenType.IDENTIFIER, "^([a-zA-Z_]\\w*)" },
        { TokenType.POTENCIATION, "^\\^"},
        { TokenType.SPLITER, "^\\:"},
        { TokenType.SPACES,"^\\s+"},

    };

}
public enum TokenType
{
    PLUS, MINUS, MUL, DIV, AND, OR, LESS,
    GREATER, STRING, NUMS, LEFTPARENT, RIGHTPARENT,
    EQUALS, LEFTBRAKETS, RIGHTBRAKETS, LEFTBRACES, RIGHTBRACES,
    IDENTIFIER, SPACES, COMMA, ASIGNMENT, CONCATWHITHSPACE,
    CONCATWHITHOUTSPACE, GREATEREQUALS, LESSEQUALS, KEYWORD,
    INCREMENT, DECREMENT, POTENCIATION, BOOLEANS, SPLITER

}
public class Location
{
    public int Row;
    public int Col;
    public Location(int row, int col)
    {
        Row = row;
        Col = col;
    }
}
public class Lexer
{
    List<Token> Tokens = new List<Token>();
    Dictionary<string, List<Location>> Errors = new Dictionary<string, List<Location>>();
    public List<Token> GetTokens()
    {
        return Tokens;
    }
    void FixErrors()
    {
        int actualRow = 0;
        int actualCol = 0;
        List<int> Removes = new List<int>();
        int index = -1;
        foreach (var error in Errors.Keys)
        {
            foreach (var k in Errors[error])
            {
                index++;
                if (actualRow == 0)
                {
                    actualRow = k.Row;
                    actualCol = k.Col;
                }
                else if (actualRow - k.Row == 0)
                {
                    if (actualCol - k.Col == -1)
                    {
                        actualCol = k.Col;
                        Removes.Add(index);
                    }
                }
            }
            foreach (var item in Removes)
            {
                Errors[error].RemoveAt(item);
            }
            Removes = new List<int>();
        }
    }

    private static string GetFileContent(string root)
    {
        StreamReader streamReader = new StreamReader(root);
        string FileContent = streamReader.ReadToEnd();
        streamReader.Close();
        return FileContent;
    }
    public List<Token> Tokenizer(string s)
    {
        string[] LinesOfSentences = s.Split('\n');
        for (int i = 0; i < LinesOfSentences.Length; i++)
        {
            Tokenizer(LinesOfSentences[i], i);
        }
        return Tokens;
    }
    private void Tokenizer(string s, int row)
    {
        int index = 0;
        while (index < s.Length)
        {
            if (s[index] == ' ')
            {
                var regex = new Regex(Token.pairs[TokenType.SPACES]);
                var match = regex.Match(s.Substring(index));
                index += match.Length;
                continue;
            }
            bool Match = false;
            foreach (var token in Token.pairs)
            {
                var regex = new Regex(token.Value);
                var match = regex.Match(s.Substring(index));

                if (match.Success)
                {
                    if (token.Key == TokenType.SPACES)
                    {
                        index += match.Length;
                        Match = match.Success;
                        break;
                    }
                    Token token1 = new Token(token.Key, match.Groups[0].Value, new Location(row + 1, index + 1));
                    Tokens.Add(token1);
                    Match = match.Success;
                    index += match.Length;
                    break;
                }
            }
            if (!Match)
            {
                if (Errors.ContainsKey("Invalid Token"))
                {
                    Errors["Invalid Token"].Add(new Location(row + 1, index + 1));
                    index++;
                }
                else
                {
                    List<Location> values = new List<Location>();
                    values.Add(new Location(row + 1, index + 1));
                    Errors.Add("Invalid Token", values);
                    index++;
                }
            }
        }
    }
    static void Main()
    {
        string s = GetFileContent(@"/home/kevin/Documentos/TXT/Documento sin título 1");
        List<Token> tokens = new List<Token>();
        Lexer a = new Lexer();
        tokens = a.Tokenizer(s);
        a.FixErrors();
        foreach (var item in tokens)
        {
            System.Console.WriteLine(item.Type);
        }
    }
}
