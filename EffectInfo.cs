public class EffectInfo
{
    Dictionary<string, object> Params = new Dictionary<string, object>();
    string name;
    public EffectInfo(string name, Dictionary<string, object> Params)
    {
        this.name = name;
        this.Params = Params;
    }
}