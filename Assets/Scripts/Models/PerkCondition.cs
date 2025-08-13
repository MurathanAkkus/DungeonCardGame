using System;
using UnityEngine;

public abstract class PerkCondition
{
    [SerializeField] protected ReactionTiming reactionTiming;   // ReactionTiming, perk'in tetiklenme zamanýný belirler (örneðin, eylem öncesi, sonrasý vb.)
    public abstract void SubscribeCondition(Action<GameAction> reaction);   // Abone olma metodu, belirli bir eylem gerçekleþtiðinde tetiklenecek reaksiyonu alýr
    public abstract void UnsubscribeCondition(Action<GameAction> reaction); // Aboneliði kaldýrma metodu, belirli bir eylem için tetiklenecek reaksiyonu kaldýrýr
    public abstract bool SubConditionIsMet(GameAction action);              // Alt koþulun karþýlanýp karþýlanmadýðýný kontrol eder, yani perk'in tetiklenmesi için gerekli þartlarýn saðlanýp saðlanmadýðýný belirler
}
