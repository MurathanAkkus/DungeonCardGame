using UnityEngine;

public class ManualTargetSystem : Singleton<ManualTargetSystem>
{
    [SerializeField] private ArrowView arrowView;
    [SerializeField] private LayerMask targetLayerMask;

    public void StartTargeting(Vector3 startPosition)
    {
        // Ok g�r�n�m�n� ba�lat ve ba�lang�� pozisyonunu ayarla.
        arrowView.SetupArrow(startPosition);
        // Ok g�r�n�m�n� etkinle�tir.
        arrowView.gameObject.SetActive(true);
    }

    public EnemyView EndTargeting(Vector3 endPosition)
    {   // Ok g�r�n�m�n� devre d��� b�rak.
        arrowView.gameObject.SetActive(false);
        // Raycast atarak hedefi tespit etmeye �al��.
        // endPosition: Ray'in ba�layaca�� konum.
        // Vector3.forward: Ray'in ilerleyece�i y�n (d�nyada Z ekseni ileri y�n).
        // out RaycastHit hit: Ray'in �arpt��� bilgiyi saklamak i�in.
        // 10f: Ray'in maksimum mesafesi.
        // targetLayerMask: Sadece bu layer mask'e sahip nesneler kontrol edilir.
        if (Physics.Raycast(endPosition, Vector3.forward, out RaycastHit hit, 10f, targetLayerMask)
            && hit.collider != null // Bir collider'a �arp�lm�� m� kontrol et.
            && hit.transform.TryGetComponent(out EnemyView enemyView)) // // �arp�lan nesnede EnemyView var m� kontrol et.
        {   // E�er ray bir d��mana �arpm��sa, o d��man�n EnemyView bile�enini d�nd�r.
            return enemyView;
        }
        // Hi�bir d��mana �arpmad�ysa null d�nd�r.
        return null;
    }
}
