using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private string Name;
    private Deck deck;
    private List<Card> cards = new List<Card>();
    public bool isAI { get; private set; }
    public string getName() { return Name; }
    public void AddCard(Card card)
    {
        cards.Add(card);
        //moving card to the player hand          
    }
    public void RemoveCard(Card card)
    {
        cards.Remove(card);
    }
    private void printDeck()
    {
        foreach (var card in cards)
        {
            Debug.Log($"{Name}: {card.getType()} {card.getValue()}");
        }
    }
    
    private void Start()
    {
        deck = FindObjectOfType<Deck>();
        deck.TakeCards(this);       
        if (!isAI)
        {                     
            foreach (var card in cards)
            {
                card.setFront();
            }
        }       
        printDeck();
    }   
}
