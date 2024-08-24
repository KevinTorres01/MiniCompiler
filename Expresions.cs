using System.Net.Http.Headers;

public interface IExpression : IAstNode
{
    object Evaluate();
}
public class BinaryExpression : IExpression
{
    IExpression left;
    IExpression right;
    Token Operator;
    public BinaryExpression(IExpression Left, IExpression Right, Token Operator)
    {
        left = Left;
        right = Right;
        this.Operator = Operator;

    }

    public object Evaluate()
    {
        switch (Operator.Type)
        {
            case TokenType.MUL:
                return (double)left.Evaluate() * (double)right.Evaluate();
            case TokenType.DIV:
                return (double)left.Evaluate() / (double)right.Evaluate();
            case TokenType.PLUS:
                return (double)left.Evaluate() + (double)right.Evaluate();
            case TokenType.MINUS:
                return (double)left.Evaluate() - (double)right.Evaluate();
            case TokenType.OR:
                return (bool)left.Evaluate() || (bool)right.Evaluate();
            case TokenType.AND:
                return (bool)left.Evaluate() && (bool)right.Evaluate();
            case TokenType.GREATEREQUALS:
                return (double)left.Evaluate() >= (double)right.Evaluate();
            case TokenType.GREATER:
                return Convert.ToDouble(left.Evaluate()) > Convert.ToDouble(right.Evaluate());
            case TokenType.LESS:
                return Convert.ToDouble(left.Evaluate()) < Convert.ToDouble(right.Evaluate());
            case TokenType.LESSEQUALS:
                return (double)left.Evaluate() <= (double)right.Evaluate();
            case TokenType.EQUALS:
                return (double)left.Evaluate() == (double)right.Evaluate();
            case TokenType.CONCATWHITHOUTSPACE:
                return (string)(left.Evaluate()) + (string)(right.Evaluate());
            case TokenType.CONCATWHITHSPACE:
                return (string)left.Evaluate() + " " + (string)right.Evaluate();
            case TokenType.POTENCIATION:
                return Math.Pow((double)left.Evaluate(), (double)right.Evaluate());
            case TokenType.ASIGNMENT:
                if (left is Variable v)
                {
                    var x = new VariableAssignation(v, right);
                    return x.Evaluate();
                }
                else if (left is GetProperties p)
                {
                    var x = new PropertySet(p, right);
                    return x.Evaluate();
                }
                else
                {
                    throw new Exception("");
                }
            default:
                return null;
        }
    }
}
public class Assignment : BinaryExpression
{
    public Assignment(IExpression Left, IExpression Right, Token Operator) : base(Left, Right, Operator)
    {
    }
}
class UnaryExpression : IExpression
{
    IExpression expression;
    Token Operator;
    public UnaryExpression(IExpression Expression, Token Operator)
    {
        expression = Expression;
        this.Operator = Operator;
    }
    public object Evaluate()
    {
        switch (Operator.Type)
        {
            case TokenType.DECREMENT:
                if (expression is Variable v)
                {
                    v.enviroment.SetValue(v.NameOfVariable, (int)expression.Evaluate() - 1);
                    return v.enviroment.GetValue(v.NameOfVariable);
                }
                throw new Exception("");
            case TokenType.MINUS:
                return -(int)expression.Evaluate();
            case TokenType.INCREMENT:
                if (expression is Variable v2)
                {
                    v2.enviroment.SetValue(v2.NameOfVariable, (double)expression.Evaluate() + 1);
                    return v2.enviroment.GetValue(v2.NameOfVariable);
                }
                throw new Exception();
            default:
                return null;
        }
    }
}
public class Atom : IExpression
{
    Object value;

    public Atom(Object Value)
    {
        value = Value;
    }

    public object Evaluate()
    {
        return value;
    }
}
public class ListExpression : IExpression
{
    List<IExpression> exp;
    public ListExpression(List<IExpression> expressions)
    {
        exp = expressions;
    }
    public object Evaluate()
    {
        List<object> list = new List<object>();
        foreach (var item in exp)
        {
            list.Add(item.Evaluate());
        }
        return list;
    }
}

public class Variable : IExpression
{
    public Enviroment enviroment;
    public string NameOfVariable;
    public Variable(string NameOfVariable, Enviroment actual)
    {
        this.NameOfVariable = NameOfVariable;
        enviroment = actual;
    }

    public object Evaluate()
    {
        return enviroment.GetValue(NameOfVariable);
    }
}
public class PropertySet : IExpression
{
    IExpression expression;
    IExpression value;
    public PropertySet(IExpression left, IExpression value)
    {
        this.expression = left;
        this.value = value;
    }

    public object Evaluate()
    {
        if (expression is GetProperties p)
        {
            var e = p.exp.Evaluate();
            if (e is List<object> list)
            {
                switch (p.NameOfPropery)
                {
                    case "Indexer":
                        list[int.Parse(p.args[0].Evaluate().ToString())] = value.Evaluate();
                        return value.Evaluate();
                    default:
                        throw new Exception();
                }
            }
            throw new Exception();
        }
        throw new Exception();
    }
}
public class VariableAssignation : IExpression
{
    Variable variable;
    IExpression value;


    public VariableAssignation(Variable v, IExpression right)
    {
        variable = v;
        value = right;
    }

    public object Evaluate()
    {
        var x = value.Evaluate();
        variable.enviroment.SetValue(variable.NameOfVariable, x);
        return x;
    }

}
public class FunctionCall : IExpression
{
    public IExpression exp;
    string NameOfMethod;
    List<IExpression> args;
    public FunctionCall(IExpression left, string name, List<IExpression> args)
    {
        exp = left;
        NameOfMethod = name;
        this.args = args;
    }
    public object Evaluate()
    {
        var x = exp.Evaluate();
        if (x is List<object> list)
        {
            switch (NameOfMethod)
            {
                case "Push":
                    list.Add(args[0].Evaluate());
                    return typeof(void);
                case "Pop":
                    list.RemoveAt(list.Count - 1);
                    return typeof(void);
                default: throw new Exception();
            }
        }
        throw new Exception();
    }

}
public class ActionExpression : IExpression
{
    public List<Token> Identifiers { get; }
    public List<IStatements> Statements { get; }
    public Enviroment Parent { get; }

    public ActionExpression(List<Token> identifiers, List<IStatements> statements, Enviroment Parent)
    {
        Identifiers = identifiers;
        Statements = statements;
        this.Parent = Parent;
    }


    public object Evaluate()
    {
        return new Action(Identifiers, Statements, Parent);
    }
}
public class GetProperties : IExpression
{
    public IExpression exp;
    public string NameOfPropery;
    public List<IExpression> args;
    public GetProperties(IExpression left, string name, List<IExpression> args)
    {
        exp = left;
        NameOfPropery = name;
        this.args = args;
    }
    public object Evaluate()
    {
        var x = exp.Evaluate();

        if (x is List<object> list)
        {
            switch (NameOfPropery)
            {
                case "Count":
                    return list.Count;
                case "Indexer":
                    return list[int.Parse(args[0].Evaluate().ToString())];
                default: throw new Exception();
            }
        }
        throw new Exception();
    }
}
public class DelegateExpression : IExpression
{
    List<Token> Identifiers;
    IExpression expression;
    Enviroment enviroment;
    public DelegateExpression(IExpression expression, List<Token> tokens, Enviroment Parent)
    {
        this.expression = expression;
        Identifiers = tokens;
        enviroment = Parent;

    }

    public object Evaluate()
    {
        return new Delegate(Identifiers, expression, enviroment);
    }
}

public class Delegate
{
    public List<Token> Identifiers { get; }
    public IExpression Expression { get; }
    public Enviroment Parent;
    public Delegate(List<Token> identifiers, IExpression expression, Enviroment Parent)
    {
        Identifiers = identifiers;
        Expression = expression;
        this.Parent = Parent;
    }

    public object Invoke(params object[] args)
    {
        if (args.Length == Identifiers.Count)
        {
            for (int i = 0; i < Identifiers.Count; i++)
            {
                Parent.SetValue(Identifiers[i].Value, args[i]);
            }
            return Expression.Evaluate();
        }
        else
        {
            throw new Exception();
        }
    }
}

