using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public enum CardType
{
    Club,
    Diamond,
    Heart,
    Spade,
    Joker
}
[RequireComponent(typeof(SpriteRenderer))]
public class Card : MonoBehaviour
{
    private static Sprite back;
    [SerializeField] private Sprite front;
    [SerializeField] private int value;   
    [SerializeField] private CardType type;
    public bool inDeck { get; private set; }
    public bool isTurned { get; private set; }
    public int getValue() { return value; }
    public CardType getType() { return type; }
    public void setFront() { GetComponent<SpriteRenderer>().sprite = front; }
    public void setBack() { GetComponent<SpriteRenderer>().sprite = back; }
    public Card TakeCard()
    {
        if(inDeck)
        {
            inDeck = false;
            return this;
        }
        return null;
    }
    public void TurnCard()
    {
        if(isTurned)
        {
            //reverse position and rotation change
        }
        else
        {
            //position and rotation change
        }
    }

    private void Awake()
    {
        if(back == null) back = GetComponent<SpriteRenderer>().sprite;
        inDeck = true; 
    }
}
