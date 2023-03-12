using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    static float R_Border, U_Border;   
    [SerializeField] private string Name;
    private Deck deck;
    private List<Card> cards = new List<Card>();
    public int getCount() { return cards.Count; }
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
        setCardsPos();
        if (!isAI) card.setFront();        
    }
    public void RemoveCard(Card card) { cards.Remove(card); }
    private void printDeck()
    {
        foreach (var card in cards)
        {
            Debug.Log($"{Name}: {card.getType()} {card.getValue()}");
        }
    }
    public void setCardsPos()
    {
        if(!isAI)
        {
            float dx = (R_Border * 2 - (R_Border / 4)) / cards.Count;
            if (dx > 1.4f) dx = 1.4f;
            float start = -R_Border + (R_Border / 10);
            for (int i = 0; i < cards.Count; i++)
            {
                cards[i].transform.position = new Vector3(start + i * dx, -U_Border + U_Border / 4, cards[i].transform.position.z);
            }
        }
        else
        {
            float dx = (R_Border * 2 - (R_Border / 4)) / cards.Count;
            if (dx > 1.4f) 
                dx = 1.4f;
            float start = -R_Border + (R_Border / 10);
            for (int i = 0; i < cards.Count; i++)
            {
                cards[i].transform.position = new Vector3(start + i * dx, U_Border - U_Border / 5, cards[i].transform.position.z);
            }
        }
    }
    
    private void Start()
    {
        if(R_Border < 1)
        {
            Vector2 worldBoundary = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
            R_Border = worldBoundary.x;
            U_Border = worldBoundary.y;
        }
        deck = FindObjectOfType<Deck>();
        deck.TakeCards(this);
        setCardsPos();
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
