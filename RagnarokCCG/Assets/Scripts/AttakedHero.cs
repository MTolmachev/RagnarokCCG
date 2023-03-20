using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AttakedHero : MonoBehaviour, IDropHandler
{
    public enum HeroType
    {
        ENEMY,
        PLAYER
    }

    public Color NormalCol, TargetCol;
    
    public HeroType type;


    public void OnDrop(PointerEventData eventData)
    {
        if(!GameManagerScript.Instance.IsPlayerTurn) return;

        CardController card = eventData.pointerDrag.GetComponent<CardController>();

        if(card &&
            card.Card.CanAtack &&
            type == HeroType.ENEMY)
        {
           GameManagerScript.Instance.DamageHero(card, true);
        }
    }

    public void HighlightAsTarget(bool highlight)
    {
        GetComponent<Image>().color = highlight ? TargetCol : NormalCol;
    }
}
