using UnityEngine;

[RequireComponent(typeof(UnityChan2DController), typeof(AudioSource))]
public class Demo : MonoBehaviour
{
    [SerializeField]
    private AudioClip damageVoice = null;

    [SerializeField]
    private AudioClip[] jumpVoices = null;

    [SerializeField]
    private AudioClip clearVoice = null;

    [SerializeField]
    private AudioClip timeOverVoice = null;

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void OnDamage()
    {
        PlayVoice(damageVoice);
    }

    void Jump()
    {
        int i = Random.Range(0, jumpVoices.Length);
        PlayVoice(jumpVoices[i]);
    }

    void Clear()
    {
        PlayVoice(clearVoice);
    }

    void TimeOver()
    {
        PlayVoice(timeOverVoice);
    }

    void PlayVoice(AudioClip voice)
    {
        audioSource.Stop();
        audioSource.PlayOneShot(voice);
    }
}