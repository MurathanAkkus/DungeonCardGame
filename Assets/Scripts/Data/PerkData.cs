using SerializeReferenceEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Perk")]
public class PerkData : ScriptableObject
{
    [field: SerializeField] public Sprite Image { get; private set; }                               // G�rsel temsil
    [field: SerializeReference, SR] public PerkCondition PerkCondition { get; private set; }        // Perk'in tetiklenme ko�ulu
    [field: SerializeReference, SR] public AutoTargetEffect AutoTargetEffect { get; private set; }  // Otomatik hedefleme etkisi
    [field: SerializeField] public bool UseAutoTarget { get; private set; } = true;                 // Otomatik hedefleme kullan�ls�n m�?
    [field: SerializeField] public bool UseActionCasterAsTarget { get; private set; } = false;      // Eylem yapan�n hedef olarak kullan�lmas�
}
