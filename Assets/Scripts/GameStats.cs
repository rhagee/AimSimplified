using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using TMPro.Examples;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Rendering;
using UnityEngine.UI;
using static Unity.VisualScripting.Member;

public class GameStats : MonoBehaviour
{


    private int score;
    private float accuracy;

    private int shooted;
    private int hitted;
    private float timer;

    public float default_timer = 30;
    
    public TextMeshProUGUI score_text;
    public TextMeshProUGUI time_text;
    public TextMeshProUGUI accuracy_text;
    public TextMeshProUGUI rest_timer;

    public TextMeshProUGUI bestScore_text;
    public TextMeshProUGUI avarageScore_text;
    public TextMeshProUGUI actualScore_text;
    public TextMeshProUGUI actualAcc_text;
    public TextMeshProUGUI actualScore_lbl;
    public TextMeshProUGUI actualAcc_lbl;

    private int[] scores;
    private int runs;
    private int i;
    private int highestScore;
    private int avarageScore;

    private bool restPhase;


    private bool isPaused;

    RunHandler.GameState state;
    RunHandler master;


    // Start is called before the first frame update
    void Start()
    {
        default_timer = 30;
        timer = 0;
        runs = 0;
        highestScore = 0;
        scores = new int[20];
        rest_timer.gameObject.SetActive(false);
        avarageScore = 0;
        bestScore_text.text = "0";
        avarageScore_text.text = "0";
        actualScore_text.text = "0";
        actualAcc_text.text = "0";
        master = GetComponent<RunHandler>();
    }

    public void Reset()
    {
        restPhase = true;
        timer = 3;
        rest_timer.gameObject.SetActive(true);
        score = 0;
        score_text.text = score.ToString();
        shooted = 0;
        hitted = 0;
        CalculateAccuracy();
    }

    private void Update()
    {
        state = master.state;
        if (state == RunHandler.GameState.Running)
        {
            if (timer > 0)
                timer -= Time.deltaTime;
            else
            {
                if (restPhase)
                {
                    rest_timer.gameObject.SetActive(false);
                    score_text.text = score.ToString();
                    CalculateAccuracy();
                    timer = default_timer;
                    restPhase = !restPhase;
                }
                else
                {
                    //Best solution to improve performance : use a Queue (add on back, change head to second, and cancel useless first data, so no iteration/copy is needed)
                    //But Array.Copy is fast enaugh to copy 20 data, so am not really looking forward to implement it losing time
                    if(i==20)
                    {
                        int[] temp_copy = new int[20];

                        //Copy array scores starting from Pos 1 , into temp array starting from pos 0, for the length of score - 1 (since last)
                        Array.Copy(scores, 1, temp_copy, 0, scores.Length - 1);
                        i--;
                        //Like this we are doing like a Shift Left canceling the most far result
                        scores = temp_copy;

                        //Now we can store in the position 20-1 = 19 our latest result and keep the last 20 records on track
                        scores[i] = score;
                    }
                    

                    runs++;
                    i++;
                    if (score > highestScore)
                        highestScore = score;
                    avarageScore = Mathf.FloorToInt((avarageScore * (runs - 1) + score) / runs);


                    actualScore_text.text = score.ToString();
                    actualAcc_text.text = (Math.Round(accuracy, 1)).ToString() + "%";
                    bestScore_text.text = highestScore.ToString();
                    avarageScore_text.text = avarageScore.ToString();


                    GetComponent<RunHandler>().GameOver();
                    //END GAME
                }
                
            }

            if (!restPhase)
            {
                TimeToDisplay(timer);
            }
            else
            {
                time_text.color = Color.white;
                time_text.text = "Rest";
                rest_timer.text =(Mathf.FloorToInt(timer)).ToString();
            }
        }
    }


    public void HideScoreAccuracy(bool value)
    {
            actualAcc_lbl.enabled = !value;
            actualScore_lbl.enabled = !value;
            actualAcc_text.enabled = !value;
            actualScore_text.enabled = !value;
    }

    private void TimeToDisplay(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        if(minutes<1 && seconds<10)
        {
            if (seconds % 2 == 0)
                time_text.color = Color.red;
            else
                time_text.color = Color.white;
        }
        time_text.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void Miss(int value)
    {
        if (!restPhase && state == RunHandler.GameState.Running)
        {
            shooted++;
            score = Mathf.Max(0, score - value);
            CalculateAccuracy();
            score_text.text = score.ToString();
        }
    }

    public void Hit(int value,GameObject go)
    {
        if (!restPhase && state == RunHandler.GameState.Running)
        {
            Destroy(go);
            this.GetComponent<RunHandler>().TargetDestroyed();
            shooted++;
            hitted++;
            score = Mathf.Max(0, score + value);
            score_text.text = score.ToString();
            CalculateAccuracy();
        }
       
        
    }

    public void FullReset()
    {
        Start();
    }

    private void CalculateAccuracy()
    {
        if (shooted > 0)
            accuracy = hitted*100f/shooted;
        else
            accuracy = 100f;

        accuracy_text.text = (Math.Round(accuracy,1)).ToString()+"%";
    }

}
