﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxController : MonoBehaviour, IPooledObject
{

    [Header("Box Attributes")]
    public Dictionary<string, bool> attributes;
    public string[] fields = {"stickerBlue", "stickerRed", "stickerWhite",
                              "bubbleWrap"}; //test for push
    // Start is called before the first frame update
    public bool isHeavy = false;
    public bool isFragile = false;
    public bool isSafe = true;
    [SerializeField] private List<SpriteRenderer> pattern;


    public delegate void BoxDelegate();
    public BoxDelegate onBoxDeath;

    public virtual void OnObjectSpawn()
    {
        attributes = new Dictionary<string, bool>();
        foreach (string attribute in fields)
        {
            attributes.Add(attribute, false);
        }


        foreach (Transform child in transform)
        {

            if (child.gameObject.name == "bubbleWrap")
            {

                pattern.Add(child.gameObject.GetComponent<SpriteRenderer>());
                continue;

            }
            else
            {

                for (int i = 0; i < 3; i++)
                {

                    pattern.Add(child.GetChild(i).GetComponent<SpriteRenderer>());

                }


            }

        }

        foreach (SpriteRenderer sprite in pattern)
        {


             sprite.enabled = false;
            

 

        }

    }

    protected virtual void Start()
    {

        //OnObjectSpawn();


        //foreach(SpriteRenderer sprite in pattern){

        //    sprite.enabled = false;
        //    sprite.sortingOrder = 1;

        //}

    }

    public virtual bool isBoxHeavy()
    {
        return false;
    }

    public virtual bool isBoxFragile()
    {
        return false;
    }

    void OnTriggerStay2D(Collider2D col)
    {
        string spriteApply = col.gameObject.name;
        if (col.tag == Tags.WRAPPING)
        {
            
            Update_Attributes(spriteApply);
            Change_Pattern(false, spriteApply);

        }

        if (col.tag == Tags.STICKER)
        {

            Update_Attributes(spriteApply);
            Change_Pattern(true, spriteApply);

        }


        if (col.tag == Tags.CHUTE)
        {
            GameObject.Find("BoxOrder").GetComponent<BoxOrderController>().CheckBox(gameObject.GetComponent<BoxController>());

        }

        if (col.tag == Tags.XRAY)
        {
            if(isSafe)
            {
                
                col.transform.GetChild(0).gameObject.SetActive(true);
            } 
            else
            {
                col.transform.GetChild(1).gameObject.SetActive(true);
            }
        }


    }

    void OnTriggerEnter2D(Collider2D col) {

        if (col.tag == Tags.INCINERATOR)
        {
            //Debug.Log("BURN");
            //Object.Destroy(gameObject);
            onDeath();

        }

    }

    void OnTriggerExit2D(Collider2D col)
    {

        if (col.tag == Tags.XRAY)
        {
            col.transform.GetChild(0).gameObject.SetActive(false);
            col.transform.GetChild(1).gameObject.SetActive(false);
        }
    }

    // public void OnTriggerStay2D(Collider2D collision)
    // {
    //         Debug.Log("EnteredChute");
    //     if (collision.tag == Tags.CHUTE)
    //     {
    //         GameObject.Find("BoxOrder").GetComponent<BoxOrderController>().CheckBox(gameObject.GetComponent<BoxController>());

    //     }
    // }

    protected virtual void Change_Pattern(bool sticker, string spriteApply)
    {
        
        // Debug.Log("####: " + GameObject.Find("SpriteContainer").GetComponent<BoxSpriteModifiers>().Apply_Sprite(sticker, spriteApply).ToString());
        
        foreach(SpriteRenderer sprite in pattern){
            
            if(attributes[sprite.name] == false){
                sprite.enabled = false;
            }

            if(sprite.name == spriteApply){

                sprite.enabled = true;

            }
            
        }

    }

    void Update_Attributes(string spriteApply)
    {

        // Debug.Log("###Updating Attribute###");
        // Debug.Log(spriteApply);

        List<string> keys = new List<string>(attributes.Keys);

        attributes[spriteApply] = true;

    }








    public virtual void onDeath()
    {
        onBoxDeath?.Invoke();
        onBoxDeath = null;
        gameObject.SetActive(false);
    }
}
