using System;
using UnityEngine;

public class OnEnemyAttackCondition : PerkCondition
{
    public override bool SubConditionMet(GameAction action)
    {   // Bu koþul her zaman saðlanýr, çünkü bu, avantajýn tetikleyicisidir.
        return true; // Saldýrganýn caný X'in üzerindeyse
    }

    public override void SubscribeCondition(Action<GameAction> reaction)
    {   // Bu koþul, düþman bir saldýrý gerçekleþtirdiðinde tetiklenir.
        ActionSystem.SubscribeReaction<AttackHeroGA>(reaction, reactionTiming);
    }

    public override void UnsubscribeCondition(Action<GameAction> reaction)
    {   // Bu koþul, düþman bir saldýrý gerçekleþtirdiðinde tetiklenir.
        ActionSystem.UnsubscribeReaction<AttackHeroGA>(reaction, reactionTiming);
    }
}
