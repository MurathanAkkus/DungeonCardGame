using System.Collections.Generic;
using UnityEngine;

public abstract class GameAction
{
    // Eylem ger�ekle�meden �nce tetiklenecek tepkiler
    public List<GameAction> PreReactions { get; private set; } = new ();
    // Eylem ger�ekle�tirilirken tetiklenecek tepkiler
    public List<GameAction> PerformReactions { get; private set; } = new();
    // Eylem ger�ekle�tikten sonra tetiklenecek tepkiler
    public List<GameAction> PostReactions { get; private set; } = new ();
}