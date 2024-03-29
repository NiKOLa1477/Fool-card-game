using System.Collections;
using System.Security.Cryptography;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Deck : MonoBehaviour
{
    private UIManager UI;
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
    private int CountCards()
    {
        int count = 0;
        foreach (var card in cards)
        {
            if (card.inDeck) count++;
        }
        return count;
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
        UI.UpdDeckCount(CountCards(), Trump);
    }
    public void changeTo36Cards()
    {
        for (int i = cards.Count - 1; i >= 0; i--) 
        {
            if (cards[i].getValue() < 6 || cards[i].getValue() == 15)
            {
                cards[i].gameObject.SetActive(false);
                cards.Remove(cards[i]);
            }
        }        
    }
    private void Awake()
    {
        UI = FindObjectOfType<UIManager>();
        if(PlayerPrefs.HasKey("Deck"))
        {
            int count = PlayerPrefs.GetInt("Deck");
            if(count == 36)
            {
                changeTo36Cards();
            }
        }
        TossTheDeck();
        while(cards[cards.Count - 1].getType() == CardType.Joker)
            TossTheDeck();
        Trump =cards[cards.Count-1].getType();
        TurnTrumpCard();
    }
}
