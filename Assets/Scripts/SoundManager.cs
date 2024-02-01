using System;
using UnityEngine;
using UnityEngine.Serialization;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip audioClip;


    private void PlayAudio()
    {
        audioSource.Stop();
        audioSource.PlayOneShot(audioClip);
    }

    private void OnEnable()
    {
        EventManager.OnPlaySound += PlayAudio;
    }

    private void OnDisable()
    {
        EventManager.OnPlaySound -= PlayAudio;
    }
}