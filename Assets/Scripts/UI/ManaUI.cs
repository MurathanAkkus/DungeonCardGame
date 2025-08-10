using TMPro;
using UnityEngine;

public class ManaUI : MonoBehaviour
{
    [SerializeField] private TMP_Text manaText;
    public void UpdateManaText(int currentMana)
    {
        if (manaText != null)
        {
            manaText.text = currentMana.ToString();
        }
        else
        {
            Debug.LogWarning("ManaUI scriptinde manaText bileþeni atanmamýþtýr.");
        }
    }
}
