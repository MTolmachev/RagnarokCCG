using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Security.Cryptography.X509Certificates;

public class Game
{
    public List<Card> EnemyDeck, PlayerDeck,
                      EnemyHand, PlayerHand,
                      EnemyField, PlayerField;

    public Game()
    {
        EnemyDeck = GiveDeckCard();
        PlayerDeck = GiveDeckCard();

        EnemyHand = new List<Card>();
        EnemyField = new List<Card>();

        PlayerHand = new List<Card>();
        EnemyField = new List<Card>();
    }

    List<Card> GiveDeckCard()
    {
        List<Card> list = new List<Card>();
        for (int i = 0; i < 10; i++)
            list.Add(CardManager.AllCards[Random.Range(0, CardManager.AllCards.Count)]);
        return list;
    }
}


public class GameManagerScript : MonoBehaviour
{
    public Game CurrentGame;
    public Transform EnemyHand, PlayerHand;
    public GameObject CardPref;

    int Turn, TurnTime = 30;
    public TextMeshProUGUI TurnTimeTxt;
    public Button EndTurnBtn;

    public bool IsPlayerTurn {
        get {
            return Turn % 2 == 0;
        }
    }

    private void Start()
    {
        Turn = 0;

        CurrentGame = new Game();

        GiveHandCards(CurrentGame.EnemyDeck, EnemyHand);
        GiveHandCards(CurrentGame.PlayerDeck, PlayerHand);

        StartCoroutine(TurnFunc());
    }

    void GiveHandCards(List<Card> deck, Transform hand)
    {
        int i = 0;
        while (i++ < 4)
            GiveCardsToHand(deck, hand);
    }

    void GiveCardsToHand(List<Card> deck, Transform hand)
    {
        if(deck.Count == 0) return;

        Card card = deck[0];

        GameObject cardGO = Instantiate(CardPref, hand, false);

        if (hand == EnemyHand)
            cardGO.GetComponent<CardInfoScript>().HideCardInfo(card);
        else
            cardGO.GetComponent<CardInfoScript>().ShowCardInfo(card);

        deck.RemoveAt(0);
    }

    IEnumerator TurnFunc()
    {
        TurnTime = 30;
        TurnTimeTxt.text = TurnTime.ToString();

        if(IsPlayerTurn)
        {
            while(TurnTime-- > 0)
            {
                TurnTimeTxt.text = TurnTime.ToString();
                yield return new WaitForSeconds(1);
            }
        }
        else
        {
            while(TurnTime-- > 27)
            {
                TurnTimeTxt.text = TurnTime.ToString();
                yield return new WaitForSeconds(1);
            }
        }

        ChangeTurn();

    }

    public void ChangeTurn()
    {
        StopAllCoroutines();
        Turn++;

        EndTurnBtn.interactable = IsPlayerTurn;

        if (IsPlayerTurn)
            GiveNewCards();
        StartCoroutine(TurnFunc());
    }

    private void GiveNewCards()
    {
        GiveCardsToHand(CurrentGame.PlayerDeck, PlayerHand);
    }
}