using UnityEngine;

public class HeroSystem : Singleton<HeroSystem>
{
    // HeroSystem, oyundaki kahramanla ilgili
    // kahraman seçimi, kahraman istatistikleri
    // ve kahraman yetenekleri gibi kahramanla
    // ilgili iþlevleri yönetmek için geniþletilebilir.
    [field: SerializeField] public HeroView HeroView { get; private set; }

    void OnEnable()
    {
        ActionSystem.SubscribeReaction<EnemyTurnGA>(EnemyTurnPreReaction, ReactionTiming.PRE);
        ActionSystem.SubscribeReaction<EnemyTurnGA>(EnemyTurnPostReaction, ReactionTiming.POST);
    }

    void OnDisable()
    {
        ActionSystem.UnsubscribeReaction<EnemyTurnGA>(EnemyTurnPreReaction, ReactionTiming.PRE);
        ActionSystem.UnsubscribeReaction<EnemyTurnGA>(EnemyTurnPostReaction, ReactionTiming.POST);
    }

    public void Setup(HeroData heroData)
    {
        HeroView.Setup(heroData);
    }

    // -------------------------- Reactions --------------------------
    private void EnemyTurnPreReaction(EnemyTurnGA enemyTurnGA)
    {   // Düþman turu baþlamadan önce elindeki tüm kartlarý discard etmek için reaction ekler
        DiscardAllCardsGA discardAllCardsGA = new DiscardAllCardsGA();
        ActionSystem.Instance.AddReaction(discardAllCardsGA);
    }
    private void EnemyTurnPostReaction(EnemyTurnGA enemyTurnGA)
    {   
        int burnStacks = HeroView.GetStatusEffectStackCount(StatusEffectType.BURN);
        if (burnStacks > 0)
        {
            ApplyBurnGA applyBurnGA = new(burnStacks, HeroView);
            ActionSystem.Instance.AddReaction(applyBurnGA);
        }
        // Düþman turu bittikten sonra 5 kart çekmek için reaction ekler
        DrawCardsGA drawCardsGA = new DrawCardsGA(5);
        ActionSystem.Instance.AddReaction(drawCardsGA);
    }
}