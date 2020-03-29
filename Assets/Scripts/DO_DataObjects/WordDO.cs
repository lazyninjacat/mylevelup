using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordDO {

    private int idNum;
    private string wordName, stockCustom, wordSound, wordImage, wordTags;

    public WordDO(int id_num, string word_name, string stock_custom, string word_tags, string word_sound, string word_image)
    {
        idNum = id_num;
        wordName = word_name;
        stockCustom = stock_custom;       
        wordTags = word_tags;
        wordSound = word_sound;
        wordImage = word_image;
    }

    public int IdNum
    {
        get
        {
            return idNum;
        }
    }

    public string Word_name
    {
        get
        {
            return wordName;
        }
        set
        {
            wordName = value;
        }
    }

    public string StockCustom
    {
        get
        {
            return stockCustom;
        }
        set
        {
            stockCustom = value;
        }
    }
    
    public string WordTags
    {
        get
        {
            return wordTags;
        }
        set
        {
            wordTags = value;
        }
    }

    public string WordSound
    {
        get
        {
            return wordSound;
        }
        set
        {
            wordSound = value;
        }
    }

    public string WordImage
    {
        get
        {
            return wordImage;
        }
        set
        {
            wordImage = value;
        }
    }

}
