using UnityEngine;

public class ManualTargetSystem : Singleton<ManualTargetSystem>
{
    [SerializeField] private ArrowView arrowView;
    [SerializeField] private LayerMask targetLayerMask;

    public void StartTargeting(Vector3 startPosition)
    {
        // Ok görünümünü baþlat ve baþlangýç pozisyonunu ayarla.
        arrowView.SetupArrow(startPosition);
        // Ok görünümünü etkinleþtir.
        arrowView.gameObject.SetActive(true);
    }

    public EnemyView EndTargeting(Vector3 endPosition)
    {   // Ok görünümünü devre dýþý býrak.
        arrowView.gameObject.SetActive(false);
        // Raycast atarak hedefi tespit etmeye çalýþ.
        // endPosition: Ray'in baþlayacaðý konum.
        // Vector3.forward: Ray'in ilerleyeceði yön (dünyada Z ekseni ileri yön).
        // out RaycastHit hit: Ray'in çarptýðý bilgiyi saklamak için.
        // 10f: Ray'in maksimum mesafesi.
        // targetLayerMask: Sadece bu layer mask'e sahip nesneler kontrol edilir.
        if (Physics.Raycast(endPosition, Vector3.forward, out RaycastHit hit, 10f, targetLayerMask)
            && hit.collider != null // Bir collider'a çarpýlmýþ mý kontrol et.
            && hit.transform.TryGetComponent(out EnemyView enemyView)) // // Çarpýlan nesnede EnemyView var mý kontrol et.
        {   // Eðer ray bir düþmana çarpmýþsa, o düþmanýn EnemyView bileþenini döndür.
            return enemyView;
        }
        // Hiçbir düþmana çarpmadýysa null döndür.
        return null;
    }
}
