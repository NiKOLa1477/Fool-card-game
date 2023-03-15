using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class DefaultStrategy : MonoBehaviour, IStrategy
{
    private Table table;
    private Player player;
    private Deck deck;
    private List<Card> cards;
    private bool isMoving = false;
   
    public Card ChooseCardToBeat() //min from common or min from trump
    {
        int minValue = 16;
        Card card = null;
        bool found = false;
        foreach (var item in cards)
        {
            if(item.getValue() < minValue 
                && item.getType() != deck.Trump 
                && item.getType() != CardType.Joker)
            {
                minValue = item.getValue();
                card = item;
                found = true;
            }
        }
        if(found) return card;
        foreach (var item in cards)
        {
            if (item.getValue() < minValue)
            {
                minValue = item.getValue();
                card = item;               
            }
        }
        return card;
    }
    public bool ChooseCardWhichBeat(out Card card) //using min card possible
    {
        Card toBeat = table.getCardToBeat();
        int minValue = 16;
        card = null;
        bool found = false;
        if(toBeat.getType() == CardType.Joker) return false;
        foreach (var item in cards) //same type
        {
            if(item.getType() == toBeat.getType() 
                && item.getValue() > toBeat.getValue() 
                && item.getValue() < minValue)
            {
                card = item;
                minValue = item.getValue();
                found = true;
            }
        }
        if (found) return true;
        foreach (var item in cards) //use trump
        {
            if (item.getType() == deck.Trump 
                && toBeat.getType() != deck.Trump
                && item.getValue() < minValue)
            {
                card = item;
                minValue = item.getValue();
                found = true;
            }
        }
        if (found) return true;
        foreach (var item in cards) //use Joker
        {
            if (item.getType() == CardType.Joker)
            {
                card = item;
                return true;
            }
        }
        return false;       
    }
    public bool ChooseCardToAdd(out Card card) //adds only common cards
    {
        bool haveCard = false;
        card = null;
        foreach (var item in cards)
        {
            if (table.HaveValue(item.getValue()) 
                && item.getType() != deck.Trump
                && item.getType() != CardType.Joker)
            {
                card = item;
                haveCard = true;
                break;
            }
        }
        return (haveCard) ? true : false;      
    }
    private void Move()
    {
        cards = player.getCards();
        if(table.getToBeatCount() == 0 && cards.Count > 0) //empty table
        {
            table.PlaceToBeatCard(player, ChooseCardToBeat());                                       
        }
        else if(table.getToBeatCount() != table.getWhBeatCount()) //need to beat
        {
            if (ChooseCardWhichBeat(out var card))
                table.PlaceWhichBeatCard(player, card);
            else
                table.onTakeCards();
        }
        else  //can add more
        {
            if (ChooseCardToAdd(out var card))
                table.PlaceToBeatCard(player, card);
            else 
                table.TossCardsToTrash();
        }     
    }
    void Start()
    {
        table = FindObjectOfType<Table>();
        player = GetComponent<Player>();
        deck = FindObjectOfType<Deck>();
    }

    private void Update()
    {
        if (!table.GameEnded)
        {
            if (player == table.getPlAt(table.currPl) && canAddCard())
            {
                if (!isMoving && !table.isPlsChanging) StartCoroutine(MoveRoutine());
            }
            else if (player == table.getPlAt(table.currEnemy) && table.getToBeatCount() != table.getWhBeatCount())
            {
                if (!isMoving && !table.isPlsChanging) StartCoroutine(MoveRoutine());
            }
        }
    }   

    private bool canAddCard()
    {
        if (table.getToBeatCount() == table.getWhBeatCount()
            && table.getPlAt(table.currEnemy).hasCards()
            && table.getToBeatCount() < 6)
        {
            return true;
        }
        else return false;
    }

    private IEnumerator MoveRoutine()
    {
        isMoving = true;
        yield return new WaitForSeconds(1);
        Move();       
        isMoving = false;
    }
}
