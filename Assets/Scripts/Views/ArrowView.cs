using UnityEngine;

/// <summary>
/// Oku ve ok �izgisini g�rsel olarak y�neten s�n�f.
/// Mouse konumuna g�re ok ba�� ve y�n� g�ncellenir.
/// </summary>
public class ArrowView : MonoBehaviour
{
    [SerializeField] private GameObject arrowHead;
    [SerializeField] private LineRenderer lineRenderer; // Ok �izgisi i�in

    private Vector3 startPosition;

    /// <summary>
    /// Her karede ok ba�� ve y�n�n� mouse konumuna g�re g�nceller.
    /// </summary>
    private void Update()
    {
        // Mouse'un d�nya koordinatlar�ndaki konumunu al.
        Vector3 endPosition = MouseUtil.GetMousePositionInWorldSpace();
        // Ba�lang�� pozisyonundan ok ba��na olan y�n� hesapla ve normalize et.
        Vector3 direction = -(startPosition - arrowHead.transform.position).normalized;
        // Ok �izgisinin ucunu, ok ba�� ile �ak��mamas� i�in ok ba��n�n y�n�nde 0.5 birim geriye al�r.
        lineRenderer.SetPosition(1, endPosition - direction * 0.5f);
        // Ok ba��n� mouse konumuna ta��.
        arrowHead.transform.position = endPosition;
        // Ok ba��n�n y�n�n� ayarla.
        arrowHead.transform.right = direction;
    }


    /// <summary>
    /// Okun ba�lang�� pozisyonunu ayarlar ve LineRenderer'� ba�lat�r.
    /// </summary>
    /// <param name="startPosition">Okun d�nya koordinatlar�ndaki ba�lang�� pozisyonu.</param>
    public void SetupArrow(Vector3 startPosition)
    {
        this.startPosition = startPosition;
        // LineRenderer'�n ba�lang�� noktas�n� ayarla.
        lineRenderer.SetPosition(0, startPosition);
        // LineRenderer'�n biti� noktas�n� mevcut mouse konumuna ayarla.
        lineRenderer.SetPosition(1, MouseUtil.GetMousePositionInWorldSpace());
    }
}
