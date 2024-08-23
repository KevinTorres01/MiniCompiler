
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

public class Parser
{
    int currentIndex = 0;
    Token CurrentToken { get => tokens[currentIndex]; }
    List<Token> tokens = new List<Token>();
    public Context context = new Context();
    public Parser(List<Token> tokens)
    {
        this.tokens = tokens;
    }
    private Token Previus()
    {
        return tokens[currentIndex - 1];
    }
    private bool IsAtEnd()
    {
        return currentIndex == tokens.Count - 1;
    }
    private void Advance()
    {
        if (!IsAtEnd())
            currentIndex++;
    }
    private Token Consume(TokenType type)
    {
        if (!IsAtEnd() && type == tokens[currentIndex].Type)
        {
            Advance();
            return Previus();
        }
        if (IsAtEnd() && type == tokens[currentIndex].Type)
        {
            return CurrentToken;
        }
        throw new Exception();
    }
    private IExpression ActionExpression(Enviroment Parent)
    {
        Consume(TokenType.LEFTPARENT);
        List<Token> tokens = new List<Token>();
        if (!(CurrentToken.Type == TokenType.RIGHTPARENT))
        {
            tokens.Add(Consume(TokenType.IDENTIFIER));
            while (Match(TokenType.COMMA))
            {
                tokens.Add(Consume(TokenType.IDENTIFIER));
            }
        }
        Consume(TokenType.RIGHTPARENT);
        Consume(TokenType.ASIGNMENT);
        Consume(TokenType.GREATER);
        List<IStatements> statements = new List<IStatements>();
        Consume(TokenType.LEFTBRACES);
        statements = List(Parent);
        return new ActionExpression(tokens, statements, Parent);
    }
    bool Match(params TokenType[] Types)
    {
        foreach (var item in Types)
        {
            if (item == CurrentToken.Type)
            {
                Advance();
                return true;
            }
        }
        return false;
    }

    public List<IProgramNode> Program(Enviroment Parent)
    {
        List<IProgramNode> programNodes = new List<IProgramNode>();
        while (!IsAtEnd())
        {
            if (Match(TokenType.CARD))
            {
                programNodes.Add(Card(Parent));
            }
            else if (Match(TokenType.EFFECT))
            {
                programNodes.Add(Effect(Parent));
            }
            else
            {
                throw new Exception();
            }
        }
        return programNodes;
    }
    public IProgramNode Effect(Enviroment Parent)
    {
        Consume(TokenType.LEFTBRACES);
        IExpression name = null;
        List<Params> param = new List<Params>();
        IExpression action = null;
        while (!Match(TokenType.RIGHTBRACES))
        {
            if (Match(TokenType.IDENTIFIER))
            {
                if (Previus().Value == "Params")
                {
                    param = Param();
                    Consume(TokenType.COMMA);
                }
                else if (Previus().Value == "Name")
                {
                    Consume(TokenType.SPLITER);
                    name = Expression(Parent);
                    Consume(TokenType.COMMA);
                }
                else if (Previus().Value == "Action")
                {

                    Consume(TokenType.SPLITER);
                    action = Expression(Parent);
                    Consume(TokenType.COMMA);
                }
                else
                {
                    throw new Exception();
                }
            }
        }
        return new EffectDeclaration(name, param, action, context);
    }
    public List<Params> Param()
    {
        List<Params> param = new List<Params>();
        Consume(TokenType.LEFTBRACES);
        while (!Match(TokenType.RIGHTBRACES) && Match(TokenType.IDENTIFIER))
        {
            string name = Previus().Value;
            Consume(TokenType.SPLITER);
            if (Match(TokenType.NUMBER, TokenType.BOOL, TokenType.STRINGKEYWORD))
            {
                param.Add(new Params(name, Previus().Type));
                Consume(TokenType.COMMA);
            }
            else
            {
                throw new Exception("");
            }
        }
        return param;
    }
    public IProgramNode Card(Enviroment Parent)
    {
        Consume(TokenType.LEFTBRACES);
        IExpression name = null;
        IExpression power = null;
        IExpression faction = null;
        IExpression type = null;
        IExpression range = null;
        while (!Match(TokenType.RIGHTBRACES))
        {
            if (Match(TokenType.IDENTIFIER))
            {
                if (Previus().Value == "Name")
                {
                    Consume(TokenType.SPLITER);
                    name = Expression(Parent);
                    Consume(TokenType.COMMA);
                }
                else if (Previus().Value == "Power")
                {

                    Consume(TokenType.SPLITER);
                    power = Expression(Parent);
                    Consume(TokenType.COMMA);
                }
                else if (Previus().Value == "Type")
                {

                    Consume(TokenType.SPLITER);
                    type = Expression(Parent);
                    Consume(TokenType.COMMA);
                }
                else if (Previus().Value == "Faction")
                {

                    Consume(TokenType.SPLITER);
                    faction = Expression(Parent);
                    Consume(TokenType.COMMA);
                }
                else if (Previus().Value == "Range")
                {
                    Consume(TokenType.SPLITER);
                    range = Expression(Parent);
                    Consume(TokenType.COMMA);
                }
            }
            else if (Match(TokenType.OnActivation))
            {

            }
            else
            {
                throw new Exception("");
            }
        }
        return new CardDeclaration(name, power, type, range, faction, context);

    }
    public IStatements Statement(Enviroment Parent)
    {
        if (Match(TokenType.IFKEYWORD))
        {
            return IFStatement(Parent);
        }
        if (Match(TokenType.WHILEKEYEORD))
        {
            return WhileStatement(Parent);
        }
        if (Match(TokenType.FORKEYWORD))
        {
            return ForStatement(Parent);
        }
        if (Match(TokenType.PRINTKEYWORD))
        {
            return PrintStatement(Parent);
        }
        return ExpressionStatement(Parent);
    }

    private IStatements PrintStatement(Enviroment Parent)
    {
        IExpression exp = Assignment(Parent);
        Consume(TokenType.ENDER);
        return new PrintStmt(exp);
    }

    private IStatements IFStatement(Enviroment Parent)
    {
        Consume(TokenType.LEFTPARENT);
        IExpression condition = Assignment(Parent);
        Consume(TokenType.RIGHTPARENT);
        Consume(TokenType.LEFTBRACES);
        Enviroment enviroment = new Enviroment(Parent);
        List<IStatements> statement = List(enviroment);

        IfStmt ifStmt = new IfStmt(condition)
        {
            statements = statement,
            ElseStatements = []
        };
        if (Match(TokenType.ELSEKEYWORD))
        {
            Consume(TokenType.LEFTBRACES);
            ifStmt.ElseStatements = List(Parent);
            
        }
        return ifStmt;
    }
    private IStatements ForStatement(Enviroment parent)
    {
        Consume(TokenType.IDENTIFIER);
        var x = Previus();
        Consume(TokenType.INKEYWORD);
        var exp = Assignment(parent);
        Consume(TokenType.LEFTBRACES);
        Enviroment enviroment = new Enviroment(parent);
        List<IStatements> statement = List(enviroment);
        return new ForStms(x.Value, exp, statement, enviroment);
    }
    private IStatements WhileStatement(Enviroment Parent)
    {
        Consume(TokenType.LEFTPARENT);
        IExpression condition = Assignment(Parent);
        Consume(TokenType.RIGHTPARENT);
        Consume(TokenType.LEFTBRACES);
        Enviroment enviroment = new Enviroment(Parent);
        List<IStatements> statement = List(enviroment);


        WhileStmt whileStmt = new WhileStmt(condition)
        {
            statements = statement
        };
        return whileStmt;
    }
    private IStatements ExpressionStatement(Enviroment Parent)
    {
        IExpression expression = Assignment(Parent);
        Consume(TokenType.ENDER);
        return new ExpStms(expression);
    }
    List<IStatements> Parse(Enviroment Parent)
    {
        List<IStatements> statements = [];
        while (!IsAtEnd())
        {
            statements.Add(Statement(Parent));
        }
        return statements;
    }
    public List<IStatements> List(Enviroment Parent)
    {
        List<IStatements> statements = [];
        while (!Match(TokenType.RIGHTBRACES) && !IsAtEnd())
        {
            statements.Add(Statement(Parent));
        }
        return statements;
    }
    public List<IExpression> ListOfExp(Enviroment Parent)
    {
        List<IExpression> left = [];
        left.Add(Assignment(Parent));
        while (Match(TokenType.COMMA))
        {
            left.Add(Assignment(Parent));
        }
        return left;
    }
    public IExpression Expression(Enviroment Parent)
    {
        return Assignment(Parent);
    }
    public IExpression Assignment(Enviroment Parent)
    {
        IExpression left = LogicOperations(Parent);
        while (Match(TokenType.ASIGNMENT))
        {
            Token Operator = Previus();
            left = new BinaryExpression(left, Assignment(Parent), Operator);
        }
        return left;
    }
    public IExpression LogicOperations(Enviroment Parent)
    {
        IExpression left = Equality(Parent);
        while (Match(TokenType.AND, TokenType.OR))
        {
            Token Operand = new Token(Previus().Type, Previus().Value, Previus().location);
            left = new BinaryExpression(left, Equality(Parent), Operand);
        }
        return left;
    }
    IExpression Equality(Enviroment Parent)
    {
        IExpression left = StringOperation(Parent);
        if (Match(TokenType.EQUALS))
        {
            var x = Previus();
            return new BinaryExpression(left, StringOperation(Parent), x);
        }
        return left;
    }
    IExpression StringOperation(Enviroment Parent)
    {
        IExpression left = Comparison(Parent);
        while (Match(TokenType.CONCATWHITHSPACE, TokenType.CONCATWHITHOUTSPACE))
        {
            Token operand = new Token(Previus().Type, Previus().Value, Previus().location);
            left = new BinaryExpression(left, Comparison(Parent), operand);
        }
        return left;
    }
    IExpression Comparison(Enviroment Parent)
    {
        IExpression left = Term(Parent);
        if (Match(TokenType.LESS, TokenType.LESSEQUALS, TokenType.GREATER, TokenType.GREATEREQUALS))
        {
            var x = Previus();
            return new BinaryExpression(left, Term(Parent), x);
        }
        return left;
    }
    IExpression Term(Enviroment Parent)
    {
        IExpression left = Factor(Parent);

        while (Match(TokenType.PLUS, TokenType.MINUS))
        {
            Token Operator = Previus();
            IExpression right = Factor(Parent);
            left = new BinaryExpression(left, right, Operator);
        }
        return left;
    }
    IExpression Factor(Enviroment Parent)
    {
        IExpression left = Power(Parent);
        while (Match(TokenType.MUL, TokenType.DIV))
        {
            Token Operator = Previus();
            IExpression right = Power(Parent);
            left = new BinaryExpression(left, right, Operator);
        }
        return left;
    }
    IExpression Power(Enviroment Parent)
    {
        IExpression left = Unary(Parent);
        while (Match(TokenType.POTENCIATION))
        {
            Token Operator = Previus();
            IExpression rigth = Power(Parent);
            left = new BinaryExpression(left, rigth, Operator);
        }
        return left;
    }
    IExpression Unary(Enviroment Parent)
    {
        if (Match(TokenType.MINUS))
        {
            var x = Previus();
            return new UnaryExpression(Literal(Parent), x);
        }
        return DotChainingPatern(Parent);
    }
    IExpression DotChainingPatern(Enviroment parent)
    {
        IExpression left = Literal(parent);
        while (Match(TokenType.DOT, TokenType.LEFTBRAKETS, TokenType.DECREMENT, TokenType.INCREMENT))
        {
            if (Previus().Type == TokenType.DOT)
            {
                if (Match(TokenType.IDENTIFIER))
                {
                    var id = Previus().Value;
                    if (Match(TokenType.LEFTPARENT))
                    {
                        if (Match(TokenType.RIGHTPARENT))
                        {
                            left = new FunctionCall(left, id, new List<IExpression>());
                        }
                        else
                        {
                            var args = ListOfExp(parent);

                            Consume(TokenType.RIGHTPARENT);
                            left = new FunctionCall(left, id, args);

                        }
                    }
                    else
                        left = new GetProperties(left, id, new List<IExpression>());
                }
                else
                    throw new Exception();
            }
            else if (Previus().Type == TokenType.INCREMENT || Previus().Type == TokenType.DECREMENT)
            {
                left = new UnaryExpression(left, Previus());
            }
            else
            {
                var asg = Assignment(parent);
                var x = new GetProperties(left, "Indexer", new List<IExpression>() { asg });
                Consume(TokenType.RIGHTBRAKETS);
                left = x;
            }
        }
        return left;
    }
    IExpression Literal(Enviroment Parent)
    {
        if (Match(TokenType.NUMS, TokenType.STRING, TokenType.BOOLEANS))
        {
            // if (!IsAtEnd())
            // {
            return (Previus().Type == TokenType.NUMS) ? new Atom(double.Parse(Previus().Value)) : Previus().Type == TokenType.STRING ? new Atom(Previus().Value) : new Atom(bool.Parse(Previus().Value));

            // }
            // else
            // {
            //     return (CurrentToken.Type == TokenType.NUMS) ? new Atom(double.Parse(CurrentToken.Value)) : CurrentToken.Type == TokenType.STRING ? new Atom(CurrentToken.Value) : new Atom(bool.Parse(CurrentToken.Value));
            // }
        }
        if (Match(TokenType.IDENTIFIER))
        {
            return new Variable(Previus().Value, Parent);
        }
        if (CurrentToken.Type == TokenType.LEFTPARENT)
        {
            Stack<Token> parentesis = new Stack<Token>();
            parentesis.Push(CurrentToken);
            for (int i = currentIndex + 1; i < tokens.Count; i++)
            {
                if (tokens[i].Type == TokenType.LEFTPARENT)
                {
                    parentesis.Push(tokens[i]);
                }
                else if (tokens[i].Type == TokenType.RIGHTPARENT)
                {
                    if (parentesis.Count == 0)
                    {
                        throw new Exception("");
                    }
                    else
                    {
                        parentesis.Pop();
                    }
                }
                if (parentesis.Count == 0 && tokens[i].Type == TokenType.ASIGNMENT)
                {
                    if (i == tokens.Count - 1)
                    {
                        throw new Exception();
                    }
                    if (tokens[i + 1].Type == TokenType.GREATER)
                    {
                        return ActionExpression(Parent);
                    }
                }
            }
            Consume(TokenType.LEFTPARENT);
            IExpression expression = Expression(Parent);
            Consume(TokenType.RIGHTPARENT);
            return expression;
        }
        if (Match(TokenType.LEFTBRAKETS))
        {
            if (Match(TokenType.RIGHTBRAKETS))
            {
                return new ListExpression(new List<IExpression>());
            }
            var list = new ListExpression(ListOfExp(Parent));
            Consume(TokenType.RIGHTBRAKETS);
            return list;
        }
        throw new Exception();
    }

}