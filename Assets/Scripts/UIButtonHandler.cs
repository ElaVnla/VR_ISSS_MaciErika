using UnityEngine;

public class UIButtonHandler : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip clickSound;

    public void OnButtonPressed()
    {
        Debug.Log("BUTTON PRESSED!");

        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }
}