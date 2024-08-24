public class OnActivationObjectExpression : IExpression
{
    EffectInfoExpression effect;
    SelectorExpression selector;
    OnActivationObjectExpression PostAction;
    public OnActivationObjectExpression(EffectInfoExpression effect, SelectorExpression selector, OnActivationObjectExpression postact)
    {
        this.effect = effect;
        this.selector = selector;
        PostAction = postact;
    }

    public object Evaluate()
    {
        object eff = effect.Evaluate();
        object sel = selector.Evaluate();
        if (eff is EffectInfo effectInfo && sel is Selector selectorinf)
        {
            if (PostAction == null)
            {
                return new OnActivationObject(effectInfo, selectorinf);
            }
            else
            {
                var postAction = PostAction.Evaluate();
                if (postAction is OnActivationObject onActivationObject)
                {
                    return new OnActivationObject(effectInfo, selectorinf, onActivationObject);

                }
                else
                {
                    throw new Exception();
                }
            }
        }
        throw new Exception();
    }
}