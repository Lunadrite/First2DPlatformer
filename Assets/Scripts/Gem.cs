using System;
using UnityEngine;

public class Gem : MonoBehaviour, IItem
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Implement IItem
    public void Collect()
    {
        // Default behavior: remove the gem from the scene
        Destroy(gameObject);
    }
}
