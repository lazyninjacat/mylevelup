using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DO_CountingGame : IValidateData
{
    public int collectWordId;
    public int collectNum;
    public int itemMax;
    public bool hints;
    public bool countAudio;
    public bool startAudio;
    public bool defaultImg;

    public List<int> wordIds;

    public DO_CountingGame(List<int> wordIds, int collectNum, int itemMax, bool hints = false, bool countAudio = false, bool startAudio = false, bool defaultImg = false)
    {
        this.collectWordId = wordIds[0];
        this.wordIds = wordIds;
        this.collectNum = collectNum;
        this.itemMax = itemMax;
        this.hints = hints;
        this.countAudio = countAudio;
        this.startAudio = startAudio;
        this.defaultImg = defaultImg;
    }

    /// <summary>
    /// Returns true if the given integer is in the word id list.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>bool</returns>
    public bool IsWordInUse(int id)
    {
        foreach (int wordId in wordIds)
        {
            if (id == wordId)
            {
                Debug.Log("*********************************** \n VAL: WORD ID FOUND IN WORD ID LIST! \n ***********************");
                return true;
            }
        }

        return false;
    }

    public bool ValidateData(DataService data)
    {
        Debug.Log("*************************************\n VAL: Trying to validate COUNTING! \n ********************************");

        foreach (int id in wordIds)
        {
            Debug.Log("*******************************\n CHECKING WORD ID " + id.ToString() + "\n $$$$$$$$$$$$$$$$$$$$$$$$$$$$");
            int temp = data.DoesWordIdExist(id);
            Debug.Log("*******************************\n TEMP INT IS " + temp.ToString() + "\n %%%%%%%%%%%%%%%%%%%%%%%%%%");

            if (temp <= 0)
            {
                Debug.Log("*********************************** \n VAL: WORD DOES NOT exist in DB! \n ***********************");
                return false;
            }
        }

        Debug.Log("********************************* \n VAL: Data validated successfully \n *******************************");
        return true;
    }
}
