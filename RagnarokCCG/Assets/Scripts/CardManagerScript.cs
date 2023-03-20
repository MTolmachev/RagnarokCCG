using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Card
{
    public string Name;
    public Sprite Logo;
    public int Attack, Defense, Manacost;
    public bool CanAtack;
    public bool IsPlaced;
    public bool IsAlive {
        get { return Defense > 0; }
    }

    public Card(string name, string logoPath, int attack, int defense, int manacost)
    {
        Name = name;
        Logo = Resources.Load<Sprite>(logoPath);
        Attack = attack;
        Defense = defense;
        CanAtack = false;
        IsPlaced = false;
        Manacost = manacost;

    }

    public void ChangeAtackState(bool can)
    {
        CanAtack = can;
    }
    public void GetDamage(int dmg)
    {
        Defense -= dmg;
    }
}

public static class CardManager
{
    public static List<Card> AllCards = new List<Card>(); 
}

public class CardManagerScript : MonoBehaviour
{
    private void Awake()
    {
        CardManager.AllCards.Add(new Card("dwarves", "Sprites/Cards/dwarves", 5, 5, 4));
        CardManager.AllCards.Add(new Card("elves", "Sprites/Cards/elves", 4, 3, 5));
        CardManager.AllCards.Add(new Card("mages", "Sprites/Cards/mages", 3, 3, 7));
        CardManager.AllCards.Add(new Card("poison", "Sprites/Cards/poison", 5, 7, 8));
        CardManager.AllCards.Add(new Card("valkyrie", "Sprites/Cards/valkyrie", 2, 5, 3));
        CardManager.AllCards.Add(new Card("vikings", "Sprites/Cards/vikings", 3, 2,2));
    }
}
