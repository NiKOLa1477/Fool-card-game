using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown players, deck;
    [SerializeField] private GameObject OptionsWind, StatsWind;
    [SerializeField] private TMP_Text Total, Wins, Looses;
    private const int initPlayers = 2;
    private void Awake()
    {
        loadSettings();
        SaveData();
    }

    private void loadSettings()
    {
        if(PlayerPrefs.HasKey("Players"))
        {
            players.value = PlayerPrefs.GetInt("Players") - initPlayers;          
        }
        if(PlayerPrefs.HasKey("Deck"))
        {
            int cards = PlayerPrefs.GetInt("Deck");
            deck.value = (cards == 36) ? 0 : 1;
        }
    }

    private void SaveData()
    {
        PlayerPrefs.SetInt("Players", players.value + initPlayers);
        int cards = (deck.value == 0) ? 36 : 54;
        PlayerPrefs.SetInt("Deck", cards);
    }

    public void Play()
    {
        PlayerPrefs.DeleteKey("LastFool");
        SaveData();
        SceneManager.LoadScene(1);
    }
    public void Options()
    {
        OptionsWind.SetActive(!OptionsWind.activeInHierarchy);
    }
    public void Stats()
    {
        loadStats();
        StatsWind.SetActive(!StatsWind.activeInHierarchy);
    }

    private void loadStats()
    {
        if(PlayerPrefs.HasKey("Total"))
        {
            Total.text = "Total: " + PlayerPrefs.GetInt("Total").ToString();
        }
        else
        {
            Total.text = "Total: 0";
        }
        if (PlayerPrefs.HasKey("Wins"))
        {
            Wins.text = "Wins: " + PlayerPrefs.GetInt("Wins").ToString();
        }
        else
        {
            Wins.text = "Wins: 0";
        }
        if (PlayerPrefs.HasKey("Looses"))
        {
            Looses.text = "Looses: " + PlayerPrefs.GetInt("Looses").ToString();
        }
        else
        {
            Looses.text = "Looses: 0";
        }
    }
    public void clearStats()
    {
        PlayerPrefs.DeleteKey("Total");
        PlayerPrefs.DeleteKey("Wins");
        PlayerPrefs.DeleteKey("Looses");
        loadStats();
    }

    public void Exit()
    {
        Application.Quit();
    }
}
