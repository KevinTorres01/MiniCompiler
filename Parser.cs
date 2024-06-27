class Parser
{
    int currentIndex = 0;
    Token CurrentToken;
    List<Token> tokens = new List<Token>();
    public Parser(List<Token> Tokens)
    {
        tokens = Tokens;
    }
    public bool Match(params TokenType[] Types)
    {
        foreach (var item in Types)
        {
            if (item == CurrentToken.Type)
            {
                return true;
            }
        }
        return false;
    }
    public object Literal()
    {
        if (tokens[currentIndex])
        {

        }
    }




}