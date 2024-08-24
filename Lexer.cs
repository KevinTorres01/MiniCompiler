using System.Text.RegularExpressions;

public class Token
{
    public TokenType Type;
    public string Value;
    public Location location;

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
        { TokenType.ENDER, "^\\;" },
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
        { TokenType.CARD,"^Card$?(?![a-zA-Z0-9])"},
        { TokenType.EFFECT,"^effect$?(?![a-zA-Z0-9])"},
        { TokenType.NUMBER,"^Number$?(?![a-zA-Z0-9])"},
        { TokenType.BOOL,"^Bool$?(?![a-zA-Z0-9])"},
        { TokenType.STRINGKEYWORD,"^String$?(?![a-zA-Z0-9])"},
        { TokenType.IFKEYWORD,"^if$?(?![a-zA-Z0-9])"},
        { TokenType.ELSEKEYWORD,"^else$?(?![a-zA-Z0-9])"},
        { TokenType.FORKEYWORD,"^for$?(?![a-zA-Z0-9])"},
        { TokenType.WHILEKEYEORD,"^while$?(?![a-zA-Z0-9])"},
        { TokenType.PRINTKEYWORD,"^print$?(?![a-zA-Z0-9])"},
        { TokenType.INKEYWORD,"^in$?(?![a-zA-Z0-9])"},
        { TokenType.KEYWORD, "^in$?(?![a-zA-Z0-9])" },
        { TokenType.IDENTIFIER, "^([a-zA-Z_]\\w*)" },
        { TokenType.POTENCIATION, "^\\^"},
        { TokenType.SPLITER, "^\\:"},
        { TokenType.SPACES,"^\\s+"},
        { TokenType.DOT,"^\\."}

    };

}
public enum TokenType
{
    PLUS, MINUS, MUL, DIV, AND, OR, LESS,
    GREATER, STRING, NUMS, LEFTPARENT, RIGHTPARENT,
    EQUALS, LEFTBRAKETS, RIGHTBRAKETS, LEFTBRACES, RIGHTBRACES,
    IDENTIFIER, SPACES, COMMA, ASIGNMENT, CONCATWHITHSPACE,
    CONCATWHITHOUTSPACE, GREATEREQUALS, LESSEQUALS, IFKEYWORD,
    FORKEYWORD, WHILEKEYEORD, ELSEKEYWORD, PRINTKEYWORD, KEYWORD,
    INCREMENT, DECREMENT, POTENCIATION, BOOLEANS, SPLITER, ENDER,
    DOT, CARD, NAME, POWER, TYPE, FACTION, RANGE, OnActivation, EFFECT,
    INKEYWORD, BOOL, NUMBER, STRINGKEYWORD, PARAMS
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
                    Token Matched;
                    if (token.Key == TokenType.STRING)
                    {
                        Matched = new Token(token.Key, match.Groups[0].Value.Substring(1, match.Groups[0].Value.Length - 2), new Location(row + 1, index + 1));
                    }
                    else
                    {
                        Matched = new Token(token.Key, match.Groups[0].Value, new Location(row + 1, index + 1));

                    }
                    Tokens.Add(Matched);
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
        string s = GetFileContent(@"/home/kevin/Escritorio/MiniCompiler/Grammar.txt");
        List<Token> tokens = new List<Token>();
        Lexer a = new Lexer();
        tokens = a.Tokenizer(s);
        a.FixErrors();
        Parser b = new Parser(tokens);
        var x = new Enviroment();
        List<IProgramNode> statements = b.Program(x);
        foreach (var item in statements)
        {
            item.Create();
        }
        foreach (var item in b.context.cards)
        {
            System.Console.WriteLine(item);
            b.context.effects[item.Key].action.Invoke();
        }
    }
}