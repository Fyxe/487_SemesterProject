using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BreadBasket")]
public class BreadBasket : ScriptableObject
{

    public int basketID = -1;
    public int basketCost = 0;
    public string basketName = "Basket";
    [TextArea] public string basketDescription = "A convenient place for storing bread.";
    public Sprite basketSprite;

    public DropSet dropSetWeapons;
    public DropSet dropSetAIs;
    public DropSet dropSetAbilities;
    public DropSet dropSetLevelPiecesGeneral;

}
