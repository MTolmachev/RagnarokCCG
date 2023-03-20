using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AttackedCard : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        if (!GameManagerScript.Instance.IsPlayerTurn) return;

        CardController attacker = eventData.pointerDrag.GetComponent<CardController>(),
                       defender = GetComponent<CardController>();

        if (attacker &&
            attacker.Card.CanAtack && 
            defender.Card.IsPlaced)
        {

            /*if(attacker.IsPlayerCard)
                attacker.Info.HighlightCard(false);*/

            GameManagerScript.Instance.CardsFight(attacker, defender);
        }

    }
}
