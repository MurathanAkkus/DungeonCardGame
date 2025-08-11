using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySystem : Singleton<EnemySystem>
{
    [SerializeField] private EnemyBoardView enemyBoardView;
    void OnEnable()
    {
        ActionSystem.AttachPerformer<EnemyTurnGA>(EnemyTurnPerformer);
    }
    void OnDisable()
    {
        ActionSystem.DetachPerformer<EnemyTurnGA>();
    }

    public void Setup(List<EnemyData> enemyDatas)
    {
        foreach (var enemyData in enemyDatas)
        {   // D��man g�r�n�mlerini olu�tur ve tahtaya ekle
            enemyBoardView.AddEnemy(enemyData);
        }
    }

    // Performers
    IEnumerator EnemyTurnPerformer(EnemyTurnGA enemyTurnGA)
    {
        Debug.Log("Enemy Turn!");
        // D��man�n perform'unun tamamlanmas�n� bekle
        yield return new WaitForSeconds(2f); // D��man turu i�in gereken s�reyi sim�le et
        // Performers tamamland�ktan sonra d��man turunun biti�ini bildir
        Debug.Log("Enemy Turn Ended!");
    }
}
