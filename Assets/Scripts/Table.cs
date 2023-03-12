using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Table : MonoBehaviour
{
    private Deck deck;
    private UIManager UI;
    [SerializeField] private List<Player> Players = new List<Player>();
    public string currPl { get; private set; }
    public string currEnemy { get; private set; }
    public bool GameEnded { get; private set; }
    
    private List<Card> toBeat = new List<Card>();
    private List<Card> whichBeat = new List<Card>();
    [SerializeField] private List<Transform> toBeatPos;
    [SerializeField] private List<Transform> whichBeatPos;
    public int getToBeatCount() { return toBeat.Count; }
    public int getWhBeatCount() { return whichBeat.Count; }
    public Card getCardToBeat() { return toBeat[toBeat.Count - 1]; }
    [SerializeField] private Transform trashPos;

    

    private bool isPlAlive(string player)
    {
        foreach (var pl in Players)
        {
            if (pl.getName() == player && !pl.isBot())
                return true;
        }
        return false;
    }
    public void PlaceToBeatCard(Player pl, Card card)
    {
        card.OnTable(true);
        card.setFront();
        card.setLayer(1);
        toBeat.Add(card);
        card.transform.position = toBeatPos[toBeat.Count - 1].position;
        pl.RemoveCard(card);
        pl.setCardsPos();        
        CheckBtns();
    }
    public void PlaceWhichBeatCard(Player pl, Card card)
    {
        card.OnTable(true);
        card.setFront();
        card.setLayer(2);
        whichBeat.Add(card);
        card.transform.position = whichBeatPos[whichBeat.Count - 1].position;
        pl.RemoveCard(card);
        pl.setCardsPos();
        CheckBtns();
    }
    public bool HaveValue(int value)
    {
        foreach (var item in toBeat)
        {
            if (item.getValue() == value)
            {
                return true;
            }
        }
        foreach (var item in whichBeat)
        {
            if (item.getValue() == value)
            {
                return true;
            }
        }
        return false;
    }
    public void tryPlaceCard(Player pl, Card card)
    {
        if(pl.getName() == currPl && toBeat.Count < 6)
        {
            if(toBeat.Count > 0 && HaveValue(card.getValue()))
            {
                PlaceToBeatCard(pl, card);
            }
            else if(toBeat.Count == 0)
            {
                PlaceToBeatCard(pl, card);
            }
        }
        else if(pl.getName() == currEnemy && toBeat.Count != whichBeat.Count)
        {
            var item = toBeat[toBeat.Count - 1];
            if(item.getType() != CardType.Joker)
            {
                if (card.getType() == CardType.Joker)
                {
                    PlaceWhichBeatCard(pl, card);
                }
                else if (item.getType() == card.getType()
                    && item.getValue() < card.getValue()
                    && card.getType() != deck.Trump)
                {
                    PlaceWhichBeatCard(pl, card);
                }
                else if (item.getType() == deck.Trump
                    && card.getType() == deck.Trump
                    && item.getValue() < card.getValue())
                {
                    PlaceWhichBeatCard(pl, card);
                }
                else if (card.getType() == deck.Trump 
                    && item.getType() != deck.Trump)
                {
                    PlaceWhichBeatCard(pl, card);
                }
            }          
        }
    }

    private void ChangePlayers(bool skipOne = false)
    {        
        for (int i = 0; i < Players.Count; i++)
        {
            if (Players[i].getName() == currEnemy)
            {
                currPl = Players[i].getName();
                currEnemy = (i + 1 >= Players.Count) ? Players[0].getName() : Players[i + 1].getName();
                break;
            }
        }
        if (skipOne) ChangePlayers(); //check active players later
        UI.UpdPlayers(getCurrPlInd(), getCurrEnInd());
        Debug.Log($"Player: {currPl}\tEnemy: {currEnemy}");
    }
    public void TossCardsToTrash()
    {
        foreach (var item in toBeat)
        {
            item.transform.position = trashPos.position;
            item.setBack();
            item.OnTable(false);           
        }
        toBeat = new List<Card>();
        foreach (var item in whichBeat)
        {
            item.transform.position = trashPos.position;
            item.setBack();
            item.OnTable(false);          
        }
        whichBeat = new List<Card>();
        foreach (var pl in Players)
        {
            deck.TakeCards(pl, 6 - pl.getCount());
        }
        if (haveWinner()) GameEnded = true;
        UI.DeactAllBtns();
        ChangePlayers();
    }

    public void TakeCards()
    {
        foreach (var pl in Players)
        {
            if(pl.getName() == currEnemy)
            {
                foreach (var item in toBeat)
                {
                    item.setBack();
                    pl.AddCard(item);
                    item.OnTable(false);                                    
                }
                toBeat = new List<Card>();
                foreach (var item in whichBeat)
                {
                    item.setBack();
                    pl.AddCard(item);
                    item.OnTable(false);                                     
                }
                whichBeat = new List<Card>();               
            }
            deck.TakeCards(pl, 6 - pl.getCount());
        }
        if (haveWinner()) GameEnded = true;
        UI.DeactAllBtns();
        ChangePlayers(true);
    }
   
    private void CheckBtns()
    {
        if (toBeat[0] != null && toBeat.Count != whichBeat.Count && isPlAlive(currEnemy))
        {
            UI.ActTakeBtn();
        }
        else if (toBeat.Count == whichBeat.Count && isPlAlive(currPl))
        {
            UI.ActPassBtn();
        }        
    }
    private bool haveWinner()
    {
        int i = 0;
        string fool = "";
        foreach (var pl in Players)
        {
            if(pl.hasCards())
            {
                i++;
                fool = pl.getName();
            }
        }
        if(i == 1)
        {
            Debug.Log($"{fool} is a fool!");
            UI.DeactAllBtns(true);
            return true;
        }
        else if(i == 0)
        {
            Debug.Log("Draw");
            UI.DeactAllBtns(true);
            return true;
        }
        return false;
    }
    private int getCurrPlInd()
    {
        for (int i = 0; i < Players.Count; i++)
        {
            if (Players[i].getName() == currPl) return i;
        }
        return -1;
    }
    private int getCurrEnInd()
    {
        for (int i = 0; i < Players.Count; i++)
        {
            if (Players[i].getName() == currEnemy) return i;
        }
        return -1;
    }

    private void Awake()
    {
        UI = FindObjectOfType<UIManager>();
        deck = FindObjectOfType<Deck>();       
        currPl = Players[0].getName();
        currEnemy = Players[1].getName();
        UI.UpdPlayers(getCurrPlInd(), getCurrEnInd());
    }
}
