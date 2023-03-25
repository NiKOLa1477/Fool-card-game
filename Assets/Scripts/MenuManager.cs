using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown players, deck;
    [SerializeField] private GameObject OptionsWind;
    private const int initPlayers = 2;
    private void Awake()
    {
        SaveData();
    }
    private void SaveData()
    {
        PlayerPrefs.SetInt("Players", players.value + initPlayers);
        int cards = (deck.value == 0) ? 36 : 54;
        PlayerPrefs.SetInt("Deck", cards);
    }

    public void Play()
    {
        SaveData();
        SceneManager.LoadScene(1);
    }
    public void Options()
    {
        OptionsWind.SetActive(!OptionsWind.activeInHierarchy);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
