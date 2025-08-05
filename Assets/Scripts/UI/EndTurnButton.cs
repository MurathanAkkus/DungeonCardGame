using UnityEngine;

public class EndTurnButton : MonoBehaviour
{
    public void OnClick()
    {
        EnemyTurnGA enemyTurnGA = new EnemyTurnGA();
        ActionSystem.Instance.Perform(enemyTurnGA);
    }
}