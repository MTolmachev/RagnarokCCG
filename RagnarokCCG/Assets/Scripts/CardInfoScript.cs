using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardInfoScript : MonoBehaviour
{
    public CardController Controller;
    public Image Logo;
    public TextMeshProUGUI Name, Attack, Defense, Manacost;
    public GameObject HideObj, HighlightedObj;
    public Color NormalCol, TargetCol;

    public void HideCardInfo()
    {
       
       HideObj.SetActive(true);
     
       Manacost.text = "";
    }

    public void ShowCardInfo()
    {
        HideObj.SetActive(false);

        Logo.sprite = Controller.Card.Logo;
        Logo.preserveAspect = true;
        Name.text = Controller.Card.Name;

        RefreshData();


    }

    public void RefreshData()
    {
        Attack.text = Controller.Card.Attack.ToString();
        Defense.text = Controller.Card.Defense.ToString();
        Manacost.text = Controller.Card.Manacost.ToString();
    }

    public void HighlightCard(bool highlight)
    {
        HighlightedObj.SetActive(highlight);
    }
     public void HighlightManaAvaliability(int currentMana)
    {
        GetComponent<CanvasGroup>().alpha = currentMana >= Controller.Card.Manacost ? 1 : .5f;
    }

    public void HighlightAsTarget(bool highlight)
    {
        GetComponent<Image>().color = highlight ? TargetCol : NormalCol;
    }
}
