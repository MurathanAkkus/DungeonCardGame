using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Bir eylem tetiklendi�inde, �ncesinde, s�ras�nda ve
// sonras�nda ba�ka eylemler veya fonksiyonlar zincirleme �ekilde �al��t�r�labilir.

public class ActionSystem : Singleton<ActionSystem>
{
    private List<GameAction> reactions = null; // O anda i�lenen tepki listesi
    public ActionState State { get; private set; } = ActionState.Idle; // Sistemin mevcut durumu (Idle, Performing, Completed, Error)
    private static Dictionary<Type, List<Action<GameAction>>> preSubs = new();
    private static Dictionary<Type, Func<GameAction, IEnumerator>> performers = new();
    private static Dictionary<Type, List<Action<GameAction>>> postSubs = new();

    public void Perform(GameAction action, Action OnPerformFinished = null)
    {   // Bir eylemi ba�lad���nda
        if (State == ActionState.Performing)
        {
            Debug.LogWarning("ActionSystem zaten bir aksiyonu perform ediyor!");
            return;
        }
        State = ActionState.Performing;
        StartCoroutine(Flow(action, () =>
        {
            State = ActionState.Completed;  // Ak�� tamamlan�nca Completed olarak ayarla
            OnPerformFinished?.Invoke();    // D��ar�dan verilen callback'i �a��r
            State = ActionState.Idle;       // Sonra tekrar Idle'a d�n
        }));
    }

    public void AddReaction(GameAction gameAction)
    {   // O anda i�lenen tepki listesine yeni bir tepki ekler
        reactions?.Add(gameAction);
    }

    private IEnumerator Flow(GameAction action, Action OnFlowFinished = null)
    {   // Bir eylemin t�m ak��� s�ras�yla i�ler (�n, ana, son tepkiler)
        reactions = action.PreReactions;       // �n tepkileri al
        PerformSubscribers(action, preSubs);   // �n tepkilere abone olan fonksiyonlar� �al��t�r
        yield return PerformReactions();       // �n tepkileri zincirleme i�le

        reactions = action.PerformReactions;   // Ana tepkileri al
        yield return PerformPerformer(action); // Ana i�levi (performer) �al��t�r
        yield return PerformReactions();       // Ana tepkileri zincirleme i�le

        reactions = action.PostReactions;      // Son tepkileri al
        PerformSubscribers(action, postSubs);  // Son tepkilere abone olan fonksiyonlar� �al��t�r
        yield return PerformReactions();       // Son tepkileri zincirleme i�le

        OnFlowFinished?.Invoke();              // Ak�� tamamlan�nca callback'i �a��r
    }

    private IEnumerator PerformPerformer(GameAction action)
    {   // Action tipine atanm�� performer fonksiyonunu �al��t�r�r
        Type type = action.GetType();
        if (performers.ContainsKey(type))
        {
            yield return performers[type](action);
        }
        else
        {
            Debug.LogWarning($"Action type a gore bir fonksiyon bulunamad� {type}. Action gerceklesmeyecek!"); // Yoksa uyar� ver
        }
    }
    
    private void PerformSubscribers(GameAction action, Dictionary<Type, List<Action<GameAction>>> subscribers)
    {   // Belirli bir action tipine abone olan fonksiyonlar� �al��t�r�r
        Type type = action.GetType();
        if (subscribers.ContainsKey(type))
        {
            foreach (var subscriber in subscribers[type])
            {    // Hepsini s�rayla �al��t�r
                subscriber(action);
            }
        }
    }

    private IEnumerator PerformReactions()
    {   // O anki tepki listesindeki her bir tepkiyi zincirleme �ekilde i�ler
        foreach (var reaction in reactions)
        {
            yield return Flow(reaction); // Tekrar Flow ba�lat (i� i�e ak��)
        }
    }

    public static void AttachPerformer<T>(Func<T, IEnumerator> performer) where T : GameAction
    {   // Belirli bir action tipine performer fonksiyonu ekler
        Type type = typeof(T);
        // Sistemde t�m performer'lar tek tipte saklan�rken,
        // her action'�n kendi tipine �zel fonksiyonu �al��t�r�l�r.
        IEnumerator wrappedPerformer(GameAction action) => performer((T)action);
        if (performers.ContainsKey(type))
        {
            Debug.LogWarning($"{type} i�in zaten performer var. �st�ne yaz�l�yor!");
            performers[type] = wrappedPerformer;
        }
        else
        {
            performers.Add(type, wrappedPerformer);
        }
    }

    public static void DetachPerformer<T>() where T : GameAction
    {   // Belirli bir action tipinden performer fonksiyonunu kald�r�r
        Type type = typeof(T);
        if (performers.ContainsKey(type))
        {
            performers.Remove(type);
        }
    }

    public static void SubscribeReaction<T>(Action<T> reaction, ReactionTiming timing) where T : GameAction
    {   // Belirli bir action tipine ve zamanlamaya (PRE/POST) reaction fonksiyonu ekler
        Dictionary<Type, List<Action<GameAction>>> subs = timing == ReactionTiming.PRE ? preSubs : postSubs; // Zamanlamaya g�re do�ru s�zl��� se�
        void wrappedReaction(GameAction action) => reaction((T)action);

        if (subs.ContainsKey(typeof(T)))
        {
            subs[typeof(T)].Add(wrappedReaction);
        }
        else
        {
            subs.Add(typeof(T), new List<Action<GameAction>> { wrappedReaction }); // Yoksa yeni liste olu�tur
        }
    }

    public static void UnsubscribeReaction<T>(Action<T> reaction, ReactionTiming timing) where T : GameAction
    {   // Belirli bir action tipinden ve zamanlamadan tepki fonksiyonunu kald�r�r
        Dictionary<Type, List<Action<GameAction>>> subs = timing == ReactionTiming.PRE ? preSubs : postSubs;
        if (subs.ContainsKey(typeof(T)))
        {
            void wrappedReaction(GameAction action) => reaction((T)action);
            subs[typeof(T)].Remove(wrappedReaction);
        }
    }
}