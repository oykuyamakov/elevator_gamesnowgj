using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteBob : MonoBehaviour
{
    Vector3 startPos;
    
    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = transform.position;
        pos.y = startPos.y + Mathf.Sin(Time.timeSinceLevelLoad * 4f) * .02f;
        transform.position = pos;
    }
}
