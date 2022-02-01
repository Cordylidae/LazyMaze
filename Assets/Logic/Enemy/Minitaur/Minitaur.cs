using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minitaur : MonoBehaviour
{

    private Rigidbody2D minitaurRigidbody2D;


    // Start is called before the first frame update
    void Start()
    {
        minitaurRigidbody2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        minitaurRigidbody2D.velocity = Vector2.zero;
    }
}
