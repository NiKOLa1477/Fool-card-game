using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Profiling;
using UnityEngine.UI;

public class Table : MonoBehaviour
{
    private static float R_Border, U_Border;
    private Deck deck;
    private UIManager UI;
    [SerializeField] private List<Player> Players = new List<Player>();
    public int getPlCount() { return Players.Count; }
    public int currPl { get; private set; }
    public int currEnemy { get; private set; }
    public bool isPlayerMoving { get; private set; }
    public Player getPlAt(int index) { return Players[index]; }
    public bool GameEnded { get; private set; }   
    private bool isPlayerFinished;
    private float beatTime = 20;
    private bool savedWinner;

    public void finishTurn() 
    {
        UI.DeactAllBtns();
        isPlayerFinished = true;       
    }
    
    private List<Card> toBeat = new List<Card>();
    private List<Card> whichBeat = new List<Card>();
    [SerializeField] private List<Transform> toBeatPos;
    [SerializeField] private List<Transform> whichBeatPos;
    public int getToBeatCount() { return toBeat.Count; }
    public int getWhBeatCount() { return whichBeat.Count; }
    public Card getCardToBeat() { return toBeat[toBeat.Count - 1]; }
    [SerializeField] private Transform trashPos;
  
    private int findPlIndex(Player pl)
    {
        for (int i = 0; i < Players.Count; i++)
        {
            if (pl == Players[i]) return i;
        }
        return -1;
    }
    public void setPlCardsPos(Player pl)
    {
        var cards = pl.getCards();
        float dx, start;
        if (pl.isBot())
        {
            switch (Players.Count)
            {
                case 2:
                    dx = (R_Border * 2 - (R_Border / 4)) / cards.Count;
                    if (dx > 1.4f)
                        dx = 1.4f;
                    start = -R_Border + (R_Border / 10);
                    for (int i = 0; i < cards.Count; i++)
                    {
                        cards[i].setLayer(i);
                        cards[i].transform.position = new Vector3(start + i * dx, U_Border - U_Border / 5, cards[i].transform.position.z);
                    }
                    break;
                case 3:
                    if(findPlIndex(pl) == 1)
                    {
                        dx = (R_Border - (R_Border / 4)) / cards.Count;
                        if (dx > 1.4f)
                            dx = 1.4f;
                        start = -R_Border + (R_Border / 10);
                        for (int i = 0; i < cards.Count; i++)
                        {
                            cards[i].setLayer(i);
                            cards[i].transform.position = new Vector3(start + i * dx, U_Border - U_Border / 5, cards[i].transform.position.z);
                        }
                    }
                    else
                    {
                        dx = (R_Border - (R_Border / 4)) / cards.Count;
                        if (dx > 1.4f)
                            dx = 1.4f;
                        start = R_Border - (R_Border / 10);
                        for (int i = 0; i < cards.Count; i++)
                        {
                            cards[i].setLayer(i);
                            cards[i].transform.position = new Vector3(start - i * dx, U_Border - U_Border / 5, cards[i].transform.position.z);
                        }
                    }
                    break;
                case 4:
                    if (findPlIndex(pl) == 1)
                    {
                        dx = (R_Border - (R_Border / 2)) / cards.Count;
                        if (dx > 1.4f)
                            dx = 1.4f;
                        start = -R_Border + (R_Border / 10);
                        for (int i = 0; i < cards.Count; i++)
                        {
                            cards[i].setLayer(i);
                            cards[i].transform.position = new Vector3(start + i * dx, U_Border - U_Border / 5, cards[i].transform.position.z);
                        }
                    }
                    else if (findPlIndex(pl) == 2)
                    {
                        dx = (R_Border / 2) / cards.Count;
                        if (dx > 1.4f)
                            dx = 1.4f;
                        start = -R_Border / 4;
                        for (int i = 0; i < cards.Count; i++)
                        {
                            cards[i].setLayer(i);
                            cards[i].transform.position = new Vector3(start + i * dx, U_Border - U_Border / 5, cards[i].transform.position.z);
                        }
                    }                    
                    else
                    {
                        dx = (R_Border - (R_Border / 2)) / cards.Count;
                        if (dx > 1.4f)
                            dx = 1.4f;
                        start = R_Border - (R_Border / 10);
                        for (int i = 0; i < cards.Count; i++)
                        {
                            cards[i].setLayer(i);
                            cards[i].transform.position = new Vector3(start - i * dx, U_Border - U_Border / 5, cards[i].transform.position.z);
                        }
                    }
                    break;
            }
        }
        else
        {            
            dx = (R_Border * 2 - (R_Border / 4)) / cards.Count;
            if (dx > 1.4f) dx = 1.4f;
            start = -R_Border + (R_Border / 10);
            for (int i = 0; i < cards.Count; i++)
            {
                cards[i].transform.position = new Vector3(start + i * dx, -U_Border + U_Border / 4, cards[i].transform.position.z);
            }
        }
    }
    public void PlaceToBeatCard(Player pl, Card card)
    {
        card.OnTable(true);
        card.setFront();
        card.setLayer(1);
        toBeat.Add(card);
        card.transform.position = toBeatPos[toBeat.Count - 1].position;
        pl.RemoveCard(card);
        setPlCardsPos(pl);        
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
        setPlCardsPos(pl);
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
        if(pl == Players[currPl] && !UI.isPaused 
            && Players[currEnemy].getCardsCount() > toBeat.Count - whichBeat.Count
            && toBeat.Count < 6)
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
        else if(pl == Players[currEnemy] && toBeat.Count != whichBeat.Count && !UI.isPaused)
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
        var temp = tryFindNext(currEnemy, Players.Count);       
        if(temp == -1)
        {
            temp = tryFindNext(0, currEnemy);           
        }
        if (temp == -1) Debug.Log("Can't find next player");
        else currPl = temp;
        temp = tryFindNext(currPl + 1, Players.Count);
        if(temp == -1)
        {
            temp = tryFindNext(0, currPl);
        }
        if (temp == -1) Debug.Log("Can't find next enemy");
        else currEnemy = temp;
        if (skipOne) ChangePlayers(); 
        UI.UpdPlayers(currPl, currEnemy);               
    }

    private int tryFindNext(int start, int end)
    {
        for (int i = start; i < end; i++)
        {
            if (Players[i].hasCards())
            {
                return i;               
            }
        }
        return -1;
    }
    public void TossCardsToTrash()
    {
        isPlayerMoving = true;
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
            deck.TakeCards(pl, 6 - pl.getCardsCount());
        }
        if (haveWinner()) GameEnded = true;
        UI.DeactAllBtns();
        if(!GameEnded) ChangePlayers();
        isPlayerMoving = false;
    }

    public void onTossCards()
    {
        isPlayerFinished = false;
        UI.DeactAllBtns();
        StopAllCoroutines();        
        StartCoroutine(addCardsToBeat(3));
    }
    private IEnumerator addCardsToBeat(float delay)
    {
        int placedCards = 0;
        for (int i = currEnemy + 1; i < Players.Count; i++)
        {
            if (i != currEnemy)
            {
                if (Players[i].isBot())
                {
                    var str = Players[i].GetComponent<IStrategy>();
                    while (Players[currEnemy].getCardsCount() > toBeat.Count - whichBeat.Count
                        && toBeat.Count < 6)
                    {
                        if (str.ChooseCardToAdd(out var card))
                        {
                            placedCards++;
                            PlaceToBeatCard(Players[i], card);
                            if (!Players[currEnemy].isBot()) delay = beatTime;
                            yield return new WaitForSeconds(delay);
                        }
                        else break;
                    }
                }
                else if (Players[i].hasCards())
                {
                    var tempPl = currPl;
                    currPl = i;
                    UI.ActPassBtn();
                    isPlayerMoving = true;
                    while (!isPlayerFinished)
                    {
                        yield return new WaitForSeconds(delay);
                    }
                    isPlayerFinished = false;
                    UI.DeactAllBtns();
                    currPl = tempPl;                    
                }               
            }
        }
        for (int i = 0; i < currPl; i++)
        {
            if (i != currEnemy)
            {
                if (Players[i].isBot())
                {
                    var str = Players[i].GetComponent<IStrategy>();
                    while (Players[currEnemy].getCardsCount() > toBeat.Count - whichBeat.Count
                        && toBeat.Count < 6)
                    {
                        if (str.ChooseCardToAdd(out var card))
                        {
                            PlaceToBeatCard(Players[i], card);
                            yield return new WaitForSeconds(delay);
                        }
                        else break;
                    }
                }
                else if (Players[i].hasCards())
                {
                    var tempPl = currPl;
                    currPl = i;
                    UI.ActPassBtn();
                    isPlayerMoving = true;
                    while (!isPlayerFinished)
                    {
                        yield return new WaitForSeconds(delay);
                    }
                    isPlayerFinished = false;
                    UI.DeactAllBtns();
                    currPl = tempPl;                   
                }
            }
        }
        if (placedCards > 0) onTossCards();
        else if (toBeat.Count != whichBeat.Count)
        {
            TakeCards();
        }
        else
        {
            TossCardsToTrash();
        }
    }

    public void onTakeCards()
    {       
        isPlayerFinished = false;
        UI.DeactAllBtns();
        StopAllCoroutines();
        StartCoroutine(addCardsToTake(1));       
    }

    private IEnumerator addCardsToTake(float delay)
    {
        for (int i = currPl; i < Players.Count; i++)
        {
            if(i != currEnemy)
            {
                if (Players[i].isBot())
                {
                    var str = Players[i].GetComponent<IStrategy>();
                    while (Players[currEnemy].getCardsCount() > toBeat.Count - whichBeat.Count
                        && toBeat.Count < 6)
                    {
                        if (str.ChooseCardToAdd(out var card))
                        {
                            yield return new WaitForSeconds(delay);
                            PlaceToBeatCard(Players[i], card);
                        }
                        else break;
                    }
                }
                else if (Players[i].hasCards())
                {
                    var tempPl = currPl;
                    currPl = i;
                    UI.ActPassBtn();
                    isPlayerMoving = true;
                    while (!isPlayerFinished)
                    {
                        yield return new WaitForSeconds(delay);
                    }
                    isPlayerFinished = false;
                    UI.DeactAllBtns();
                    currPl = tempPl;
                }
            }
        }
        for (int i = 0; i < currPl; i++)
        {
            if (i != currEnemy)
            {
                if (Players[i].isBot())
                {
                    var str = Players[i].GetComponent<IStrategy>();
                    while (Players[currEnemy].getCardsCount() > toBeat.Count - whichBeat.Count
                        && toBeat.Count < 6)
                    {
                        if (str.ChooseCardToAdd(out var card))
                        {
                            yield return new WaitForSeconds(delay);
                            PlaceToBeatCard(Players[i], card);
                        }
                        else break;
                    }
                }
                else if (Players[i].hasCards())
                {
                    var tempPl = currPl;
                    currPl = i;
                    UI.ActPassBtn();
                    isPlayerMoving = true;
                    while (!isPlayerFinished)
                    {
                        yield return new WaitForSeconds(delay);
                    }
                    isPlayerFinished = false;
                    UI.DeactAllBtns();
                    currPl = tempPl;
                }
            }
        }
        TakeCards();
    }
    private void TakeCards()
    {
        isPlayerMoving = true;
        foreach (var pl in Players)
        {
            if(pl == Players[currEnemy])
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
            deck.TakeCards(pl, 6 - pl.getCardsCount());
        }
        if (haveWinner()) GameEnded = true;
        UI.DeactAllBtns();
        if (!GameEnded) ChangePlayers(true);
        isPlayerMoving = false;       
    }
   
    private void CheckBtns()
    {
        if (toBeat[0] != null && toBeat.Count != whichBeat.Count && !Players[currEnemy].isBot())
        {
            UI.ActTakeBtn();
        }
        else if (toBeat.Count == whichBeat.Count && !Players[currPl].isBot())
        {
            UI.ActTossBtn();
        }       
    }
    private bool haveWinner()
    {
        int actPls = 0;
        string fool = "";
        foreach (var pl in Players)
        {
            if(pl.hasCards())
            {
                actPls++;
                fool = pl.getName();
            }
        }
        saveWinner(actPls);
        if(actPls == 1)
        {
            saveTotal();
            for (int i = 0; i < Players.Count; i++)           
            {
                if (Players[i].getName() == fool)
                {
                    saveFool(i);
                    PlayerPrefs.SetInt("LastFool", i);
                }
            }
            Debug.Log($"{fool} is a fool!");
            UI.DeactAllBtns(true);
            return true;
        }
        else if(actPls == 0)
        {
            saveTotal();
            PlayerPrefs.DeleteKey("LastFool");
            Debug.Log("Draw");
            UI.DeactAllBtns(true);
            return true;
        }
        return false;
    }  

    private void saveTotal()
    {
        if (PlayerPrefs.HasKey("Total"))
        {
            PlayerPrefs.SetInt("Total", PlayerPrefs.GetInt("Total") + 1);
        }
        else
        {
            PlayerPrefs.SetInt("Total", 1);
        }
    }
    private void saveWinner(int actPls)
    {
        if (actPls == Players.Count - 1)
        {
            foreach (var pl in Players)
            {
                if (!pl.hasCards() && !pl.isBot() && !savedWinner)
                {
                    savedWinner = true;
                    if(PlayerPrefs.HasKey("Wins"))
                    {
                        PlayerPrefs.SetInt("Wins", PlayerPrefs.GetInt("Wins") + 1);
                    }
                    else
                    {
                        PlayerPrefs.SetInt("Wins", 1);
                    }
                }
            }
        }
    }
    private void saveFool(int index)
    {
        if (!Players[index].isBot())
        {
            if (PlayerPrefs.HasKey("Looses"))
            {
                PlayerPrefs.SetInt("Looses", PlayerPrefs.GetInt("Looses") + 1);
            }
            else
            {
                PlayerPrefs.SetInt("Looses", 1);
            }
        }
    }

    private void Awake()
    {
        savedWinner = false;
        deck = FindObjectOfType<Deck>();       
        Vector2 worldBoundary = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        R_Border = worldBoundary.x;
        U_Border = worldBoundary.y;
        UI = FindObjectOfType<UIManager>();        
    }
    private void Start()
    {
        loadData();
        foreach (var pl in Players)
        {
            pl.TakeCards();
        }
        if (PlayerPrefs.HasKey("LastFool"))
        {
            currEnemy = PlayerPrefs.GetInt("LastFool");
            currPl = (currEnemy == 0) ? Players.Count - 1 : currEnemy - 1;
        }
        else
        {
            currPl = getPlWithMinTrump();
            currEnemy = (currPl == Players.Count - 1) ? 0 : currPl + 1;
        }
        UI.UpdPlayers(currPl, currEnemy);
    }

    private int getPlWithMinTrump()
    {
        int minInd = 5;
        int minValue = 15;
        for (int i = 0; i < Players.Count; i++)
        {
            if (Players[i].getMinTrumpValue() < minValue)
            {
                minInd = i;
                minValue = Players[i].getMinTrumpValue();
            }
        }
        return minInd;
    }

    private void loadData()
    {
        if(PlayerPrefs.HasKey("Players"))
        {
            int count = PlayerPrefs.GetInt("Players");
            for (int i = Players.Count - 1; i >= count; i--)
            {
                Players.Remove(Players[i]);
            }
        }       
    }
}
