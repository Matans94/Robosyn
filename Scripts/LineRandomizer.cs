using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Just a hacky class to quickly check my shader on many different lines.
// In play mode - click "Space" To randomize the line
public class LineRandomizer : MonoBehaviour
{

    private LineRenderer lr;
    private int pts;
    
    void Start()
    {
        lr = GetComponent<LineRenderer>();
        pts = lr.positionCount;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) {
            Vector3[] newPts = new Vector3[pts];
            for (int i = 0; i < pts; i++) {
                newPts[i] = new Vector3(Random.Range(-5, 5), Random.Range(-5, 5), 1);
            }
            lr.SetPositions(newPts);

        }
        
    }



}
