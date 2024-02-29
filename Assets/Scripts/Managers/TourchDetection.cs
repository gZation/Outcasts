using System.Collections.Generic;
using UnityEngine;

public class TourchDetection : MonoBehaviour
{
    private int totalTourches;
    private int litTourches;
    private void Start()
    {
        foreach (Transform child in transform)
        {
            var tourch = child.GetComponent<Tourch>();
            if (tourch)
            {
                tourch.Detector = this;
                totalTourches++;
            }
        }
    }

    public void LitTourch()
    {
        litTourches++;
        if (litTourches >= totalTourches)
        {
            // Complete Achievement
        }
    }
}