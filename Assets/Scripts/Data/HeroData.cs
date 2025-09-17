using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Hero", fileName = "HeroData", order = 1)]
public class HeroData : ScriptableObject
{
    [field: SerializeField] public Sprite Image { get; private set; }
    [field: SerializeField] public int Health { get; private set; }
    [field: SerializeField] public int StartingArmor { get; private set; }
    [field: SerializeField] public int StartingStrength { get; private set; }
    [field: SerializeField] public List<CardData> Deck { get; private set; }
}
