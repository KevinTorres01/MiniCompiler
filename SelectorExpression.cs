public class SelectorExpression : IExpression
{
    public IExpression Source;
    public IExpression Single;
    public IExpression predicate;
    public SelectorExpression(IExpression Source, IExpression Single, IExpression predicate)
    {
        this.Source = Source;
        this.Single = Single;
        this.predicate = predicate;
    }

    public object Evaluate()
    {
        object sourc = Source.Evaluate();
        object sing = Single.Evaluate();
        object pred = predicate.Evaluate();

        if (sourc is string a && sing is bool b && pred is Delegate j)
        {
            return new Selector(a, b, j);

        }
        throw new Exception();
    }
}
public class Selector
{
    string source;
    bool single;
    Delegate Delegate;
    public Selector(string source, bool single, Delegate predicate)
    {
        this.source = source;
        this.single = single;
        Delegate = predicate;
    }
}