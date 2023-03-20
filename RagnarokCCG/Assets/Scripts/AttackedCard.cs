using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AttackedCard : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        if (!GetComponent<CardMovementScript>().GameManager.IsPlayerTurn) return;

        CardInfoScript card = eventData.pointerDrag.GetComponent<CardInfoScript>();

        if (card && 
            card.SelfCard.CanAtack && 
            transform.parent == GetComponent<CardMovementScript>().GameManager.EnemyField)
        {
            card.SelfCard.ChangeAtackState(false);

            if(card.IsPlayer)
                card.DeHighlightCard();

            GetComponent<CardMovementScript>().GameManager.CardsFight(card, GetComponent<CardInfoScript>());
        }

    }
}
