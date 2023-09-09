//Project Day Start : 06/09/2023 - approx 0 knowledge of Unity

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class RunHandler : MonoBehaviour
{

    public enum GameState
    {
        Start,
        Restart,
        Pause,
        Running
    }


    public GameObject target;
    public Canvas menu;
    public Canvas settings;
    private bool settingsOpen;
    public GameObject resumeBtn;
    public TextMeshProUGUI startText;
    public int minTargets;
    public GameState state;
    GameStats stats;

    private int v_min = 10;
    private int v_max = 30;
    private int h_min = -15;
    private int h_max = 15;

    private int targetsOnline = 0;

    GameObject player;

    public TMP_Dropdown resDropDown;
    private Resolution[] resolutions;





    public void Awake()
    {
        minTargets = 5;
        Screen.fullScreen = true;
        resolutions = Screen.resolutions;
        resDropDown.ClearOptions();
        List<String> options = new List<String>();
        int currRes = 0;

        for(int i=0;i<resolutions.Length; i++)
        {
            
            options.Add(resolutions[i].width + " x " + resolutions[i].height + " " + resolutions[i].refreshRateRatio.ToString() +"Hz");
            
            if(Screen.currentResolution.width == resolutions[i].width && Screen.currentResolution.height == resolutions[i].height && Screen.currentResolution.refreshRateRatio.ToString() == resolutions[i].refreshRateRatio.ToString())
            {
                currRes = i;
            }
        }
        resDropDown.AddOptions(options);
        resDropDown.value = currRes; 

        settingsOpen = false;
        settings.gameObject.SetActive(false);
        stats = GetComponent<GameStats>();
        stats.HideScoreAccuracy(true);
        menu.gameObject.SetActive(true);
        player = GameObject.FindGameObjectWithTag("Player");
        OnPause();
        resumeBtn.SetActive(false);
        state = GameState.Start;
    }

    public void Reset()
    {
        //Destroy Remaining Targets
        GameObject[] x = GameObject.FindGameObjectsWithTag("Target");
        int currTargets = x.Length;
        for (int i = 0; i < currTargets; i++)
        {
            Destroy(x[i]);
            targetsOnline = 0;
        }

        //Trigger Reset in GameStats
        this.GetComponent<GameStats>().Reset();


        OnPause();
        resumeBtn.SetActive(false);
        state = GameState.Restart;


        //Reset Targets
        targetsOnline = 0;

        //Generate Random Seed
        UnityEngine.Random.InitState(System.DateTime.Now.Millisecond);

        //Generate minTargets targets
        while (targetsOnline < minTargets)
        {
            GenerateTargets();
        }


    }

    public void OnFullReset()
    {
        startText.text = "Start";
        stats.FullReset();
        Reset();
    }

    public void GameStart()
    {
        startText.text = "Restart";
        Reset();
        OnResume();
    }

    public void GameOver()
    {
        startText.text = "Start";
        Reset();
        stats.HideScoreAccuracy(false);
    }


    //YZPosition RandomObj
    private class YZPosition
    {
        
        private float minY;
        private float minZ;
        private float maxY;
        private float maxZ;

        private float Y;
        private float Z;

        public YZPosition(float minY, float minZ, float maxY, float maxZ)
        {
            this.maxY = maxY;
            this.maxZ = maxZ;
            this.minY = minY;
            this.minZ = minZ;
        }

        public float getY() => Y;
        public float getZ() => Z;

        public void GenerateRandomCoordinates()
        {
            Y = UnityEngine.Random.Range(minY, maxY);
            Z = UnityEngine.Random.Range(minZ, maxZ);
        }

        private float getMiddle (float min, float max)
        {
            return (max - min / 2) + min;
        }
        //0 : top left , 1 : top right , 2 : bottom left , 3 : bottom right
        public void GenerateRandomCoordinates(int zone)
        {
            switch (zone)
            {
                case 0:
                    Y = UnityEngine.Random.Range(getMiddle(minY,maxY), maxY);
                    Z = UnityEngine.Random.Range(minZ, getMiddle(minZ,maxZ));
                    break;
                case 1:
                    Y = UnityEngine.Random.Range(getMiddle(minY,maxY), maxY);
                    Z = UnityEngine.Random.Range(getMiddle(minZ,maxZ), maxZ);
                    break;
                case 2:
                    Y = UnityEngine.Random.Range(minY, getMiddle(minY, maxY));
                    Z = UnityEngine.Random.Range(minZ, getMiddle(minZ, maxZ) );
                    break;
                default:
                    Y = UnityEngine.Random.Range(minY, getMiddle(minY, maxY));
                    Z = UnityEngine.Random.Range(getMiddle(minZ, maxZ),maxZ);
                    break;
            }
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (state == GameState.Running)
        {
            CheckPause();
            UnityEngine.Random.InitState(System.DateTime.Now.Millisecond);
            GenerateTargets();
        }
    }


    private void CheckPause ()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnPause();
        }
    }

    public void OnPause()
    {
        stats.HideScoreAccuracy(true);
        resumeBtn.SetActive(true);
        state = GameState.Pause;
        menu.gameObject.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void OnResume()
    {
        state = GameState.Running;
        menu.gameObject.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void GenerateTargets ()
    {
        if (targetsOnline < minTargets)
        {
            YZPosition spawner = new YZPosition(v_min, h_min, v_max, h_max);
            spawner.GenerateRandomCoordinates();
            
            Instantiate(target, new Vector3(-25, spawner.getY(), spawner.getZ()), new Quaternion(0, 0, 0, 0));
            targetsOnline++;
        }
    }

    public void TargetDestroyed()
    {
        targetsOnline--;
    }

    public void QuitGame()
    {
       Application.Quit();
    }

    public void Settings()
    {
        settingsOpen = !settingsOpen;
        settings.gameObject.SetActive(settingsOpen);
    }

    public void OnFullScreenChange(bool newValue)
    {
        Screen.fullScreen = newValue;  
    }

    public void OnResolutionChange(int i)
    {
        Resolution res = resolutions[i];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
       
    }

    public void MinTargetsChange(String x)
    {
        minTargets = int.Parse(x);
        startText.text = "Start";
        Reset();
        stats.Reset();
    }
}
