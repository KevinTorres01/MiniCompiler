public interface IAstNode;
public interface IProgramNode : IAstNode
{
    public void Create();
    public void CreateCard();
    public void CreateEfect();
}

public class CompiledCard
{
    string Name;
    double Power;
    string Type;
    string Faction;
    string Range;
    public List<OnActivationObject> onActivations = new List<OnActivationObject>();
    public CompiledCard(string name, double power, string type, string faction, string range, List<OnActivationObject> onActivations)
    {
        Name = name;
        Power = power;
        Type = type;
        Faction = faction;
        Range = range;
        this.onActivations = onActivations;

    }
    public override string ToString()
    {
        return Name + " " + Power + " " + Type + " " + Range + " " + Faction;
    }
}
public class CompiledEffect
{
    public string Name;
    public List<Params> Params;
    public Action action;
    public CompiledEffect(string name, List<Params> Params, Action action)
    {
        this.Name = name;
        this.Params = Params;
        this.action = action;
    }
}