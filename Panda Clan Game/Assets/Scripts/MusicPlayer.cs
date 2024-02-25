using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicPlayer : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] AudioSource songSrc;

    [Header("Audio Files")]
    public AudioClip song1;
    public AudioClip song2;

    // Start is called before the first frame update
    void Start()
    {
        songSrc.clip= song1;
        songSrc.Play();

    }

    // Update is called once per frame
    void Update()
    {
        

    }
}
