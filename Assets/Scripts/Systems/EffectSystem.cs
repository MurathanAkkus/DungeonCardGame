using System.Collections;
using UnityEngine;

public class EffectSystem : MonoBehaviour
{
    void OnEnable()
    {
        // Sistemin aktif oldu�u anda gerekli performer ve reaction aboneliklerini yapar
        Debug.Log("EffectSystem OnEnable");
        ActionSystem.AttachPerformer<PerformEffectGA>(PerformEffectPerformer);
    }
    void OnDisable()
    {
        // Sistem devre d��� oldu�unda abonelikleri kald�r�r
        ActionSystem.DetachPerformer<PerformEffectGA>();
    }

    private IEnumerator PerformEffectPerformer(PerformEffectGA performEffectGA)
    {
        GameAction effectAction = performEffectGA.Effect.GetGameAction();       // PerformEffectGA i�indeki Effect nesnesinden ilgili GameAction'� al�r
        ActionSystem.Instance.AddReaction(effectAction);                        // Al�nan GameAction'� ActionSystem'e bir reaksiyon olarak ekler
        yield return null;                                                      // Coroutine'in bir sonraki frame'e ge�mesini sa�lar
    }
}
