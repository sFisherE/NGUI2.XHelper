﻿using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

public class DestroyComponent : ScriptableWizard
{

    [MenuItem("NGUIHelper/Destroy Component")]
    static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard("Destroy Component", typeof(DestroyComponent), "Close", "Destroy");
    }


}
