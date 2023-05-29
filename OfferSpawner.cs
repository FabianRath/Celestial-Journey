using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OfferSpawner : MonoBehaviour{
    
    public GameObject offer;
    private ShopItem[] shopItems = {
        new ShopItem("Shield", 1, "shield", "A shield allows you to hit a pillar and survive, you can only carry one per run. When the spaceship collides with a pillar you will have 5 seconds of immunity for collisions with any pillars."),
        new ShopItem("Booster", 1, "booster", "A booster will allow you to skip the first 500 meters of distance. This is useful for coin collection as the number of coins you will receive when flying through a ring is dependent on the distance you have travelled."),
        new ShopItem("Light", 100, "lighting", "Work in Progress")};
    private List<Button> itemButtonList = new List<Button>();
    private bool loaded = false;

    void Update(){
        if(PlayerPrefs.GetInt("shop")==1){
            if(!loaded){
                spawn();
            }
            PlayerPrefs.SetInt("shop",0);
        }

        for(int x = 0;x < shopItems.Length; x++){
            if(PlayerPrefs.GetInt(shopItems[x].getItemName()) != 0){
                GameObject offerObject = GameObject.Find("Offer"+x);
                Image imageX = offerObject.transform.Find("XImage").GetComponent<Image>();
                Sprite spriteX = Resources.Load<Sprite>("XImageSprite");
                imageX.sprite = spriteX;
            }    
        }
    }

    void spawn(){
        GameObject newObj;
        for(int i = 0; i < shopItems.Length; i++){
            newObj = Instantiate(offer, transform);
            newObj.name = "Offer" + i;
            
            TextMeshProUGUI textItemName = newObj.transform.Find("ButtonItem/TextItemName").GetComponent<TextMeshProUGUI>();
            textItemName.text = shopItems[i].getItemName();

            TextMeshProUGUI textItemPrice = newObj.transform.Find("TextItemPrice").GetComponent<TextMeshProUGUI>();
            textItemPrice.text = shopItems[i].getitemPrice().ToString();

            TextMeshProUGUI textItemDescription = newObj.transform.Find("TextItemDescription").GetComponent<TextMeshProUGUI>();
            textItemDescription.text = shopItems[i].getitemDescription();

            Image imageItem = newObj.transform.Find("ImageItem").GetComponent<Image>();
            imageItem.sprite = shopItems[i].getImageSprite();

            Button buttonItem = newObj.transform.Find("ButtonItem").GetComponent<Button>();
            buttonItem.interactable = PlayerPrefs.GetInt(shopItems[i].getItemName()) != 0 ? false : true;
            buttonItem.onClick.AddListener(() => TaskOnClick(buttonItem));
            itemButtonList.Add(buttonItem);
            }
        loaded = true;
    }

    void TaskOnClick(Button buttonClicked){
        for(int i = 0; i < itemButtonList.Count; i++){
            if(buttonClicked == itemButtonList[i]){
                int totalCoins = PlayerPrefs.GetInt("totalCoins");
                if(shopItems[i].getitemPrice() < totalCoins){
                    PlayerPrefs.SetInt("totalCoins", totalCoins-shopItems[i].getitemPrice());
                    PlayerPrefs.SetInt(shopItems[i].getItemName(), 1);
                }
                if(PlayerPrefs.GetInt(shopItems[i].getItemName()) != 0){
                        itemButtonList[i].interactable = false;
                }
            }
        }
    }
}