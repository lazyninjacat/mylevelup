using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MAS_Admin : MasterClass
{
    public override void CreateWorkers()
    {
        AddController("CON_AdminMenu", new CON_AdminMenu(this));
        AddModel("MOD_AdminMenu", new MOD_AdminMenu(this));
    }
}
