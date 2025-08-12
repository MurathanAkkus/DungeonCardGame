using DG.Tweening;
using TMPro;
using UnityEngine;

public class ManaUI : MonoBehaviour
{
    [SerializeField] private TMP_Text manaText;

    public void UpdateManaText(int currentMana)
    {
        manaText.text = currentMana.ToString();
    }

    public void ShowNotEnoughManaEffect()
    {
        if (manaText == null) return;

        // Daha �nceki animasyonlar� iptal et
        manaText.rectTransform.DOKill();
        manaText.DOKill();

        // Shake
        manaText.rectTransform.DOShakeScale(
            duration: 0.5f,
            strength: 0.2f,
            vibrato: 10,
            randomness: 90f,
            randomnessMode: ShakeRandomnessMode.Full
        );

        // K�rm�z� yan�p s�nme
        Color originalColor = manaText.color;
        manaText.DOColor(Color.red, 0.25f)
                .SetLoops(4, LoopType.Yoyo) // 1 saniye yan�p s�nme
                .OnComplete(() => manaText.color = originalColor);
    }
}
