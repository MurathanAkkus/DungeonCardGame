using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

/// <summary>
/// Elde (hand) tutulan kartların görünümünü yönetir.
/// Kart ekleme/çıkarma sonrası kartları bir Spline boyunca animasyonla konumlandırır ve döndürür.
/// </summary>
public class HandView : MonoBehaviour
{
    [Header("Layout")]
    [SerializeField] private SplineContainer splineContainer;  // Kartların dizileceği eğri
    [SerializeField] private float layoutDuration = 0.15f;     // Yerleşim animasyon süresi
    [SerializeField] private Ease ease = Ease.OutCubic;        // Animasyon eğrisi (DOTween)

    [Header("Spacing (dinamik)")]
    [SerializeField] private float spacingMin = 0.04f;         // Çok kartta minimum aralık
    [SerializeField] private float spacingMax = 0.12f;         // Az kartta maksimum aralık
    [SerializeField] private int spacingMaxCards = 12;         // Bu sayıda kartta spacingMin'e iner

    [Header("Z Offset")]
    [SerializeField] private float zOffsetPerCard = 0.01f;     // Z-fighting önlemek için hafif geri it

    // Elimizdeki kart görünümleri ve O(1) erişim için indeks
    private readonly List<CardView> cards = new();
    private readonly Dictionary<Card, CardView> index = new();

    // Aynı anda tek bir yerleşim coroutine'i çalışsın
    private Coroutine layoutRoutine;

    void OnValidate()
    {
        // Inspector’da atanmamışsa, parent’lardan bulmaya çalış
        if (splineContainer == null)
            splineContainer = GetComponentInParent<SplineContainer>();
    }

    // -------------------------- Public API --------------------------

    /// <summary>
    /// Yeni kart ekle ve yerleşimi animasyonla güncelle.
    /// </summary>
    public IEnumerator AddCard(CardView cv)
    {
        if (cv == null) yield break;

        cards.Add(cv);
        index[cv.Card] = cv;

        yield return UpdateCardPositions(layoutDuration);
    }

    /// <summary>
    /// Kartı kaldır; yerleşimi animasyonla günceller. Hemen dönmek istersen bu sürümü kullan.
    /// </summary>
    public CardView RemoveCard(Card card)
    {
        if (!index.TryGetValue(card, out var cv)) return null;
        if (!cards.Remove(cv)) return null;

        index.Remove(card);
        StartLayout(layoutDuration);  // Beklemeden animasyonu başlat
        return cv;
    }

    /// <summary>
    /// Kartı kaldır; yerleşimin bitmesini coroutine olarak beklemek istersen bu sürümü kullan.
    /// </summary>
    public IEnumerator RemoveCardCo(Card card, float duration = -1f)
    {
        if (!index.TryGetValue(card, out var cv)) yield break;
        if (!cards.Remove(cv)) yield break;

        index.Remove(card);
        if (duration < 0f) duration = layoutDuration;

        yield return UpdateCardPositions(duration);
    }

    /// <summary>
    /// Mevcut dizilimi tekrar hesaplayıp animasyonla uygular (ör. dışsal yeniden akış).
    /// </summary>
    public void RefreshLayout(float duration = -1f)
    {
        if (duration < 0f) duration = layoutDuration;
        StartLayout(duration);
    }

    // -------------------------- Internal --------------------------

    /// <summary>
    /// Aynı anda tek yerleşim çalışsın diye yardımcı başlatıcı.
    /// </summary>
    private void StartLayout(float duration)
    {
        if (layoutRoutine != null)
            StopCoroutine(layoutRoutine);

        layoutRoutine = StartCoroutine(UpdateCardPositions(duration));
    }

    /// <summary>
    /// Tüm kartları spline üzerinde düzgün aralıkla yeniden konumlandırır/çevirir.
    /// DOTween Sequence kullanır ve tamamlanmasını kesin olarak bekler.
    /// </summary>
    private IEnumerator UpdateCardPositions(float duration)
    {
        // Null kalıntıları temizle (Destroy edilmiş referanslar)
        cards.RemoveAll(c => c == null);

        if (splineContainer == null || cards.Count == 0)
        {
            layoutRoutine = null;
            yield break;
        }

        var spline = splineContainer.Spline;
        var containerTr = splineContainer.transform;

        // Kart sayısına göre aralığı dinamik belirle
        float spacing = ComputeSpacing(cards.Count);

        // 0.5 merkezli, toplam genişlik kadar sola kaydır (ortalamak için)
        float firstP = 0.5f - (cards.Count - 1) * spacing / 2f;

        // Tüm kart tweenlerini tek sequence’ta topla ve sonunda bekle
        Sequence seq = DOTween.Sequence();

        for (int i = 0; i < cards.Count; i++)
        {
            // Bu kartın spline parametresi ve güvenli aralık
            float p = Mathf.Clamp01(firstP + i * spacing);

            // Spline uzayında örnekleme
            Vector3 localPos = spline.EvaluatePosition(p);
            Vector3 localTan = spline.EvaluateTangent(p);
            Vector3 localUp = spline.EvaluateUpVector(p);

            // Dünya uzayına dönüşüm (container rot/scale dahil)
            Vector3 worldPos = containerTr.TransformPoint(localPos);
            Vector3 worldTan = containerTr.TransformDirection(localTan).normalized;
            Vector3 worldUp = containerTr.TransformDirection(localUp).normalized;

            // Yön hesaplama: -up düzlemi ve ona dik eksen
            Quaternion worldRot = Quaternion.LookRotation(-worldUp, Vector3.Cross(-worldUp, worldTan).normalized);

            var t = cards[i].transform;

            // Aynı hedefe zaten çok yakınsa tween açma
            if ((t.position - worldPos).sqrMagnitude < 0.0001f && Quaternion.Angle(t.rotation, worldRot) < 0.1f)
            {
                t.position = worldPos + (i * zOffsetPerCard) * Vector3.back;
                t.rotation = worldRot;
                continue;
            }

            // Önceki tweenleri öldür; çakışma/stacking olmasın
            t.DOKill();

            // Z-fighting önlemek için hafif geri it
            var moveTween = t.DOMove(worldPos + (i * zOffsetPerCard) * Vector3.back, duration);
            var rotTween = t.DORotateQuaternion(worldRot, duration);

            // Aynı anda çalışsınlar: sequence’a JOIN et
            seq.Join(moveTween.SetEase(ease).SetLink(t.gameObject, LinkBehaviour.KillOnDestroy));
            seq.Join(rotTween.SetEase(ease).SetLink(t.gameObject, LinkBehaviour.KillOnDestroy));
        }

        // Sequence tamamlanmasını bekle (easing/timeScale ne olursa olsun senkron)
        yield return seq.WaitForCompletion();

        layoutRoutine = null;
    }

    /// <summary>
    /// Kart sayısına bağlı spacing hesaplama.
    /// 1 kartta spacingMax, spacingMaxCards ve üzeri kartta spacingMin.
    /// </summary>
    private float ComputeSpacing(int count)
    {
        count = Mathf.Max(1, count);
        float t = Mathf.InverseLerp(1f, spacingMaxCards, Mathf.Min(count, spacingMaxCards));
        return Mathf.Lerp(spacingMax, spacingMin, t);
    }
}