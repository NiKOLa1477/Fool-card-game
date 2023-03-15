using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private List<Image> PlNameBgs;
    [SerializeField] private TMP_Text inDeckCount;
    [SerializeField] private List<string> trumpNames;
    [SerializeField] private Button Take, Toss, Pass, Restart;

    public void UpdDeckCount(int value, CardType trump) 
    { 
        inDeckCount.text = (value != 0) ? value.ToString() : trumpNames[(int) trump];         
    }
    public void UpdPlayers(int currPlInd, int currEnInd)
    {
        for (int i = 0; i < PlNameBgs.Count; i++)
        {
            if (i == currPlInd)
                PlNameBgs[i].color = Color.green;
            else if(i == currEnInd)
                PlNameBgs[i].color = Color.red;
            else
                PlNameBgs[i].color = Color.clear;
        }
    }
    public void ActTakeBtn()
    {
        Take.gameObject.SetActive(true);
        Toss.gameObject.SetActive(false);
        Pass.gameObject.SetActive(false);
    }
    public void ActTossBtn()
    {
        Take.gameObject.SetActive(false);
        Toss.gameObject.SetActive(true);
        Pass.gameObject.SetActive(false);
    }
    public void ActPassBtn()
    {
        Take.gameObject.SetActive(false);
        Toss.gameObject.SetActive(false);
        Pass.gameObject.SetActive(true);
    }
    public void DeactAllBtns(bool gameEnded = false)
    {
        Take.gameObject.SetActive(false);
        Toss.gameObject.SetActive(false);
        Pass.gameObject.SetActive(false);
        if (gameEnded) Restart.gameObject.SetActive(true);
    }
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void Exit()
    {
        Application.Quit();
    }

    private void Awake()
    {
        trumpNames=new List<string>();
        trumpNames.Add("Club");
        trumpNames.Add("Diamond");
        trumpNames.Add("Heart");
        trumpNames.Add("Spade");        
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Exit();
    }
}
