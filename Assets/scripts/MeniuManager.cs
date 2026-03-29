using UnityEngine;
using UnityEngine.SceneManagement; // OBLIGATORIU pentru schimbat scene

public class MeniuManager : MonoBehaviour
{
    public void PornesteJocul()
    {
        // Înlocuiește "NumeScenaTa" cu numele exact al scenei tale cu harta
        SceneManager.LoadScene("Main"); 
    }

    public void InchideJocul()
    {
        Application.Quit();
        Debug.Log("Jocul s-a închis!"); // Merge doar în varianta Build, nu în Editor
    }
}