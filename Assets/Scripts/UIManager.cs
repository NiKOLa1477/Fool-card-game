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
    [SerializeField] private Button onTake, onToss, onPass, Restart;

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
        onTake.gameObject.SetActive(true);
        onToss.gameObject.SetActive(false);
        onPass.gameObject.SetActive(false);
    }
    public void ActTossBtn()
    {
        onTake.gameObject.SetActive(false);
        onToss.gameObject.SetActive(true);
        onPass.gameObject.SetActive(false);
    }
    public void ActPassBtn()
    {
        onTake.gameObject.SetActive(false);
        onToss.gameObject.SetActive(false);
        onPass.gameObject.SetActive(true);
    }
    public void DeactAllBtns(bool gameEnded = false)
    {
        onTake.gameObject.SetActive(false);
        onToss.gameObject.SetActive(false);
        onPass.gameObject.SetActive(false);
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

    private void swapNames(int i, int j)
    {
        var temp = PlNameBgs[i];
        PlNameBgs[i] = PlNameBgs[j];
        PlNameBgs[j] = temp;
    }
    private void Start()
    {
        var table = FindObjectOfType<Table>();       
        if(table.getPlCount() == 3)
        {
            swapNames(2, 3);
            PlNameBgs[3].gameObject.SetActive(false);
        }
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
