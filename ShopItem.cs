using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopItem
{
    private string itemName;
    private int itemPrice;
    private string itemImageName;
    private string itemDescription;

    public ShopItem(string itemName, int itemPrice, string itemImageName, string itemDescription){
        this.itemName=itemName;
        this.itemPrice=itemPrice;
        this.itemImageName=itemImageName;
        this.itemDescription=itemDescription;
    }

    public Sprite getImageSprite(){
        return Resources.Load<Sprite>(itemImageName);
    }

    public string getItemName(){
        return itemName;
    }

    public int getitemPrice(){
        return itemPrice;
    }

    public string getitemImageName(){
        return itemImageName;
    }

    public string getitemDescription(){
        return itemDescription;
    }
}
