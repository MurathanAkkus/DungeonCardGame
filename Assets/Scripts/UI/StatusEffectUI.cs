using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Tek bir durum efekti kullanýcý arayüzü öðesini temsil eder
public class StatusEffectUI : MonoBehaviour 
{
    [SerializeField] private Image image;
    [SerializeField] private TMP_Text stackCountText;

    public void Set(Sprite sprite, int stackCount)
    {
        image.sprite = sprite;
        image.enabled = sprite != null;
        if (stackCount > 1)
        {
            stackCountText.text = stackCount.ToString();
            stackCountText.enabled = true;
        }
        else
        {
            stackCountText.enabled = false;
        }
    }
}
