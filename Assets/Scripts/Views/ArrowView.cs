using UnityEngine;

/// <summary>
/// Oku ve ok çizgisini görsel olarak yöneten sýnýf.
/// Mouse konumuna göre ok baþý ve yönü güncellenir.
/// </summary>
public class ArrowView : MonoBehaviour
{
    [SerializeField] private GameObject arrowHead;
    [SerializeField] private LineRenderer lineRenderer; // Ok çizgisi için

    private Vector3 startPosition;

    /// <summary>
    /// Her karede ok baþý ve yönünü mouse konumuna göre günceller.
    /// </summary>
    private void Update()
    {
        // Mouse'un dünya koordinatlarýndaki konumunu al.
        Vector3 endPosition = MouseUtil.GetMousePositionInWorldSpace();
        // Baþlangýç pozisyonundan ok baþýna olan yönü hesapla ve normalize et.
        Vector3 direction = -(startPosition - arrowHead.transform.position).normalized;
        // Ok çizgisinin ucunu, ok baþý ile çakýþmamasý için ok baþýnýn yönünde 0.5 birim geriye alýr.
        lineRenderer.SetPosition(1, endPosition - direction * 0.5f);
        // Ok baþýný mouse konumuna taþý.
        arrowHead.transform.position = endPosition;
        // Ok baþýnýn yönünü ayarla.
        arrowHead.transform.right = direction;
    }


    /// <summary>
    /// Okun baþlangýç pozisyonunu ayarlar ve LineRenderer'ý baþlatýr.
    /// </summary>
    /// <param name="startPosition">Okun dünya koordinatlarýndaki baþlangýç pozisyonu.</param>
    public void SetupArrow(Vector3 startPosition)
    {
        this.startPosition = startPosition;
        // LineRenderer'ýn baþlangýç noktasýný ayarla.
        lineRenderer.SetPosition(0, startPosition);
        // LineRenderer'ýn bitiþ noktasýný mevcut mouse konumuna ayarla.
        lineRenderer.SetPosition(1, MouseUtil.GetMousePositionInWorldSpace());
    }
}
