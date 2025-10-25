using System.Collections;
using UnityEngine;

public class DamageSystem : MonoBehaviour
{
    [SerializeField] private GameObject damageVFX;

    void OnEnable()
    {
        ActionSystem.AttachPerformer<DealDamageGA>(DealDamagePerformer);
    }
    void OnDisable()
    {
        ActionSystem.DetachPerformer<DealDamageGA>();
    }

    private IEnumerator DealDamagePerformer(DealDamageGA dealDamageGA)
    {
        foreach (var target in dealDamageGA.Targets)
        {
            if (target == null) 
                continue;

            target.Damage(dealDamageGA.Amount, ignoreArmor: dealDamageGA.IgnoreArmor);

            if (target != null && target.gameObject != null)
                Instantiate(damageVFX, target.transform.position, Quaternion.identity);

            yield return new WaitForSeconds(0.15f); // Hasar animasyonu i�in bekle

            if (target.CurrentHealth <= 0)
            {
                // E�er hedefin can� s�f�r veya alt�na d��t�yse, d��man� �ld�r
                if (target is EnemyView enemyView && enemyView != null)
                {
                    KillEnemyGA killEnemyGA = new KillEnemyGA(enemyView);
                    ActionSystem.Instance.AddReaction(killEnemyGA);
                }
                else
                {
                    // Game Over logic eklenecek
                    // GameOver sahnesine ge�i� yapabiliriz
                }
            }
        }
        // yield return null; // Bir sonraki kareye ge�meden �nce bekle
    }
}
