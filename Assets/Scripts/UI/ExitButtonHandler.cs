using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitButtonHandler : MonoBehaviour
{
    public void ExitGame()
    {
        Debug.Log("Exiting the game!"); // Logs the exit action in the Editor
        Application.Quit(); // Closes the application
    }
}
