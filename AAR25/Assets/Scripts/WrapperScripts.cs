using Oculus.Interaction;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class WrapperScripts : MonoBehaviour
{
    public GameObject menu;
    public AudioSource audioSource; 
    public AudioClip selectSound;
    private void Start()
    {

    }

    public void Select()
    {
        if (audioSource != null && selectSound != null)
        {
            audioSource.PlayOneShot(selectSound);
        }

        menu.GetComponent<ColorHolder>().selectedColor = gameObject.GetComponentInChildren<RoundedBoxProperties>().Color;
    }

    // public void Unselect()
    // {
    //     
    // }
}