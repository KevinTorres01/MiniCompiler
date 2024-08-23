public interface IStatements : IAstNode
{
    public void Execute();
}
public class ExpStms : IStatements
{
    IExpression expression;
    public ExpStms(IExpression exp)
    {
        expression = exp;
    }
    public void Execute()
    {
        expression.Evaluate();
    }
}
public class IfStmt : IStatements
{
    IExpression condition;
    public List<IStatements> statements;
    public List<IStatements> ElseStatements;
    public IfStmt(IExpression condition)
    {
        this.condition = condition;
    }
    public void Execute()
    {

        if (condition.Evaluate() is true)
        {
            foreach (var item in statements)
            {
                item.Execute();
            }
        }
        else
        {
            foreach (var item in ElseStatements)
            {
                item.Execute();
            }
        }
    }
}
class WhileStmt : IStatements
{
    IExpression condition;
    public List<IStatements> statements;
    public WhileStmt(IExpression condition)
    {
        this.condition = condition;
    }
    public void Execute()
    {
        while (condition.Evaluate() is true)
        {
            foreach (var item in statements)
            {
                item.Execute();
            }
        }
    }
}
public class ForStms : IStatements
{
    string id;
    IExpression exp;
    List<IStatements> statements;
    Enviroment enviroment;

    public ForStms(string Id, IExpression expression, List<IStatements> stm, Enviroment enviroment)
    {
        id = Id;
        exp = expression;
        statements = stm;
        this.enviroment = enviroment;
    }
    public void Execute()
    {
        if (exp.Evaluate() is IEnumerable<object> l)
        {
            var j = l.ToList();
            for (int i = 0; i < j.Count; i++)
            {
                enviroment.SetValue(id, j[i]);
                foreach (var item in statements)
                {
                    item.Execute();
                }
            }

        }
    }
}
public class PrintStmt : IStatements
{
    IExpression exp;
    public PrintStmt(IExpression exp)
    {
        this.exp = exp;
    }
    public void Execute()
    {
        System.Console.WriteLine(exp.Evaluate());
    }
}