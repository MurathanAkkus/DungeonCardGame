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

            yield return new WaitForSeconds(0.15f); // Hasar animasyonu için bekle

            if (target.CurrentHealth <= 0)
            {
                // Eðer hedefin caný sýfýr veya altýna düþtüyse, düþmaný öldür
                if (target is EnemyView enemyView && enemyView != null)
                {
                    KillEnemyGA killEnemyGA = new KillEnemyGA(enemyView);
                    ActionSystem.Instance.AddReaction(killEnemyGA);
                }
                else
                {
                    // Game Over logic eklenecek
                    // GameOver sahnesine geçiþ yapabiliriz
                }
            }
        }
        // yield return null; // Bir sonraki kareye geçmeden önce bekle
    }
}
