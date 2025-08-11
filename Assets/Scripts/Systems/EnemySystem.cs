using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySystem : Singleton<EnemySystem>
{
    [SerializeField] private EnemyBoardView enemyBoardView;
    void OnEnable()
    {
        ActionSystem.AttachPerformer<EnemyTurnGA>(EnemyTurnPerformer);
        ActionSystem.AttachPerformer<AttackHeroGA>(AttackHeroPerformer);
    }
    void OnDisable()
    {
        ActionSystem.DetachPerformer<EnemyTurnGA>();
        ActionSystem.DetachPerformer<AttackHeroGA>();
    }

    public void Setup(List<EnemyData> enemyDatas)
    {
        foreach (var enemyData in enemyDatas)
        {   // D��man g�r�n�mlerini olu�tur ve tahtaya ekle
            enemyBoardView.AddEnemy(enemyData);
        }
    }

    // Performers
    private IEnumerator EnemyTurnPerformer(EnemyTurnGA enemyTurnGA)
    {
        foreach (var enemy in enemyBoardView.EnemyViews)
        {
            // D��manlar�n s�rayla hamle yapmas�n� sa�la
            AttackHeroGA attackHeroGA = new AttackHeroGA(enemy);
            ActionSystem.Instance.AddReaction(attackHeroGA);
        }
        yield return null; // Bir sonraki kareye ge�meden �nce bekle
    }

    private IEnumerator AttackHeroPerformer(AttackHeroGA attackHeroGA)
    {
        EnemyView attacker = attackHeroGA.Attacker;
        Tween tween = attacker.transform.DOMoveX(attacker.transform.position.x - 1f, 0.15f);
        yield return tween.WaitForCompletion(); // Tween tamamlanana kadar bekle
        attacker.transform.DOMoveX(attacker.transform.position.x + 1f, 0.15f)
            .OnComplete(() => 
            {
                // Sald�r� sonras� yap�lacak i�lemler
                Debug.Log($"{attacker.name} sald�rd�!");
            });
        // Deal Damage to Hero
        DealDamageGA dealDamageGA = new DealDamageGA(attacker.AttackPower, new List<CombatantView> { HeroSystem.Instance.HeroView });
        ActionSystem.Instance.AddReaction(dealDamageGA);
    }
}
