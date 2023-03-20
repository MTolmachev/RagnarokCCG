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

        CardController card = eventData.pointerDrag.GetComponent<CardController>();

        if (card && 
            GameManagerScript.Instance.IsPlayerTurn &&
            GameManagerScript.Instance.PlayerMana >= card.Card.Manacost &&
            !card.Card.IsPlaced)
        {
            card.Movement.DefaultParent = transform;

            card.OnCast();
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
