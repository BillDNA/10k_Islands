using System.Collections;
using System.Collections.Generic;
using Sirenix.Serialization;
using UnityEngine;

public class SFXManager : BaseSingleton<SFXManager>
{
    public AudioSource source;


    public void PlaySFX(SFX sfx)
    {
        if (SFXLibrary.Instance.AllBooks.ContainsKey(sfx))
        {
            source.clip = SFXLibrary.Instance.AllBooks[sfx];
            source.Play();
        }
    }
}