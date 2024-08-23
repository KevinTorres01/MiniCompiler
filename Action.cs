public class Action
{
    public List<Token> Identifiers { get; }
    public List<IStatements> Statements { get; }
    public Enviroment Parent { get; }
    public Action(List<Token> identifiers, List<IStatements> statements, Enviroment Parent)
    {
        Identifiers = identifiers;
        Statements = statements;
        this.Parent = Parent;
    }

    public void Invoke(params object[] args)
    {
        if (args.Length == Identifiers.Count)
        {
            for (int i = 0; i < args.Length; i++)
            {
                Parent.SetValue(Identifiers[i].Value, args[i]);
            }
            foreach (var item in Statements)
            {
                item.Execute();
            }
            return;
        }
        throw new Exception("");
    }

}