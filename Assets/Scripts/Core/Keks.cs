using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keks : MonoBehaviour
{
    public Animator AnimKeks;

    public void Play(string clip)
    {
        AnimKeks.Play(clip);
    }
    
    public void ResetStateAnimation()
    {
        AnimKeks.Play("Walk");
    }
}
