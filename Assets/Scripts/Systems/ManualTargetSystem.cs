using UnityEngine;

public class ManualTargetSystem : Singleton<ManualTargetSystem>
{
    [SerializeField] private ArrowView arrowView;
    [SerializeField] private LayerMask targetLayerMask;

    public void StartTargeting(Vector3 startPosition)
    {
        arrowView.Show(startPosition);
    }

    public EnemyView EndTargeting(Vector3 endPosition)
    {   
        arrowView.Hide();
        // Raycast atarak hedefi tespit etmeye �al��.
        // endPosition: Ray'in ba�layaca�� konum.
        // Vector3.forward: Ray'in ilerleyece�i y�n (d�nyada Z ekseni ileri y�n).
        // out RaycastHit hit: Ray'in �arpt��� bilgiyi saklamak i�in.
        // 10f: Ray'in maksimum mesafesi.
        // targetLayerMask: Sadece bu layer mask'e sahip nesneler kontrol edilir.
        var cam = Camera.main;
        if (cam == null) return null;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, targetLayerMask))
        {
            if (hit.transform.TryGetComponent(out EnemyView enemyView))
                return enemyView;
        }
        return null;
    }
}