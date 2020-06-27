using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DO_FlashCard : IValidateData
{
    public List<int> wordIdList;

    public DO_FlashCard(List<int> wordIds)
    {
        wordIdList = wordIds;
    }

    public bool ValidateData(DataService data)
    {
        Debug.Log("*************************************\n FLASH: Trying to validate FLASH! \n ********************************");

        foreach (int id in wordIdList)
        {
            Debug.Log("*******************************\n FLASH:CHECKING WORD ID " + id.ToString() + "\n $$$$$$$$$$$$$$$$$$$$$$$$$$$$");
            int temp = data.DoesWordIdExist(id);
            Debug.Log("*******************************\n FLASH:TEMP INT IS " + temp.ToString() + "\n %%%%%%%%%%%%%%%%%%%%%%%%%%");

            if (temp <= 0)
            {
                Debug.Log("*********************************** \n FLASH:: WORD DOES NOT exist in DB! \n ***********************");
                return false;
            }
        }

        Debug.Log("********************************* \n FLASH:: Data validated successfully \n *******************************");
        return true;
    }
}
