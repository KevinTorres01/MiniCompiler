public class Enviroment
{
    public Dictionary<string, object> pairs = new Dictionary<string, object>();
    public Enviroment parent;
    public Enviroment(Enviroment parent)
    {
        this.parent = parent;
    }
    public Enviroment()
    {

    }
    public Enviroment Search(string search)
    {
        if (pairs.ContainsKey(search))
        {
            return this;
        }
        if (parent == null)
        {
            return null;
        }
        return parent.Search(search);
    }
    public void SetValue(string id, object value)
    {
        Enviroment enviroment = Search(id);
        if (enviroment == null)
        {
            this.pairs.Add(id, value);
        }
        else
        {
            enviroment.pairs[id] = value;
        }
    }
    public object GetValue(string id)
    {

        if (pairs.ContainsKey(id))
        {
            return pairs[id];
        }
        if (parent == null)
        {
            throw new Exception("");
        }
        return parent.GetValue(id);
    }
}