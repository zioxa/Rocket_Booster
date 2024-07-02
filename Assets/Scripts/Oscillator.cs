using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]   //this will now allow to attach a script again and again on an object
public class Oscillator : MonoBehaviour
{

    [SerializeField] Vector3 movementVector = new Vector3(10f,0f,0f);

    float movementFactor;

    Vector3 startingPosition;

    [Range(1,10)] [SerializeField] float period = 2f;


    // Start is called before the first frame update
    void Start()
    {
        startingPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(period <= Mathf.Epsilon  /* Mathf.Epsilon is the smallest float number we can represent in c# */  ) { return; }
        float cycle = Time.time / period;

        const float tau = Mathf.PI * 2f;

        float rawSinWave = Mathf.Sin(tau * cycle);  //goes from -1 to +1

        movementFactor = rawSinWave / 2f + 0.5f;  // we divide by 2 so our range will be -0.5 to 0.5 and then add o.5 to make the range 0 to 1.

        Vector3 offSet = movementVector * movementFactor;
        transform.position = startingPosition + offSet;
    }
}
