using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;

    public static UIManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<UIManager>();
            return instance;
        }
    }

    public Text ammoText;   // 탄알 텍스트
    public Text scoreText;  // 스코어 텍스트
    public Text waveText;   // 웨이브 텍스트
    public GameObject gameOverUI;

    void Start()
    {
        
    }

    public void UpdateAmmoText(int magAmmo, int remainAmmo)
    {
        ammoText.text = magAmmo + " / " + remainAmmo;
    }

    public void UpdateScoreText(int newScore)
    {
        scoreText.text = "Score : " + newScore;
    }

    public void UpdateWaveText(int waves, int count)
    {
        waveText.text = "Wave : " + waves + "\n EnmyLeft : " + count;
    }

    public void SetAciveGameOverUI(bool active)
    {
        gameOverUI.SetActive(active);
    }

    public void GameReStart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        // 자기자신 씬 네임으로 액티브(활성화) 된다.
    }
}
