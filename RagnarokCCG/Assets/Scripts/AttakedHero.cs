using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AttakedHero : MonoBehaviour, IDropHandler
{
    public enum HeroType
    {
        ENEMY,
        PLAYER
    }
    
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
}
