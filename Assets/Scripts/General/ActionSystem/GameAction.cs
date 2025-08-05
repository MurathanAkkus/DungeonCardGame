using System.Collections.Generic;
using UnityEngine;

public abstract class GameAction
{
    // Eylem gerçekleþmeden önce tetiklenecek tepkiler
    public List<GameAction> PreReactions { get; private set; } = new ();
    // Eylem gerçekleþtirilirken tetiklenecek tepkiler
    public List<GameAction> PerformReactions { get; private set; } = new();
    // Eylem gerçekleþtikten sonra tetiklenecek tepkiler
    public List<GameAction> PostReactions { get; private set; } = new ();
}