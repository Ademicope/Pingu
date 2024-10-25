using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    private Animator anim;
    private AudioSource coinAudio;

    public AudioClip coinSound;

    // Start is called before the first frame update
    private void Awake()
    {
        anim = GetComponent<Animator>();
        coinAudio = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        anim.SetTrigger("Spawn");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            coinAudio.PlayOneShot(coinSound, 1.0f);
            GameManager.Instance.GetCoin();
            anim.SetTrigger("Collected");
            //Destroy(gameObject, 1.5f);
        }
    }
}
