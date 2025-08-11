using DG.Tweening;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSystem : Singleton<CardSystem>
{   // Oyundaki kart çekme, kart atma ve eldeki kartlarý yönetme iþlemlerini yöneten ana sistem.
    [SerializeField] private HandView handView;                 // Kartlarýn görsel olarak tutulduðu el görünümü
    [SerializeField] private Transform drawPilePoint;           // Kart çekme animasyonunun baþladýðý konum (sadece transform)
    [SerializeField] private Transform discardPilePoint;        // Kart atma animasyonunun hedef konumu (sadece transform)

    private readonly List<Card> drawPile = new List<Card>();    // Çekilecek kartlarýn bulunduðu deste
    private readonly List<Card> discardPile = new List<Card>(); // Atýlan(ýskartaya çýkarýlan) kartlarýn bulunduðu deste
    private readonly List<Card> hand = new List<Card>();        // Oyuncunun elindeki kartlar

    void OnEnable()
    {   // Sistemin aktif olduðu anda gerekli performer ve reaction aboneliklerini yapar
        Debug.Log("CardSystem OnEnable");
        ActionSystem.AttachPerformer<DrawCardsGA>(DrawCardsPerformer);
        ActionSystem.AttachPerformer<DiscardAllCardsGA>(DiscardAllCardsPerformer);
        ActionSystem.AttachPerformer<PlayCardGA>(PlayCardPerformer);
        ActionSystem.SubscribeReaction<EnemyTurnGA>(EnemyTurnPreReaction, ReactionTiming.PRE);
        ActionSystem.SubscribeReaction<EnemyTurnGA>(EnemyTurnPostReaction, ReactionTiming.POST);
    }
    void OnDisable()
    {   // Sistem devre dýþý olduðunda abonelikleri kaldýrýr
        ActionSystem.DetachPerformer<DrawCardsGA>();
        ActionSystem.DetachPerformer<DiscardAllCardsGA>();
        ActionSystem.DetachPerformer<PlayCardGA>();
        ActionSystem.UnsubscribeReaction<EnemyTurnGA>(EnemyTurnPreReaction, ReactionTiming.PRE);
        ActionSystem.UnsubscribeReaction<EnemyTurnGA>(EnemyTurnPostReaction, ReactionTiming.POST);
    }

    // -------------------------- Publics -----------------------------
    public void Setup(List<CardData> deckData)
    {   // Deste kurulumunu yapar, dýþarýdan gelen kart verileriyle drawPile'ý doldurur
        foreach (var cardData in deckData)
        {
            Card card = new(cardData);
            drawPile.Add(card);
        }
    }

    // -------------------------- Performers --------------------------

    private IEnumerator DrawCardsPerformer(DrawCardsGA drawCardsGA)
    {   // Kart çekme aksiyonu gerçekleþtiðinde çaðrýlýr
        int actualAmount = Mathf.Min(drawCardsGA.Amount, drawPile.Count);   // Çekilebilecek gerçek kart sayýsý
        int notDrawnAmount = drawCardsGA.Amount - actualAmount;             // Çekilemeyen kart sayýsý
        for (int i = 0; i < actualAmount; i++)
        {   // Her kart için çekme iþlemi
            yield return DrawCard(); 
        }
        if(notDrawnAmount > 0)
        {
            Debug.LogWarning($"Çekmek için kart kalmadý. Ýstenilen: {drawCardsGA.Amount}, Kalan: {drawPile.Count}. Sadece {actualAmount} adet kart çekilecek.");
            RefillDeck(); // Deste boþaldýysa, ýskartadaki kartlarý tekrar deste olarak kullan
        }
    }

    private IEnumerator DiscardAllCardsPerformer(DiscardAllCardsGA discardAllCardsGA)
    {   // Elindeki tüm kartlarý discard etme aksiyonu gerçekleþtiðinde çaðrýlýr
        foreach (var card in hand)
        {
            CardView cardView = handView.RemoveCard(card);  // Kartý discard destesine ekle
            yield return DiscardCard(cardView);             // Kartýn görselini ýskartaya taþý(animasyonla)
        }
        hand.Clear();                                   // Eldeki kartlarý temizle
    }

    private IEnumerator PlayCardPerformer(PlayCardGA playCardGA)
    {   // Kart oynama aksiyonu gerçekleþtiðinde çaðrýlýr
        hand.Remove(playCardGA.Card);                             // Elden kartý çýkar
        CardView cardView = handView.RemoveCard(playCardGA.Card); // Kartýn görselini eldeki kartlardan çýkar
        yield return DiscardCard(cardView);                       // Kartýn görselini ýskartaya taþý(animasyonla)

        // Efektler veya diðer iþlemler burada
        SpendManaGA spendManaGA = new SpendManaGA(playCardGA.Card.Mana); // Kartýn mana bedelini harca
        ActionSystem.Instance.AddReaction(spendManaGA);                  // Mana harcama aksiyonunu ekle

        foreach (var effectWrapper in playCardGA.Card.OtherEffects)
        {   // Kartýn efektlerini uygula
            List<CombatantView> targets = effectWrapper.TargetMode.GetTargets(); // Hedefleri al
            PerformEffectGA performEffectGA = new PerformEffectGA(effectWrapper.Effect, targets);
            ActionSystem.Instance.AddReaction(performEffectGA);
        }
    }

    // -------------------------- Reactions --------------------------

    private void EnemyTurnPreReaction(EnemyTurnGA enemyTurnGA)
    {   // Düþman turu baþlamadan önce elindeki tüm kartlarý discard etmek için reaction ekler
        DiscardAllCardsGA discardAllCardsGA = new DiscardAllCardsGA();
        ActionSystem.Instance.AddReaction(discardAllCardsGA);
    }
    private void EnemyTurnPostReaction(EnemyTurnGA enemyTurnGA)
    {    // Düþman turu bittikten sonra 5 kart çekmek için reaction ekler
        Debug.Log("EnemyTurnPostReaction tetiklendi");
        if (drawPile.Count > 0)
        {
            DrawCardsGA drawCardsGA = new DrawCardsGA(5);
            ActionSystem.Instance.AddReaction(drawCardsGA);
        }
    }

    // -------------------------- Helpers --------------------------

    private IEnumerator DrawCard()
    {   // Tek bir kart çekme iþlemini ve animasyonunu yönetir
        Card card = drawPile.Draw();                                                                                        // Desteden bir kart çek
        hand.Add(card);                                                                                                     // Ele ekle
        CardView cardView = CardViewCreator.Instance.CreateCardView(card, drawPilePoint.position, drawPilePoint.rotation);  // Kartýn görselini oluþtur
        yield return handView.AddCard(cardView);                                                                            // Görseli elde göster
    }

    private void RefillDeck()
    {   // Deste bittiðinde discard(ýskarta) destesiyle doldurur
        drawPile.AddRange(discardPile);
        discardPile.Clear();
    }

    private IEnumerator DiscardCard(CardView cardView)
    {   // Kartý discard destesine atma animasyonunu oynatýr ve görseli yok eder
        discardPile.Add(cardView.Card);                                         // Kartý ýskarta destesine ekle
        cardView.transform.DOScale(Vector3.zero, 0.15f);                            // Kartý küçült
        Tween tween = cardView.transform.DOMove(discardPilePoint.position, 0.15f);  // Kartý discard noktasýna taþý(sað aþaðýda)
        yield return tween.WaitForCompletion();                                     // Animasyonun bitmesini bekle
        Destroy(cardView.gameObject);                                               // Kart görselini yok et
    }
}