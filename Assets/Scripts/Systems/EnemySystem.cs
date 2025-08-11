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
        {   // Düþman görünümlerini oluþtur ve tahtaya ekle
            enemyBoardView.AddEnemy(enemyData);
        }
    }

    // Performers
    IEnumerator EnemyTurnPerformer(EnemyTurnGA enemyTurnGA)
    {
        Debug.Log("Enemy Turn!");
        // Düþmanýn perform'unun tamamlanmasýný bekle
        yield return new WaitForSeconds(2f); // Düþman turu için gereken süreyi simüle et
        // Performers tamamlandýktan sonra düþman turunun bitiþini bildir
        Debug.Log("Enemy Turn Ended!");
    }
}
