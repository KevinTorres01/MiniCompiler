public interface IExpression
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
                return (int)left.Evaluate() * (int)right.Evaluate();
            case TokenType.DIV:
                return (int)left.Evaluate() / (int)right.Evaluate();
            case TokenType.PLUS:
                return (int)left.Evaluate() + (int)right.Evaluate();
            case TokenType.MINUS:
                return (int)left.Evaluate() - (int)right.Evaluate();
            case TokenType.OR:
                return (bool)left.Evaluate() || (bool)right.Evaluate();
            case TokenType.AND:
                return (bool)left.Evaluate() && (bool)right.Evaluate();
            case TokenType.GREATEREQUALS:
                return (int)left.Evaluate() >= (int)right.Evaluate();
            case TokenType.GREATER:
                return (int)left.Evaluate() > (int)right.Evaluate();
            case TokenType.LESS:
                return (int)left.Evaluate() < (int)right.Evaluate();
            case TokenType.LESSEQUALS:
                return (int)left.Evaluate() <= (int)right.Evaluate();
            case TokenType.EQUALS:
                return (int)left.Evaluate() == (int)right.Evaluate();
            case TokenType.CONCATWHITHOUTSPACE:
                return (string)left.Evaluate() + (string)right.Evaluate();
            case TokenType.CONCATWHITHSPACE:
                return (string)left.Evaluate() + " " + (string)right.Evaluate();
            case TokenType.POTENCIATION:
                return Math.Pow((int)left.Evaluate(), (int)right.Evaluate());
            default:
                return null;
        }
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
                return (int)expression.Evaluate() - 1;
            case TokenType.MINUS:
                return -(int)expression.Evaluate();
            case TokenType.INCREMENT:
                return (int)expression.Evaluate() + 1;
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
