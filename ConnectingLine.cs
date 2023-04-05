using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectingLine : MonoBehaviour {
    public int cpFrom;
    public int cpTo;
    public bool showing;
    public void Show(bool active)
    {
        showing = active;
        GetComponent<SpriteRenderer>().enabled = active;
    }
}
