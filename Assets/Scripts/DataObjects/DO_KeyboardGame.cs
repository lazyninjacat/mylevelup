using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DO_KeyboardGame : IValidateData
{
    public bool wordSound;
    public bool letterSound;
    public bool letterName;
    public bool keyboardColor;
    public bool provided;

    public List<int> wordIdList;

    public DO_KeyboardGame(List<int> wordIds, bool wordSound = false, bool letterName = false, bool letterSound = false, bool keyboardColor = true, bool provided = true)
    {
        wordIdList = wordIds;
        this.wordSound = wordSound;
        this.letterName = letterName;
        this.letterSound = letterSound;
        this.keyboardColor = keyboardColor;
        this.provided = provided;
    }

    /// <summary>
    /// Returns true if the given integer is in the word id list.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>bool</returns>
    public bool IsWordInUse(int id)
    {
        foreach (int wordId in wordIdList)
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
        Debug.Log("*************************************\n VAL: Trying to validate KEYBOARD! \n ********************************");

        foreach (int id in wordIdList)
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
