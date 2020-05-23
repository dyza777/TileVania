﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelExit : MonoBehaviour
{
    [SerializeField] float levelLoadDelay = 2f;
    [SerializeField] float LevelExitSlowMofactor = 0.2f;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        StartCoroutine(ExitLevel());
    }

    IEnumerator ExitLevel()
    {
        Time.timeScale = LevelExitSlowMofactor;
        yield return new WaitForSecondsRealtime(levelLoadDelay);
        Time.timeScale = 1f;

        var currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex + 1);
    }
}
