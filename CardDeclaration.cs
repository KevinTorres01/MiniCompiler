public class CardDeclaration : IProgramNode
{
    IExpression name;
    IExpression power;
    IExpression type;
    IExpression range;
    IExpression faction;
    List<OnActivationObjectExpression> onActivationDeclarations;
    Context context;
    public CardDeclaration(IExpression Name, IExpression Power, IExpression Type, IExpression Range, IExpression Faction, Context context, List<OnActivationObjectExpression> onActivationDeclarations)
    {
        name = Name;
        power = Power;
        type = Type;
        range = Range;
        faction = Faction;
        this.context = context;
        this.onActivationDeclarations = onActivationDeclarations;
    }

    public void CreateCard()
    {
        object Name = name.Evaluate();
        object Power = power.Evaluate();
        object Type = type.Evaluate();
        object Range = range.Evaluate();
        object Faction = faction.Evaluate();
        var onActivation = onActivationDeclarations.Select(x => (OnActivationObject)x.Evaluate()).ToList();

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
                    context.cards.Add(stringName, new CompiledCard(stringName, intPower, stringType, stringFaction, stringRange, onActivation));
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
