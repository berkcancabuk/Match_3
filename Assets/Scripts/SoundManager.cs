using System;
using UnityEngine;
using UnityEngine.Serialization;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource audioSourceMainMusic;
    [SerializeField] private AudioClip audioClipBubbleSound;
    [SerializeField] private AudioClip audioClipMainMusic;


    private void PlayAudio()
    {
        audioSource.Stop();
        audioSource.PlayOneShot(audioClipBubbleSound);
    }

    private void OnEnable()
    {
        audioSourceMainMusic.clip = audioClipMainMusic;
        audioSourceMainMusic.Play();
        EventManager.OnPlaySound += PlayAudio;
    }

    private void OnDisable()
    {
        EventManager.OnPlaySound -= PlayAudio;
    }
}