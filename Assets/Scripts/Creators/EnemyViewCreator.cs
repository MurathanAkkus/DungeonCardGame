using UnityEngine;

public class EnemyViewCreator : Singleton<EnemyViewCreator>
{
    [SerializeField] private EnemyView enemyViewPrefab;

    public EnemyView CreateEnemyView(EnemyData enemyData, Vector3 position, Quaternion rotation)
    {
        if (enemyViewPrefab == null)
        {
            Debug.LogError("EnemyViewCreator'de tanýmlanmýþ bir enemyViewPrefab'ý yok!");
            return null;
        }
        EnemyView enemyView = Instantiate(enemyViewPrefab, position, rotation);
        enemyView.Setup(enemyData);
        return enemyView;
    }
}
