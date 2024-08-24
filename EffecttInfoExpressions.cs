public class EffectInfoExpression : IExpression
{
    public Dictionary<string, IExpression> paramsValues;
    public IExpression Name;
    public Context Context;

    public EffectInfoExpression(Dictionary<string, IExpression> paramsValues, IExpression name, Context context)
    {
        this.paramsValues = paramsValues;
        Name = name;
        Context = context;
    }

    public object Evaluate()
    {
        object name = Name.Evaluate();
        Dictionary<string, object> keyValuePairs = new Dictionary<string, object>();
        if (name is string s && Context.effects.ContainsKey(s) && paramsValues.Count == Context.effects[s].Params.Count)
        {
            foreach (var item in Context.effects[s].Params)
            {
                var nam = item.Name;
                var value = paramsValues[nam].Evaluate();
                if (value is double d && item.type == TokenType.NUMBER)
                {
                    keyValuePairs.Add(nam, d);
                }
                else if (value is string k && item.type == TokenType.STRINGKEYWORD)
                {
                    keyValuePairs.Add(nam, k);
                }
                else if (value is bool m && item.type == TokenType.BOOL)
                {
                    keyValuePairs.Add(nam, m);
                }
                else
                {
                    throw new Exception("");
                }

            }
            return new EffectInfo(s, keyValuePairs);
        }
        throw new Exception("");

    }
}