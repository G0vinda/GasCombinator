using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
   public void LoadCredits()
   {
      SceneManager.LoadScene("CreditScene");
   }
   
   public void LoadMenu()
   {
      SceneManager.LoadScene("MainMenuScene");
   }

   public void CloseGame()
   {
      Application.Quit();
   }
}
