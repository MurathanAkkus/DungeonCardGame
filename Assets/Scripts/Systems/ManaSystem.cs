using System.Collections;
using UnityEngine;

/// <summary>
/// Oyuncunun mana miktarýný yöneten sistem.
/// Mana harcama, doldurma ve UI güncellemeleri ile ilgilenir.
/// </summary>
public class ManaSystem : Singleton<ManaSystem>
{
    [SerializeField] private ManaUI manaUI;             // Mana bilgisini ekranda gösteren UI referansý

    private const int MAX_MANA = 3;                     // Maksimum mana miktarý
    private int currentMana = MAX_MANA;                 // Þu anki mana miktarý

    void OnEnable()
    {   // Sistem etkinleþtiðinde gerekli event ve performer baðlantýlarý yapýlýr
        // Mana harcama ve doldurma iþlemlerini ActionSystem'e baðla
        ActionSystem.AttachPerformer<SpendManaGA>(SpendManaPerformer);
        ActionSystem.AttachPerformer<RefillManaGA>(RefillManaPerformer);
        // Düþman turu sonrasý mana yenileme reaksiyonu ekle
        ActionSystem.SubscribeReaction<EnemyTurnGA>(EnemyTurnPostReaction, ReactionTiming.POST);
    }

    void OnDisable()
    {   // Sistem devre dýþý kaldýðýnda baðlantýlar kaldýrýlýr
        ActionSystem.DetachPerformer<SpendManaGA>();
        ActionSystem.DetachPerformer<RefillManaGA>();
        ActionSystem.UnsubscribeReaction<EnemyTurnGA>(EnemyTurnPostReaction, ReactionTiming.POST);
    }

    public bool HasEnoughMana(int mana)
    {   // Yeterli mana olup olmadýðýný kontrol eder.
        return currentMana >= mana;
    }

    
    private IEnumerator SpendManaPerformer(SpendManaGA spendManaGA)
    {   // Mana harcama iþlemini gerçekleþtirir ve UI'ý günceller.
        currentMana -= spendManaGA.Amount;
        manaUI.UpdateManaText(currentMana);
        yield return null;  // Eylem için bir gecikme
    }

    private IEnumerator RefillManaPerformer(RefillManaGA refillManaGA)
    {   // Manayý maksimuma doldurur ve UI'ý günceller.
        if (currentMana >= MAX_MANA)
        {   // test için
            Debug.LogWarning("Mana zaten maksimum seviyede.");
            yield break;
        }
        currentMana = MAX_MANA;
        manaUI.UpdateManaText(currentMana);
        yield return null; 
    }

    private void EnemyTurnPostReaction(EnemyTurnGA enemyTurnGA)
    {   // Düþman turu bittikten sonra manayý doldurmak için tetiklenir.
        RefillManaGA refillManaGA = new RefillManaGA();
        ActionSystem.Instance.AddReaction(refillManaGA);
    }
}
