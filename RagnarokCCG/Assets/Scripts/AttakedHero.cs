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

    public GameManagerScript GameManager;

    public void OnDrop(PointerEventData eventData)
    {
        if(!GameManager.IsPlayerTurn) return;

        CardInfoScript card = eventData.pointerDrag.GetComponent<CardInfoScript>();

        if(card &&
            card.SelfCard.CanAtack &&
            type == HeroType.ENEMY)
        {
            card.SelfCard.CanAtack = false;
            GameManager.DamageHero(card, true);
        }
    }

    public void HighlightAsTarget(bool highlight)
    {
        GetComponent<Image>().color = highlight ? TargetCol : NormalCol;
    }
}
