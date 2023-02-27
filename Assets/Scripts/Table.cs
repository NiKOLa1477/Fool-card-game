using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : MonoBehaviour
{
    [SerializeField] private List<Player> Players = new List<Player>();
    private string currPl, currEnemy;

    private CardType Trump;
    private List<Card> toBeat = new List<Card>();
    private List<Card> whichBeat = new List<Card>();
    [SerializeField] private List<Transform> toBeatPos;
    [SerializeField] private List<Transform> whichBeatPos;
    [SerializeField] private Transform trashPos;

    private void PlaceToBeatCard(Player pl, Card card)
    {
        card.setFront();
        toBeat.Add(card);
        card.transform.position = toBeatPos[toBeat.Count - 1].position;
        pl.RemoveCard(card);
    }
    private void PlaceWhichBeatCard(Player pl, Card card)
    {
        card.setFront();
        whichBeat.Add(card);
        card.transform.position = whichBeatPos[whichBeat.Count - 1].position;
        pl.RemoveCard(card);
    }
    public void tryPlaceCard(Player pl, Card card)
    {
        if(pl.name == currPl && toBeat.Count < 6)
        {
            if(toBeat.Count > 1)
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
        else if(pl.name == currEnemy)
        {
            var item = toBeat[toBeat.Count - 1];
            if (card.getType() == CardType.Joker)
            {
                PlaceWhichBeatCard(pl, card);
            }
            else if (item.getType() == card.getType()
                && item.getValue() < card.getValue()
                && card.getType() != Trump)
            {
                PlaceWhichBeatCard(pl, card);
            }
            else if (item.getType() == Trump
                && card.getType() == Trump
                && item.getValue() < card.getValue())
            {
                PlaceWhichBeatCard(pl, card);
            }
            else if (card.getType() == Trump)
            {
                PlaceWhichBeatCard(pl, card);
            }
        }
    }

    private void ChangePlayers(bool skipOne = false)
    {
        
        for (int i = 0; i < Players.Count; i++)
        {
            if (Players[i].name == currEnemy)
            {
                currPl = currEnemy;
                currEnemy = (i + 1 >= Players.Count) ? Players[0].getName() : Players[i + 1].getName();               
            }
        }
        if (skipOne && Players.Count > 2) ChangePlayers(); //check active players later
    }
    public void TossCardsToTrash()
    {
        foreach (var item in toBeat)
        {
            item.transform.position = trashPos.position;
            item.setBack();
            toBeat.Remove(item);
        }
        foreach (var item in whichBeat)
        {
            item.transform.position = trashPos.position;
            item.setBack();
            whichBeat.Remove(item);
        }
        ChangePlayers();
    }

    public void TakeCards()
    {
        foreach (var item in toBeat)
        {
            //move cards to player hand
            item.setBack();
            toBeat.Remove(item);
        }
        foreach (var item in whichBeat)
        {
            //move cards to player hand
            item.setBack();
            whichBeat.Remove(item);
        }
        ChangePlayers(true);
    }

    private void Start()
    {
        Trump = FindObjectOfType<Deck>().Trump;
        currPl = Players[0].getName();
        currEnemy = Players[1].getName();
    }
}
