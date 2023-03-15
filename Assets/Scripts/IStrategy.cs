using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStrategy
{
    public Card ChooseCardToBeat();
    public bool ChooseCardWhichBeat(out Card card);
    public bool ChooseCardToAdd(out Card card);    
}
