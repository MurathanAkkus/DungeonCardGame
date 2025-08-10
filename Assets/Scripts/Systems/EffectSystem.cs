using System.Collections;
using UnityEngine;

public class EffectSystem : MonoBehaviour
{
    void OnEnable()
    {
        // Sistemin aktif olduðu anda gerekli performer ve reaction aboneliklerini yapar
        Debug.Log("EffectSystem OnEnable");
        ActionSystem.AttachPerformer<PerformEffectGA>(PerformEffectPerformer);
    }
    void OnDisable()
    {
        // Sistem devre dýþý olduðunda abonelikleri kaldýrýr
        ActionSystem.DetachPerformer<PerformEffectGA>();
    }

    private IEnumerator PerformEffectPerformer(PerformEffectGA performEffectGA)
    {
        GameAction effectAction = performEffectGA.Effect.GetGameAction();       // PerformEffectGA içindeki Effect nesnesinden ilgili GameAction'ý alýr
        ActionSystem.Instance.AddReaction(effectAction);                        // Alýnan GameAction'ý ActionSystem'e bir reaksiyon olarak ekler
        yield return null;                                                      // Coroutine'in bir sonraki frame'e geçmesini saðlar
    }
}
