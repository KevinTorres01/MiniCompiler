public class CardDeclaration : IProgramNode
{
    IExpression name;
    IExpression power;
    IExpression type;
    IExpression range;
    IExpression faction;
    Context context;
    public CardDeclaration(IExpression Name, IExpression Power, IExpression Type, IExpression Range, IExpression Faction, Context context)
    {
        name = Name;
        power = Power;
        type = Type;
        range = Range;
        faction = Faction;
        this.context = context;
    }

    public void CreateCard()
    {
        object Name = name.Evaluate();
        object Power = power.Evaluate();
        object Type = type.Evaluate();
        object Range = range.Evaluate();
        object Faction = faction.Evaluate();
        if (Name is string stringName && Power is double intPower && intPower >= 0 && Type is string stringType && (stringType == "Golden" || stringType == "Silver" || stringType == "Lider") && Faction is string stringFaction)
        {
            if (Range is string stringRange)
            {
                int M = 0;
                int S = 0;
                int R = 0;
                for (int i = 0; i < stringRange.Length; i++)
                {
                    if (stringRange[i] == 'M')
                        M++;
                    else if (stringRange[i] == 'R')
                        R++;
                    else if (stringRange[i] == 'S')
                        S++;
                    else
                        break;
                }
                if (M <= 1 && S <= 1 && R <= 1 && S + R + M == stringRange.Length)
                {
                    context.cards.Add(stringName, new CompiledCard(stringName, intPower, stringType, stringFaction, stringRange));
                    return;
                }
                else
                {
                    throw new Exception("");
                }
            }
            else
            {
                throw new Exception();
            }
        }
        throw new Exception();
    }

    public void Create()
    {
        CreateCard();
    }

    public void CreateEfect()
    {
        throw new NotImplementedException();
    }
}
