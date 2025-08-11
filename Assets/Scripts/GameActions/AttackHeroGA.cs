// Bu kod oyunda bir kahramana saldýrmak için bir eylemi tanýmlar.
public class AttackHeroGA : GameAction
{
    public EnemyView Attacker { get; private set; }
    public AttackHeroGA(EnemyView attacker)
    {
        Attacker = attacker;
    }
}