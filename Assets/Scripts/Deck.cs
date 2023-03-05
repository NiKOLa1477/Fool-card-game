using System.Collections;
using System.Security.Cryptography;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Deck : MonoBehaviour
{
    [SerializeField] private List<Card> cards;
    public CardType Trump { get; private set; }
    private void TossTheDeck()
    {
        RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
        int n = cards.Count;
        while (n > 1)
        {
            byte[] box = new byte[1];
            do provider.GetBytes(box);
            while (!(box[0] < n * (Byte.MaxValue / n)));
            int k = (box[0] % n);
            n--;
            Card value = cards[k];
            cards[k] = cards[n];
            cards[n] = value;
        }
    }
    private void TurnTrumpCard()
    {
        cards[cards.Count - 1].setLayer(-1);
        cards[cards.Count - 1].TurnCard();
        cards[cards.Count - 1].transform.position -= new Vector3(0.4f, 0, 0);
        cards[cards.Count - 1].setFront();       
    }
    public void TakeCards(Player pl, int count = 6)
    {
        for (int i = 0; i < cards.Count; i++)
        {
            if (cards[i].inDeck && count > 0)
            {
                Card takenCard = cards[i].TakeCard(pl);
                if (i == cards.Count - 1)
                {
                    cards[cards.Count - 1].TurnCard();
                    cards[cards.Count - 1].setBack();
                }
                pl.AddCard(takenCard);
                count--;
            }
        }
    }
    private void Awake()
    {
        TossTheDeck();
        Trump=cards[cards.Count-1].getType();
        TurnTrumpCard();
    }
}
