using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class soundEffect : MonoBehaviour
{
    public AudioSource mySounds;
    public AudioClip hoverSound;
    public AudioClip clickSound;

    public void hoveringSound(){
        mySounds.PlayOneShot(hoverSound);
    }

    public void clickingSound(){
        mySounds.PlayOneShot(clickSound);
    }
}
