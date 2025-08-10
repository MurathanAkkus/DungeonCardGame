using TMPro;
using UnityEngine;

public class EnemyView : CombatantView
{
    [SerializeField] private TMP_Text attackText;

    public int AttackPower { get; set; }

    public void Setup()
    {
        AttackPower = 10;
        UpdateAttackText();
        SetupBase(AttackPower, null);
        // Image i�in null varsay�larak, gerekirse ger�ek sprite ile de�i�tirilir
    }

    private void UpdateAttackText()
    {
        attackText.text = $"ATK: {AttackPower}";
    }
}
