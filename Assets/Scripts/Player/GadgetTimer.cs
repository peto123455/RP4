using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GadgetTimer : MonoBehaviour
{
    public float time, timeLeft;
    public bool active;

    public GameObject timerIns;

    public GadgetTimer(float time, bool active)
    {
        this.time = time;
        this.timeLeft = time;
        this.active = active;
    }

    void Awake()
    {
        timerIns = GameObject.Find("UI").gameObject.transform.GetChild(5).gameObject;
    }

    void Update()
    {
        if(this.active)
        {
            this.timeLeft -= Time.deltaTime;
            timerIns.transform.GetChild(2).gameObject.GetComponent<Text>().text = ((int) this.timeLeft).ToString();
            timerIns.GetComponent<Slider>().value = this.timeLeft;
            if(this.timeLeft <= 0) StopGadgetTimer();
        }
    }

    public void StartGadgetTimer(float time)
    {
        SetToCurrentTime(time);

        timerIns.GetComponent<Slider>().maxValue = time;
        timerIns.GetComponent<Slider>().value = time;
        timerIns.SetActive(true);
        timerIns.transform.GetChild(2).gameObject.GetComponent<Text>().text = time.ToString();

    }

    private void SetToCurrentTime(float time)
    {
        this.time = time;
        this.timeLeft = time;
        this.active = true;
    }

    public void StopGadgetTimer()
    {
        this.active = false;
        timerIns.SetActive(false);
        Player.player.OnGadgetTimerStop();
    }
}
