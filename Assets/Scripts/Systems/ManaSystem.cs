using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaSystem : Singleton<ManaSystem>
{
    [SerializeField] private ManaUI manaUI;

    private const int MAX_MANA = 3;
    private int currentMana = MAX_MANA;
    
    void OnEnable()
    {
        ActionSystem.AttachPerformer<SpendManaGA>(SpendManaPerformer);
        ActionSystem.AttachPerformer<RefillManaGA>(RefillManaPerformer);
        ActionSystem.SubscribeReaction<EnemyTurnGA>(EnemyTurnPostReaction, ReactionTiming.POST);
    }

    void OnDisable()
    {
        ActionSystem.DetachPerformer<SpendManaGA>();
        ActionSystem.DetachPerformer<RefillManaGA>();
        ActionSystem.UnsubscribeReaction<EnemyTurnGA>(EnemyTurnPostReaction, ReactionTiming.POST);
    }

    public bool HasEnoughMana(int mana)
    {
        return currentMana >= mana;
    }

    private IEnumerator SpendManaPerformer(SpendManaGA spendManaGA)
    {
        currentMana -= spendManaGA.Amount;
        manaUI.UpdateManaText(currentMana);
        yield return null;  // Eylem i�in bir gecikme
    }

    private IEnumerator RefillManaPerformer(RefillManaGA refillManaGA)
    {
        if (currentMana >= MAX_MANA)
        {   // test i�in
            Debug.LogWarning("Mana zaten maksimum seviyede.");
            yield break;
        }
        currentMana = MAX_MANA;
        manaUI.UpdateManaText(currentMana);
        yield return null; 
    }

    private void EnemyTurnPostReaction(EnemyTurnGA enemyTurnGA)
    {
        // D��man turu sonras� mana yenileme i�lemi
        RefillManaGA refillManaGA = new RefillManaGA();
        ActionSystem.Instance.AddReaction(refillManaGA);
    }
}
