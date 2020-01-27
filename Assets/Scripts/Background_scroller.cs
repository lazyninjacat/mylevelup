using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background_scroller : MonoBehaviour {



    public float scrollSpeed;
    public float tileSizeX;

    private Vector2 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }


    void Update()
    {
        float newPosition = Mathf.Repeat(Time.time * scrollSpeed, tileSizeX);
        transform.position = startPosition + Vector2.left * newPosition;


    }
}
