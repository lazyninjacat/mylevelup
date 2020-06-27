using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IValidateData
{
    /// <summary>
    /// This method is used by data objects to validate their attached data.
    /// Returns true if data is still valid.\
    /// <seealso cref="DO_PlayListObject"/>
    /// <seealso cref="DO_Reward"/>
    /// <seealso cref="DO_WordScramble"/>
    /// </summary>
    /// <returns>bool</returns>
    bool ValidateData(DataService data);
}
