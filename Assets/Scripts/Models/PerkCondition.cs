using System;
using UnityEngine;

public abstract class PerkCondition
{
    [SerializeField] protected ReactionTiming reactionTiming;   // ReactionTiming, perk'in tetiklenme zaman�n� belirler (�rne�in, eylem �ncesi, sonras� vb.)
    public abstract void SubscribeCondition(Action<GameAction> reaction);   // Abone olma metodu, belirli bir eylem ger�ekle�ti�inde tetiklenecek reaksiyonu al�r
    public abstract void UnsubscribeCondition(Action<GameAction> reaction); // Aboneli�i kald�rma metodu, belirli bir eylem i�in tetiklenecek reaksiyonu kald�r�r
    public abstract bool SubConditionIsMet(GameAction action);              // Alt ko�ulun kar��lan�p kar��lanmad���n� kontrol eder, yani perk'in tetiklenmesi i�in gerekli �artlar�n sa�lan�p sa�lanmad���n� belirler
}
