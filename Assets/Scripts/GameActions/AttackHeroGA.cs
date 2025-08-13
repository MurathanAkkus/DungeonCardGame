// Bu kod oyunda bir kahramana sald�rmak i�in bir eylemi tan�mlar.
public class AttackHeroGA : GameAction, IHaveCaster
{
    public EnemyView Attacker { get; private set; }
    public CombatantView Caster { get; private set; }

    public AttackHeroGA(EnemyView attacker)
    {
        Attacker = attacker;
        Caster = Attacker;
    }
}