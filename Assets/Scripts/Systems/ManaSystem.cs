using System.Collections;
using UnityEngine;

/// <summary>
/// Oyuncunun mana miktar�n� y�neten sistem.
/// Mana harcama, doldurma ve UI g�ncellemeleri ile ilgilenir.
/// </summary>
public class ManaSystem : Singleton<ManaSystem>
{
    [SerializeField] private ManaUI manaUI;             // Mana bilgisini ekranda g�steren UI referans�

    private const int MAX_MANA = 3;                     // Maksimum mana miktar�
    private int currentMana = MAX_MANA;                 // �u anki mana miktar�

    void OnEnable()
    {   // Sistem etkinle�ti�inde gerekli event ve performer ba�lant�lar� yap�l�r
        // Mana harcama ve doldurma i�lemlerini ActionSystem'e ba�la
        ActionSystem.AttachPerformer<SpendManaGA>(SpendManaPerformer);
        ActionSystem.AttachPerformer<RefillManaGA>(RefillManaPerformer);
        // D��man turu sonras� mana yenileme reaksiyonu ekle
        ActionSystem.SubscribeReaction<EnemyTurnGA>(EnemyTurnPostReaction, ReactionTiming.POST);
    }

    void OnDisable()
    {   // Sistem devre d��� kald���nda ba�lant�lar kald�r�l�r
        ActionSystem.DetachPerformer<SpendManaGA>();
        ActionSystem.DetachPerformer<RefillManaGA>();
        ActionSystem.UnsubscribeReaction<EnemyTurnGA>(EnemyTurnPostReaction, ReactionTiming.POST);
    }

    public bool HasEnoughMana(int mana)
    {   // Yeterli mana olup olmad���n� kontrol eder.
        return currentMana >= mana;
    }

    
    private IEnumerator SpendManaPerformer(SpendManaGA spendManaGA)
    {   // Mana harcama i�lemini ger�ekle�tirir ve UI'� g�nceller.
        currentMana -= spendManaGA.Amount;
        manaUI.UpdateManaText(currentMana);
        yield return null;  // Eylem i�in bir gecikme
    }

    private IEnumerator RefillManaPerformer(RefillManaGA refillManaGA)
    {   // Manay� maksimuma doldurur ve UI'� g�nceller.
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
    {   // D��man turu bittikten sonra manay� doldurmak i�in tetiklenir.
        RefillManaGA refillManaGA = new RefillManaGA();
        ActionSystem.Instance.AddReaction(refillManaGA);
    }
}
