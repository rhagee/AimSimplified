using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    public Camera firePoint;
    private GameStats stats;

    private RunHandler.GameState state;
    private RunHandler master;

    private void Awake()
    {
        GameObject x = GameObject.FindGameObjectWithTag("Master");
        stats = x.GetComponent<GameStats>();
        master = x.GetComponent<RunHandler>();
    }

    private void Update()
    {
        state = master.state;
        if (state == RunHandler.GameState.Running)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                Shoot();
            }
        }
        
    }

    private void Shoot()
    {
        RaycastHit hit;
    

        //RayCast to simulate a Shoot and get what i "hit".
        //Parameters are : From what position? Where to send it? Where to store the result? How Long is it?
        if (Physics.Raycast(firePoint.transform.position, firePoint.transform.forward, out hit, 1000))
        {
               
            //Getting the Tag of the Hitted Target
            if(hit.collider.CompareTag("Target"))
            {
                //It is a Target so Break it
                stats.Hit(5,hit.collider.gameObject);
            }
            else
            {
                stats.Miss(2);
            }
        }
    }
}
