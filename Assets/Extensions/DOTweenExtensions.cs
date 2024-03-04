using System.Collections;
using DG.Tweening;

public static class DOTweenExtensions
{
    public static IEnumerator WaitForCompletion(this Tween tween)
    {
        bool isCompleted = false;
        tween.OnComplete(() => isCompleted = true);

        // Wait until the tween marks isCompleted as true
        while (!isCompleted)
        {
            yield return null;
        }
    }
}