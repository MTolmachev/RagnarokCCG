using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Security.Cryptography.X509Certificates;

public class Game
{
    public List<Card> EnemyDeck, PlayerDeck;

    public Game()
    {
        EnemyDeck = GiveDeckCard();
        PlayerDeck = GiveDeckCard();
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
    public Transform EnemyHand, PlayerHand,
                     EnemyField, PlayerField;
    public GameObject CardPref;

    int Turn, TurnTime = 30;
    public TextMeshProUGUI TurnTimeTxt;
    public Button EndTurnBtn;

    public int PlayerMana = 10, EnemyMana = 10;
    public TextMeshProUGUI PlayerManaTxt, EnemyManaTxt;

    public int PlayerHP = 10, EnemyHP = 10;
    public TextMeshProUGUI PlayerHPTxt, EnemyHPTxt;

    public GameObject ResultGO;
    public TextMeshProUGUI ResultTxt;

    public AttakedHero EnemyHero, PlayerHero;


    public List<CardInfoScript> PlayerHandCards = new List<CardInfoScript>(),
                                PlayerFieldCards = new List<CardInfoScript>(),
                                EnemyHandCards = new List<CardInfoScript>(),
                                EnemyFieldCards = new List<CardInfoScript>();

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

        ShowMana();
        ShowHP();

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
        {
            cardGO.GetComponent<CardInfoScript>().HideCardInfo(card);
            EnemyHandCards.Add(cardGO.GetComponent<CardInfoScript>());
        }
        else
        {
            cardGO.GetComponent<CardInfoScript>().ShowCardInfo(card, true);
            PlayerHandCards.Add(cardGO.GetComponent<CardInfoScript>());
            cardGO.GetComponent<AttackedCard>().enabled = false;
        }
            

        deck.RemoveAt(0);
    }

    IEnumerator TurnFunc()
    {
        TurnTime = 30;
        TurnTimeTxt.text = TurnTime.ToString();

        foreach (var card in PlayerFieldCards)
            card.DeHighlightCard();

        CheckCardForAvailability();

        if (IsPlayerTurn)
        {
            foreach (var card in PlayerFieldCards)
            {
                card.SelfCard.ChangeAtackState(true);
                card.HighlightCard();
            }
                

            while(TurnTime-- > 0)
            {
                TurnTimeTxt.text = TurnTime.ToString();
                yield return new WaitForSeconds(1);
            }
            ChangeTurn();
        }
        else
        {
            foreach (var card in EnemyFieldCards)
                card.SelfCard.ChangeAtackState(true);

           
           StartCoroutine(EnemyTurn(EnemyHandCards));
            
        }
    }

    IEnumerator EnemyTurn(List<CardInfoScript> cards)
    {
        yield return new WaitForSeconds(1);

        int count = cards.Count == 1 ? 1 :
            Random.Range(0, cards.Count);

        for (int i = 0; i < count; i++)
        {
            if (EnemyFieldCards.Count > 5 ||
                EnemyMana == 0 ||
                EnemyHandCards.Count == 0)
                break;

            List<CardInfoScript> cardsList = cards.FindAll(x => EnemyMana >= x.SelfCard.Manacost);

            if (cardsList.Count == 0) break;

            cardsList[0].GetComponent<CardMovementScript>().MoveToField(EnemyField);

            ReduceMana(false, cardsList[0].SelfCard.Manacost);

            yield return new WaitForSeconds(.51f);

            cardsList[0].ShowCardInfo(cardsList[0].SelfCard, false);
            cardsList[0].transform.SetParent(EnemyField);

            EnemyFieldCards.Add(cardsList[0]);
            EnemyHandCards.Remove(cardsList[0]);
        }
        GiveCardsToHand(CurrentGame.EnemyDeck, EnemyHand);

        yield return new WaitForSeconds(1);

        foreach (var activeCard in EnemyFieldCards.FindAll(x=>x.SelfCard.CanAtack))
        {
            if (Random.Range(0, 2) == 0 &&
                PlayerFieldCards.Count > 0)
            {
                var enemy = PlayerFieldCards[Random.Range(0, PlayerFieldCards.Count)];

                activeCard.SelfCard.ChangeAtackState(false);

                activeCard.GetComponent<CardMovementScript>().MoveToTarget(enemy.transform);
                yield return new WaitForSeconds(.75f);

                CardsFight(enemy, activeCard);
            }
            else
            {
                activeCard.SelfCard.ChangeAtackState(false);

                activeCard.GetComponent<CardMovementScript>().MoveToTarget(PlayerHero.transform);
                yield return new WaitForSeconds(.75f);
                DamageHero(activeCard, false);
            }
            
            yield return new WaitForSeconds(.2f);

        }
        yield return new WaitForSeconds(1f);
        ChangeTurn();
    }

    public void ChangeTurn()
    {
        StopAllCoroutines();
        Turn++;

        EndTurnBtn.interactable = IsPlayerTurn;

        if (IsPlayerTurn)
        {
            GiveNewCards();
            PlayerMana = EnemyMana = 10;
            ShowMana();
        }
            
        StartCoroutine(TurnFunc());
    }

    private void ShowMana()
    {
        PlayerManaTxt.text = PlayerMana.ToString();
        EnemyManaTxt.text = EnemyMana.ToString();
    }

    private void ShowHP()
    {
        PlayerHPTxt.text = PlayerHP.ToString();
        EnemyHPTxt.text = EnemyHP.ToString();
    }

    private void GiveNewCards()
    {
        GiveCardsToHand(CurrentGame.PlayerDeck, PlayerHand);
    }

    public void CardsFight(CardInfoScript playerCard, CardInfoScript enemyCard)
    {
        playerCard.SelfCard.GetDamage(enemyCard.SelfCard.Attack);
        enemyCard.SelfCard.GetDamage(playerCard.SelfCard.Attack);

        if (!playerCard.SelfCard.IsAlive)
            DestroyCard(playerCard);
        else
            playerCard.RefreshData();

        if (!enemyCard.SelfCard.IsAlive)
            DestroyCard(enemyCard);
        else
            enemyCard.RefreshData();
    }

    void DestroyCard(CardInfoScript card)
    {
        card.GetComponent<CardMovementScript>().OnEndDrag(null);

        if (EnemyFieldCards.Exists(x => x == card))
            EnemyFieldCards.Remove(card);

        if (PlayerFieldCards.Exists(x => x == card))
            PlayerFieldCards.Remove(card);

        Destroy(card.gameObject);

    }

    public void ReduceMana(bool playerMana, int manacost)
    {
        if(playerMana)
            PlayerMana = Mathf.Clamp(PlayerMana - manacost, 0, int.MaxValue);
        else
            EnemyMana = Mathf.Clamp(EnemyMana - manacost, 0, int.MaxValue);

        ShowMana();
    }

    public void DamageHero(CardInfoScript card, bool isEnemyAttacked)
    {
        if (isEnemyAttacked)
            EnemyHP = Mathf.Clamp(EnemyHP - card.SelfCard.Attack, 0, int.MaxValue);
        else
            PlayerHP = Mathf.Clamp(PlayerHP - card.SelfCard.Attack, 0, int.MaxValue);

        ShowHP();

        card.DeHighlightCard();
        CheckForResult();
    }

    void CheckForResult()
    {
        if (EnemyHP == 0)
            ShowResult("YOU WIN");
        else if (PlayerHP == 0)
            ShowResult("YOU LOOOOOSE");
    }

    void ShowResult(string msg)
    {
        ResultGO.SetActive(true);
        ResultTxt.text = msg;
        StopAllCoroutines();
    }

    public void CheckCardForAvailability()
    {
        foreach (var card in PlayerHandCards)
            card.CheckForAvailability(PlayerMana);
    }

    public void HighlightTargets(bool highlight)
    {
        foreach(var card in EnemyFieldCards)
            card.HighlightAsTarget(highlight);

        EnemyHero.HighlightAsTarget(highlight);
    }
}
