using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Table : MonoBehaviour
{
    private Deck deck;
    [SerializeField] private List<Player> Players = new List<Player>();
    private string currPl, currEnemy;
    
    private List<Card> toBeat = new List<Card>();
    private List<Card> whichBeat = new List<Card>();
    [SerializeField] private List<Transform> toBeatPos;
    [SerializeField] private List<Transform> whichBeatPos;
    [SerializeField] private Transform trashPos;

    [SerializeField] private Button Take, Pass;

    private void PlaceToBeatCard(Player pl, Card card)
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
    private void PlaceWhichBeatCard(Player pl, Card card)
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
    public void tryPlaceCard(Player pl, Card card)
    {
        if(pl.getName() == currPl && toBeat.Count < 6)
        {
            if(toBeat.Count > 0)
            {
                foreach (var item in toBeat)
                {
                    if(item.getValue() == card.getValue())
                    {
                        PlaceToBeatCard(pl, card);
                    }
                }
                foreach (var item in whichBeat)
                {
                    if(item.getValue() == card.getValue())
                    {
                        PlaceToBeatCard(pl, card);
                    }
                }
            }
            else
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
        haveWinner();
        disableBtns();
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
        haveWinner();
        disableBtns();
        ChangePlayers(true);
    }

    private void disableBtns()
    {
        Take.gameObject.SetActive(false);
        Pass.gameObject.SetActive(false);
    }
    private void CheckBtns()
    {
        if (toBeat[0] != null && toBeat.Count != whichBeat.Count)
        {
            Take.gameObject.SetActive(true);
            Pass.gameObject.SetActive(false);
        }
        else if (toBeat[0] != null)
        {
            Take.gameObject.SetActive(false);
            Pass.gameObject.SetActive(true);
        }        
    }
    private void haveWinner()
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
            disableBtns();
        }
        else if(i == 0)
        {
            Debug.Log("Draw");
            disableBtns();
        }
    }

    private void Start()
    {
        deck = FindObjectOfType<Deck>();       
        currPl = Players[0].getName();
        currEnemy = Players[1].getName();
    }
}
