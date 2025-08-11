using DG.Tweening;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSystem : Singleton<CardSystem>
{   // Oyundaki kart �ekme, kart atma ve eldeki kartlar� y�netme i�lemlerini y�neten ana sistem.
    [SerializeField] private HandView handView;                 // Kartlar�n g�rsel olarak tutuldu�u el g�r�n�m�
    [SerializeField] private Transform drawPilePoint;           // Kart �ekme animasyonunun ba�lad��� konum (sadece transform)
    [SerializeField] private Transform discardPilePoint;        // Kart atma animasyonunun hedef konumu (sadece transform)

    private readonly List<Card> drawPile = new List<Card>();    // �ekilecek kartlar�n bulundu�u deste
    private readonly List<Card> discardPile = new List<Card>(); // At�lan(�skartaya ��kar�lan) kartlar�n bulundu�u deste
    private readonly List<Card> hand = new List<Card>();        // Oyuncunun elindeki kartlar

    void OnEnable()
    {   // Sistemin aktif oldu�u anda gerekli performer ve reaction aboneliklerini yapar
        Debug.Log("CardSystem OnEnable");
        ActionSystem.AttachPerformer<DrawCardsGA>(DrawCardsPerformer);
        ActionSystem.AttachPerformer<DiscardAllCardsGA>(DiscardAllCardsPerformer);
        ActionSystem.AttachPerformer<PlayCardGA>(PlayCardPerformer);
        ActionSystem.SubscribeReaction<EnemyTurnGA>(EnemyTurnPreReaction, ReactionTiming.PRE);
        ActionSystem.SubscribeReaction<EnemyTurnGA>(EnemyTurnPostReaction, ReactionTiming.POST);
    }
    void OnDisable()
    {   // Sistem devre d��� oldu�unda abonelikleri kald�r�r
        ActionSystem.DetachPerformer<DrawCardsGA>();
        ActionSystem.DetachPerformer<DiscardAllCardsGA>();
        ActionSystem.DetachPerformer<PlayCardGA>();
        ActionSystem.UnsubscribeReaction<EnemyTurnGA>(EnemyTurnPreReaction, ReactionTiming.PRE);
        ActionSystem.UnsubscribeReaction<EnemyTurnGA>(EnemyTurnPostReaction, ReactionTiming.POST);
    }

    // -------------------------- Publics -----------------------------
    public void Setup(List<CardData> deckData)
    {   // Deste kurulumunu yapar, d��ar�dan gelen kart verileriyle drawPile'� doldurur
        foreach (var cardData in deckData)
        {
            Card card = new(cardData);
            drawPile.Add(card);
        }
    }

    // -------------------------- Performers --------------------------

    private IEnumerator DrawCardsPerformer(DrawCardsGA drawCardsGA)
    {   // Kart �ekme aksiyonu ger�ekle�ti�inde �a�r�l�r
        int actualAmount = Mathf.Min(drawCardsGA.Amount, drawPile.Count);   // �ekilebilecek ger�ek kart say�s�
        int notDrawnAmount = drawCardsGA.Amount - actualAmount;             // �ekilemeyen kart say�s�
        for (int i = 0; i < actualAmount; i++)
        {   // Her kart i�in �ekme i�lemi
            yield return DrawCard(); 
        }
        if(notDrawnAmount > 0)
        {
            Debug.LogWarning($"�ekmek i�in kart kalmad�. �stenilen: {drawCardsGA.Amount}, Kalan: {drawPile.Count}. Sadece {actualAmount} adet kart �ekilecek.");
            RefillDeck(); // Deste bo�ald�ysa, �skartadaki kartlar� tekrar deste olarak kullan
        }
    }

    private IEnumerator DiscardAllCardsPerformer(DiscardAllCardsGA discardAllCardsGA)
    {   // Elindeki t�m kartlar� discard etme aksiyonu ger�ekle�ti�inde �a�r�l�r
        foreach (var card in hand)
        {
            CardView cardView = handView.RemoveCard(card);  // Kart� discard destesine ekle
            yield return DiscardCard(cardView);             // Kart�n g�rselini �skartaya ta��(animasyonla)
        }
        hand.Clear();                                   // Eldeki kartlar� temizle
    }

    private IEnumerator PlayCardPerformer(PlayCardGA playCardGA)
    {   // Kart oynama aksiyonu ger�ekle�ti�inde �a�r�l�r
        hand.Remove(playCardGA.Card);                             // Elden kart� ��kar
        CardView cardView = handView.RemoveCard(playCardGA.Card); // Kart�n g�rselini eldeki kartlardan ��kar
        yield return DiscardCard(cardView);                       // Kart�n g�rselini �skartaya ta��(animasyonla)

        // Efektler veya di�er i�lemler burada
        SpendManaGA spendManaGA = new SpendManaGA(playCardGA.Card.Mana); // Kart�n mana bedelini harca
        ActionSystem.Instance.AddReaction(spendManaGA);                  // Mana harcama aksiyonunu ekle

        foreach (var effectWrapper in playCardGA.Card.OtherEffects)
        {   // Kart�n efektlerini uygula
            List<CombatantView> targets = effectWrapper.TargetMode.GetTargets(); // Hedefleri al
            PerformEffectGA performEffectGA = new PerformEffectGA(effectWrapper.Effect, targets);
            ActionSystem.Instance.AddReaction(performEffectGA);
        }
    }

    // -------------------------- Reactions --------------------------

    private void EnemyTurnPreReaction(EnemyTurnGA enemyTurnGA)
    {   // D��man turu ba�lamadan �nce elindeki t�m kartlar� discard etmek i�in reaction ekler
        DiscardAllCardsGA discardAllCardsGA = new DiscardAllCardsGA();
        ActionSystem.Instance.AddReaction(discardAllCardsGA);
    }
    private void EnemyTurnPostReaction(EnemyTurnGA enemyTurnGA)
    {    // D��man turu bittikten sonra 5 kart �ekmek i�in reaction ekler
        Debug.Log("EnemyTurnPostReaction tetiklendi");
        if (drawPile.Count > 0)
        {
            DrawCardsGA drawCardsGA = new DrawCardsGA(5);
            ActionSystem.Instance.AddReaction(drawCardsGA);
        }
    }

    // -------------------------- Helpers --------------------------

    private IEnumerator DrawCard()
    {   // Tek bir kart �ekme i�lemini ve animasyonunu y�netir
        Card card = drawPile.Draw();                                                                                        // Desteden bir kart �ek
        hand.Add(card);                                                                                                     // Ele ekle
        CardView cardView = CardViewCreator.Instance.CreateCardView(card, drawPilePoint.position, drawPilePoint.rotation);  // Kart�n g�rselini olu�tur
        yield return handView.AddCard(cardView);                                                                            // G�rseli elde g�ster
    }

    private void RefillDeck()
    {   // Deste bitti�inde discard(�skarta) destesiyle doldurur
        drawPile.AddRange(discardPile);
        discardPile.Clear();
    }

    private IEnumerator DiscardCard(CardView cardView)
    {   // Kart� discard destesine atma animasyonunu oynat�r ve g�rseli yok eder
        discardPile.Add(cardView.Card);                                         // Kart� �skarta destesine ekle
        cardView.transform.DOScale(Vector3.zero, 0.15f);                            // Kart� k���lt
        Tween tween = cardView.transform.DOMove(discardPilePoint.position, 0.15f);  // Kart� discard noktas�na ta��(sa� a�a��da)
        yield return tween.WaitForCompletion();                                     // Animasyonun bitmesini bekle
        Destroy(cardView.gameObject);                                               // Kart g�rselini yok et
    }
}