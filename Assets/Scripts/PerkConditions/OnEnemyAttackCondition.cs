using System;
using UnityEngine;

public class OnEnemyAttackCondition : PerkCondition
{
    public override bool SubConditionMet(GameAction action)
    {   // Bu ko�ul her zaman sa�lan�r, ��nk� bu, avantaj�n tetikleyicisidir.
        return true; // Sald�rgan�n can� X'in �zerindeyse
    }

    public override void SubscribeCondition(Action<GameAction> reaction)
    {   // Bu ko�ul, d��man bir sald�r� ger�ekle�tirdi�inde tetiklenir.
        ActionSystem.SubscribeReaction<AttackHeroGA>(reaction, reactionTiming);
    }

    public override void UnsubscribeCondition(Action<GameAction> reaction)
    {   // Bu ko�ul, d��man bir sald�r� ger�ekle�tirdi�inde tetiklenir.
        ActionSystem.UnsubscribeReaction<AttackHeroGA>(reaction, reactionTiming);
    }
}
