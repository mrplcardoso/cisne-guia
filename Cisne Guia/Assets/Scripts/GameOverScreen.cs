using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScreen : MonoBehaviour
{
    void Start()
    {
      StartCoroutine(ToTitle());
    }

  IEnumerator ToTitle()
  {
    yield return new WaitForSeconds(3f);
    SceneManager.LoadScene("MainMenu");
  }
}
