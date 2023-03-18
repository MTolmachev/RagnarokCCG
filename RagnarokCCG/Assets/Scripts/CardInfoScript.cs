using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardInfoScript : MonoBehaviour
{
    public Card SelfCard;
    public Image Logo;
    public TextMeshProUGUI Name, Attack, Defense;
    public GameObject HideObj, HighlightedObj;
    public bool IsPlayer;

    public void HideCardInfo(Card card)
    {
       SelfCard = card;
       HideObj.SetActive(true);
       IsPlayer = false;
    }

    public void ShowCardInfo(Card card, bool IsPlayer)
    {
        SelfCard = card;
        this.IsPlayer = IsPlayer;

        HideObj.SetActive(false);

        Logo.sprite = card.Logo;
        Logo.preserveAspect = true;
        Name.text = card.Name;

        RefreshData();



    }

    public void RefreshData()
    {
        Attack.text = SelfCard.Attack.ToString();
        Defense.text = SelfCard.Defense.ToString();
    }

    public void HighlightCard()
    {
        HighlightedObj.SetActive(true);
    }
    public void DeHighlightCard()
    {
        HighlightedObj.SetActive(false);
    }
}
