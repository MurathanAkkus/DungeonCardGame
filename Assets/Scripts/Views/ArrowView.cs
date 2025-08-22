using System.Collections;
using UnityEngine;

/// <summary>
/// Oku ve çizgisini mouse konumuna göre yöneten görünüm.
/// Update() yerine yalnızca gerektiğinde çalışan bir korutin kullanır.
/// </summary>

[RequireComponent(typeof(LineRenderer))]
public class ArrowView : MonoBehaviour
{
    [SerializeField] private GameObject arrowHead;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private float headOffset = 0.5f;        // ok başını çizgiden biraz geri çeker
    [SerializeField] private float minMoveThreshold = 0.01f; // mouse hareket eşiği (world units)

    private Transform headT;
    private Vector3 startPosition;
    private Vector3 lastEndPos = new Vector3(float.NaN, float.NaN, float.NaN);

    private Coroutine trackCo;
    private bool isTracking;
    private bool trackRequested;

    void Awake() => EnsureInit();
    void OnValidate() => EnsureInit();
    void OnDisable() => StopTracking(); // güvenlik

    private void EnsureInit()
    {
        // arrowHead atanmışsa headT'yi doldur
        if (arrowHead != null && headT == null)
            headT = arrowHead.transform;

        // LineRenderer inspector’da boşsa aynı objeden çek
        if (lineRenderer == null)
            lineRenderer = GetComponent<LineRenderer>();
    }

    /// <summary>
    /// Okun başlangıç pozisyonunu ayarlar ve istersen takibi başlatır.
    /// </summary>
    public void SetupArrow(Vector3 startPosition, bool startTracking = true)
    {
        EnsureInit();

        if (headT == null || lineRenderer == null)
        {
            Debug.LogError("[ArrowView] Missing references (arrowHead/lineRenderer).", this);
            enabled = false;
            return;
        }

        this.startPosition = startPosition;

        // Çizginin başlangıcını sabitle
        lineRenderer.SetPosition(0, startPosition);

        // İlk görsel güncelleme
        Vector3 endPos = MouseUtil.GetMousePositionInWorldSpace();
        UpdateVisual(endPos, force: true);

        if (startTracking)
            StartTracking();
    }

    /// <summary>Takibi başlat (sadece gerektiğinde çalışır).</summary>
    public void StartTracking()
    {
        trackRequested = true;                 // istek kaydı
        if (!isActiveAndEnabled) return;       // obje aktifleşince başlayacağız

        if (isTracking) return;
        isTracking = true;
        trackCo = StartCoroutine(TrackLoop());
    }

    /// <summary>Takibi durdur.</summary>
    public void StopTracking()
    {
        trackRequested = false;
        if (!isTracking) return;
        isTracking = false;

        if (trackCo != null)
        {
            StopCoroutine(trackCo);
            trackCo = null;
        }
    }

    private IEnumerator TrackLoop()
    {
        // her frame çalışsın istiyorsan:
        var wait = new WaitForEndOfFrame();
        // 50 Hz için: var wait = new WaitForSeconds(0.02f);

        while (isTracking)
        {
            Vector3 endPos = MouseUtil.GetMousePositionInWorldSpace();

            // Küçük hareketleri es geç (gereksiz hesap yok)
            if ((endPos - lastEndPos).sqrMagnitude >= (minMoveThreshold * minMoveThreshold))
                UpdateVisual(endPos);

            yield return wait;
        }
    }

    private void UpdateVisual(Vector3 endPos, bool force = false)
    {
        EnsureInit(); // ekstra güvenlik: başlatılmamışsa başlat

        if (headT == null || lineRenderer == null) return;

        lastEndPos = endPos;

        // Ok başını mouse'a taşı
        headT.position = endPos;

        // Yön: start -> head
        Vector3 dir = headT.position - startPosition;
        if (dir.sqrMagnitude < 1e-6f) dir = Vector3.right;
        else dir.Normalize();

        // Ok başının yönü
        headT.right = dir;

        // Çizgi ucu, ok başının biraz gerisi
        lineRenderer.SetPosition(1, endPos - dir * headOffset);
    }

    // İsteğe bağlı: okun görünürlüğünü merkezi yerden kontrol etmek için
    public void Show(Vector3 startPos)
    {
        gameObject.SetActive(true);
        SetupArrow(startPos, startTracking: true);
    }

    public void Hide()
    {
        StopTracking();
        gameObject.SetActive(false);
    }
}