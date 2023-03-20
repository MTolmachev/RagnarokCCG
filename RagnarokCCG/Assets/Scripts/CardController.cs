using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CardController : MonoBehaviour
{
    public Card Card;

    public bool IsPlayerCard;

    public CardInfoScript Info;
    public CardMovementScript Movement;

    GameManagerScript GameManager;

    public void Init(Card card, bool isPlayerCard)
    {
        Card = card;
        GameManager = GameManagerScript.Instance;
        IsPlayerCard = isPlayerCard;

        if (isPlayerCard)
        {
            Info.ShowCardInfo();
            GetComponent<AttackedCard>().enabled = false; ;
        }
        else
            Info.HideCardInfo();
    }

    public void OnCast()
    {
        if(IsPlayerCard)
        {
            GameManager.PlayerHandCards.Remove(this);
            GameManager.PlayerFieldCards.Add(this);
            GameManager.ReduceMana(true, Card.Manacost);
            GameManager.CheckCardsForAvailability();
        }
        else
        {
            GameManager.EnemyHandCards.Remove(this);
            GameManager.EnemyFieldCards.Add(this);
            GameManager.ReduceMana(false, Card.Manacost);
        }

        Card.IsPlaced = true;
    }

    public void OnTakeDamage(CardController attaker = null)
    {
        CheckForAlive();

    }

    public void OnDamageDeal()
    {
        Card.CanAtack = false;
        Info.HighlightCard(false);
    }
    public void CheckForAlive()
    {
        if (Card.IsAlive)
            Info.RefreshData();
        else
            DestroyCard();
    }
    void DestroyCard()
    {
        Movement.OnEndDrag(null);

        RemoveCardFromList(GameManager.EnemyFieldCards);
        RemoveCardFromList(GameManager.EnemyHandCards);
        RemoveCardFromList(GameManager.PlayerHandCards);
        RemoveCardFromList(GameManager.PlayerFieldCards);

        Destroy(gameObject);

    }

    void RemoveCardFromList(List<CardController> list)
    {
        if(list.Exists(x => x == this))
            list.Remove(this);
    }
}
