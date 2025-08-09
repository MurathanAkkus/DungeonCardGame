using System.Collections.Generic;
using UnityEngine;

public class Card
{
    public string Title => _data.name;
    public string Description => _data.Description;
    public Sprite Image => _data.Image;
    public List<Effect> Effects => _data.Effects;
    public int Mana { get; private set; }

    private readonly CardData _data;
    public Card(CardData cardData)
    {
        _data = cardData;
        Mana = cardData.Mana;
    }
}
