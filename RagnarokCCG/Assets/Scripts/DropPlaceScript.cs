using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;


public enum FieldType
{
    PLAYER_HAND,
    PLAYER_FIELD,
    ENEMY_HAND,
    ENEMY_FIELD
}

public class DropPlaceScript : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public FieldType Type;
    
    public void OnDrop(PointerEventData eventData)
    {
        if (Type != FieldType.PLAYER_FIELD)
            return;

        CardMovementScript card = eventData.pointerDrag.GetComponent<CardMovementScript>();

        if (card && 
            card.GameManager.PlayerFieldCards.Count < 6 &&
            card.GameManager.IsPlayerTurn && 
            card.GameManager.PlayerMana >= card.GetComponent<CardInfoScript>().SelfCard.Manacost)
        {
            card.GameManager.PlayerHandCards.Remove(card.GetComponent<CardInfoScript>());
            card.GameManager.PlayerFieldCards.Add(card.GetComponent<CardInfoScript>());
            card.DefaultParent = transform;

            card.GameManager.ReduceMana(true, card.GetComponent<CardInfoScript>().SelfCard.Manacost);
        }
            
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null || Type == FieldType.ENEMY_FIELD || Type == FieldType.ENEMY_HAND || Type == FieldType.PLAYER_HAND)
            return;

        CardMovementScript card = eventData.pointerDrag.GetComponent<CardMovementScript>();
        if (card)
            card.DefaultTempCardParent = transform;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null)
            return;

        CardMovementScript card = eventData.pointerDrag.GetComponent<CardMovementScript>();
        if (card && card.DefaultTempCardParent == transform)
            card.DefaultTempCardParent = card.DefaultParent;
    }
}
