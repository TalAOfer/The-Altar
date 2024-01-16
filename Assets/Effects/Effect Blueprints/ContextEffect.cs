using System.Collections;

public class ContextEffect : Effect
{
    public EffectTarget target;

    public void Initialize(EffectTarget target)
    {
        this.target = target;
    }

    public override IEnumerator Apply(EffectContext context)
    {
        ApplierContext applierContext = new ApplierContext(null, null);
        switch (target)
        {
            case EffectTarget.Initiating:
                applierContext.TargetCard = context.InitiatingCard;
                applierContext.OtherCard = context.OtherCard;
                break;
            case EffectTarget.Other:
                applierContext.TargetCard = context.OtherCard;
                applierContext.OtherCard = context.InitiatingCard;
                break;
        }

        yield return StartCoroutine(applier.Apply(applierContext));
    }
}

public enum ApplierTarget
{
    Target,
    Other,
}

public enum EffectTarget
{
    Initiating,
    Other
}
