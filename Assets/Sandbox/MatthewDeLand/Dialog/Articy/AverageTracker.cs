using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AverageTracker : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.position = DialogManager.averagePoint;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(DialogManager.averagePoint.x, transform.position.y, DialogManager.averagePoint.y) ;
        Debug.Log(transform.position);
    }
}
