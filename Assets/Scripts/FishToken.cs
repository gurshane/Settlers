using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishToken : MonoBehaviour {

    public bool isBoot;
    public int value;

    public FishToken(bool p_boot, int p_value)
    {
        isBoot = p_boot;
        value = p_value;
    }
}
