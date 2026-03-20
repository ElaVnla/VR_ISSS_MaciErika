using UnityEngine;

public class DebrisBag : MonoBehaviour
{
    public string acceptedTag; // set per bag

    public ParticleSystem correctEffect;
    public ParticleSystem wrongEffect;

    public AudioSource audioSource;
    public AudioClip correctSound;
    public AudioClip wrongSound;

    private void OnTriggerEnter(Collider other)
    {
        // Only react to debris
        if (!(other.CompareTag("SmallDebris") ||
              other.CompareTag("MediumDebris") ||
              other.CompareTag("LargeDebris")))
            return;

        Debug.Log("Entered bag: " + other.name);

        if (other.CompareTag(acceptedTag))
        {
            Debug.Log("CORRECT");

            if (correctEffect != null)
                correctEffect.Play();

            if (audioSource != null && correctSound != null)
                audioSource.PlayOneShot(correctSound);

            Destroy(other.gameObject);
        }
        else
        {
            Debug.Log("WRONG");

            if (wrongEffect != null)
                wrongEffect.Play();

            if (audioSource != null && wrongSound != null)
                audioSource.PlayOneShot(wrongSound);
        }
    }
}