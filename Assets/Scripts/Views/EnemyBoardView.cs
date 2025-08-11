using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoardView : MonoBehaviour
{
    [SerializeField] private List<Transform> slots; // Enemy kartlarýnýn tutulduðu slotlar
    public List<EnemyView> EnemyViews { get; private set; } = new List<EnemyView>();

    public void AddEnemy(EnemyData enemyData)
    {
        Transform slot = slots[EnemyViews.Count];
        EnemyView enemyView = EnemyViewCreator.Instance.CreateEnemyView(enemyData, slot.position, slot.rotation);
        enemyView.transform.parent = slot;
        EnemyViews.Add(enemyView);
    }

    public IEnumerator RemoveEnemy(EnemyView enemyView)
    {
        if (EnemyViews.Contains(enemyView))
        {
            EnemyViews.Remove(enemyView);
            Tween tween = enemyView.transform.DOScale(Vector3.zero, 0.25f);
            yield return tween.WaitForCompletion(); // Tween tamamlanana kadar bekle
            Destroy(enemyView.gameObject); // Kartý yok et
            
        }
        else
        {
            Debug.LogWarning("EnemyView listede yok!");
        }
        yield return null; // Bir sonraki kareye geçmeden önce bekle
    }
}
