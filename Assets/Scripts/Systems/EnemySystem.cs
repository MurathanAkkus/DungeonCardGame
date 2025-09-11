using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// <summary>
// EnemySystem.cs
// Bu class oyundaki d��man sistemini y�netir, d��manlar�n turlar�n�, sald�r�lar�n� ve �l�mlerini y�netir.
// </summary>
public class EnemySystem : Singleton<EnemySystem>
{
    [SerializeField] private EnemyBoardView enemyBoardView;
    public List<EnemyView> Enemies => enemyBoardView.EnemyViews;
    void OnEnable()
    {
        ActionSystem.AttachPerformer<EnemyTurnGA>(EnemyTurnPerformer);
        ActionSystem.AttachPerformer<AttackHeroGA>(AttackHeroPerformer);
        ActionSystem.AttachPerformer<KillEnemyGA>(KillEnemyPerformer);
    }
    void OnDisable()
    {
        ActionSystem.DetachPerformer<EnemyTurnGA>();
        ActionSystem.DetachPerformer<AttackHeroGA>();
        ActionSystem.DetachPerformer<KillEnemyGA>();
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
            int burnStacks = enemy.GetStatusEffectStackCount(StatusEffectType.BURN);

            if ( burnStacks > 0)
            {
                ApplyBurnGA applyBurnGA = new(burnStacks, enemy);
                ActionSystem.Instance.AddReaction(applyBurnGA);
            }

            AttackHeroGA attackHeroGA = new AttackHeroGA(enemy);
            ActionSystem.Instance.AddReaction(attackHeroGA);
        }
        yield return null; 
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
        DealDamageGA dealDamageGA = new DealDamageGA(attacker.AttackPower, new List<CombatantView> { HeroSystem.Instance.HeroView }, attackHeroGA.Caster);
        ActionSystem.Instance.AddReaction(dealDamageGA);
    }

    private IEnumerator KillEnemyPerformer(KillEnemyGA killEnemyGA)
    {
        yield return enemyBoardView.RemoveEnemy(killEnemyGA.EnemyView);
    }
}