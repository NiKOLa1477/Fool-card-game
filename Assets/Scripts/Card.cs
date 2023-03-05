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
    private Player player;
    private Table table;
    public bool inDeck { get; private set; }
    private bool onTable;
    public void OnTable(bool value)
    {
        onTable = value;
    }
    public bool isTurned { get; private set; }
    public int getValue() { return value; }
    public CardType getType() { return type; }
    public void setFront() { GetComponent<SpriteRenderer>().sprite = front; }
    public void setBack() { GetComponent<SpriteRenderer>().sprite = back; }
    public void setPlayer(Player pl) { player = pl; }
    public Card TakeCard(Player pl)
    {        
            setPlayer(pl);
            inDeck = false;
            return this;      
    }
    public void setLayer(int ind)
    {
        var sr = GetComponent<SpriteRenderer>();
        sr.sortingOrder = ind;
    }
    public void TurnCard()
    {
        if(isTurned)
        {
            isTurned=false;
            transform.Rotate(0, 0, -90);          
        }
        else
        {
            isTurned=true;          
            transform.Rotate(0, 0, 90);           
        }
    }
    private void OnMouseDown()
    {
        if(!inDeck && !onTable) table.tryPlaceCard(player, this);
    }

    private void Awake()
    {
        table = FindObjectOfType<Table>();
        if(back == null) back = GetComponent<SpriteRenderer>().sprite;
        inDeck = true; 
    }
}
