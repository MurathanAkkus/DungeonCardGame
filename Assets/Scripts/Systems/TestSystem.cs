using System.Collections.Generic;
using UnityEngine;

public class TestSystem : MonoBehaviour
{
    [SerializeField] private List<CardData> deckData;

    private void Start()
    {
        CardSystem.Instance.Setup(deckData);
    }

    //[SerializeField] private HandView handView;
    //[SerializeField] private CardData cardData;
    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Space))
    //    {
    //        Card card = new Card(cardData);
    //        CardView cardView = CardViewCreator.Instance.CreateCardView(card, transform.position, Quaternion.identity);
    //        StartCoroutine(handView.AddCard(cardView));
    //    }
    //}
}
