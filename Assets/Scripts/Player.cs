using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{       
    [SerializeField] private string Name;
    private Deck deck;
    private Table table;
    private List<Card> cards = new List<Card>();
    public int getMinTrumpValue()
    {
        int minValue = 15;
        foreach (var card in cards)
        {
            if(card.getType() == deck.Trump && card.getValue() < minValue)
            {
                minValue = card.getValue();
            }
        }
        return minValue;
    }
    public int getCardsCount() { return cards.Count; }
    public List<Card> getCards() { return cards; }
    public bool hasCards() 
    {
        if (cards.Count > 0) return true;       
        else return false;           
    }
    [SerializeField] private bool isAI;
    public bool isBot() { return isAI; }
    public string getName() { return Name; }
    public void AddCard(Card card)
    {
        cards.Add(card.TakeCard(this));
        table.setPlCardsPos(this);
        if (!isAI) card.setFront();        
    }
    public void RemoveCard(Card card) { cards.Remove(card); }         
    private void Awake()
    {
        table = FindObjectOfType<Table>();
        deck = FindObjectOfType<Deck>();                    
    }   

    public void TakeCards()
    {
        deck.TakeCards(this);
        table.setPlCardsPos(this);
        if (!isAI)
        {
            foreach (var card in cards)
            {
                card.setFront();
                card.setLayer(1);
            }
        }
    }
}
