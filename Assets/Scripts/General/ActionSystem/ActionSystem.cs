using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Bir eylem tetiklendiðinde, öncesinde, sýrasýnda ve
// sonrasýnda baþka eylemler veya fonksiyonlar zincirleme þekilde çalýþtýrýlabilir.

public class ActionSystem : Singleton<ActionSystem>
{
    private List<GameAction> reactions = null; // O anda iþlenen tepki listesi
    public ActionState State { get; private set; } = ActionState.Idle; // Sistemin mevcut durumu (Idle, Performing, Completed, Error)
    private static Dictionary<Type, List<Action<GameAction>>> preSubs = new();
    private static Dictionary<Type, Func<GameAction, IEnumerator>> performers = new();
    private static Dictionary<Type, List<Action<GameAction>>> postSubs = new();

    public void Perform(GameAction action, Action OnPerformFinished = null)
    {   // Bir eylemi baþladýðýnda
        if (State == ActionState.Performing)
        {
            Debug.LogWarning("ActionSystem zaten bir aksiyonu perform ediyor!");
            return;
        }
        State = ActionState.Performing;
        StartCoroutine(Flow(action, () =>
        {
            State = ActionState.Completed;  // Akýþ tamamlanýnca Completed olarak ayarla
            OnPerformFinished?.Invoke();    // Dýþarýdan verilen callback'i çaðýr
            State = ActionState.Idle;       // Sonra tekrar Idle'a dön
        }));
    }

    public void AddReaction(GameAction gameAction)
    {   // O anda iþlenen tepki listesine yeni bir tepki ekler
        reactions?.Add(gameAction);
    }

    private IEnumerator Flow(GameAction action, Action OnFlowFinished = null)
    {   // Bir eylemin tüm akýþý sýrasýyla iþler (ön, ana, son tepkiler)
        reactions = action.PreReactions;       // Ön tepkileri al
        PerformSubscribers(action, preSubs);   // Ön tepkilere abone olan fonksiyonlarý çalýþtýr
        yield return PerformReactions();       // Ön tepkileri zincirleme iþle

        reactions = action.PerformReactions;   // Ana tepkileri al
        yield return PerformPerformer(action); // Ana iþlevi (performer) çalýþtýr
        yield return PerformReactions();       // Ana tepkileri zincirleme iþle

        reactions = action.PostReactions;      // Son tepkileri al
        PerformSubscribers(action, postSubs);  // Son tepkilere abone olan fonksiyonlarý çalýþtýr
        yield return PerformReactions();       // Son tepkileri zincirleme iþle

        OnFlowFinished?.Invoke();              // Akýþ tamamlanýnca callback'i çaðýr
    }

    private IEnumerator PerformPerformer(GameAction action)
    {   // Action tipine atanmýþ performer fonksiyonunu çalýþtýrýr
        Type type = action.GetType();
        if (performers.ContainsKey(type))
        {
            yield return performers[type](action);
        }
        else
        {
            Debug.LogWarning($"Action type a gore bir fonksiyon bulunamadý {type}. Action gerceklesmeyecek!"); // Yoksa uyarý ver
        }
    }
    
    private void PerformSubscribers(GameAction action, Dictionary<Type, List<Action<GameAction>>> subscribers)
    {   // Belirli bir action tipine abone olan fonksiyonlarý çalýþtýrýr
        Type type = action.GetType();
        if (subscribers.ContainsKey(type))
        {
            foreach (var subscriber in subscribers[type])
            {    // Hepsini sýrayla çalýþtýr
                subscriber(action);
            }
        }
    }

    private IEnumerator PerformReactions()
    {   // O anki tepki listesindeki her bir tepkiyi zincirleme þekilde iþler
        foreach (var reaction in reactions)
        {
            yield return Flow(reaction); // Tekrar Flow baþlat (iç içe akýþ)
        }
    }

    public static void AttachPerformer<T>(Func<T, IEnumerator> performer) where T : GameAction
    {   // Belirli bir action tipine performer fonksiyonu ekler
        Type type = typeof(T);
        // Sistemde tüm performer'lar tek tipte saklanýrken,
        // her action'ýn kendi tipine özel fonksiyonu çalýþtýrýlýr.
        IEnumerator wrappedPerformer(GameAction action) => performer((T)action);
        if (performers.ContainsKey(type))
        {
            Debug.LogWarning($"{type} için zaten performer var. Üstüne yazýlýyor!");
            performers[type] = wrappedPerformer;
        }
        else
        {
            performers.Add(type, wrappedPerformer);
        }
    }

    public static void DetachPerformer<T>() where T : GameAction
    {   // Belirli bir action tipinden performer fonksiyonunu kaldýrýr
        Type type = typeof(T);
        if (performers.ContainsKey(type))
        {
            performers.Remove(type);
        }
    }

    public static void SubscribeReaction<T>(Action<T> reaction, ReactionTiming timing) where T : GameAction
    {   // Belirli bir action tipine ve zamanlamaya (PRE/POST) reaction fonksiyonu ekler
        Dictionary<Type, List<Action<GameAction>>> subs = timing == ReactionTiming.PRE ? preSubs : postSubs; // Zamanlamaya göre doðru sözlüðü seç
        void wrappedReaction(GameAction action) => reaction((T)action);

        if (subs.ContainsKey(typeof(T)))
        {
            subs[typeof(T)].Add(wrappedReaction);
        }
        else
        {
            subs.Add(typeof(T), new List<Action<GameAction>> { wrappedReaction }); // Yoksa yeni liste oluþtur
        }
    }

    public static void UnsubscribeReaction<T>(Action<T> reaction, ReactionTiming timing) where T : GameAction
    {   // Belirli bir action tipinden ve zamanlamadan tepki fonksiyonunu kaldýrýr
        Dictionary<Type, List<Action<GameAction>>> subs = timing == ReactionTiming.PRE ? preSubs : postSubs;
        if (subs.ContainsKey(typeof(T)))
        {
            void wrappedReaction(GameAction action) => reaction((T)action);
            subs[typeof(T)].Remove(wrappedReaction);
        }
    }
}