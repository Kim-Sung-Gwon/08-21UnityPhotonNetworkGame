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

    public Text ammoText;   // ź�� �ؽ�Ʈ
    public Text scoreText;  // ���ھ� �ؽ�Ʈ
    public Text waveText;   // ���̺� �ؽ�Ʈ
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
        // �ڱ��ڽ� �� �������� ��Ƽ��(Ȱ��ȭ) �ȴ�.
    }
}
