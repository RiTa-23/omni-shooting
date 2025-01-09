using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Xbutton : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]HowToPlayCanvas CanvasScript;
    public void pushButton()
    {
        CanvasScript.Xbutton();
    }
}
