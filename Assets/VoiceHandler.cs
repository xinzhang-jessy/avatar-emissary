using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Oculus.Voice;
using Facebook.WitAi.TTS.Utilities;

public class VoiceHandler : MonoBehaviour
{
    public TextMeshProUGUI output;
    public TextMeshProUGUI input;
    public TextMeshProUGUI debug;
    public TextMeshProUGUI timerText;
    public AppVoiceExperience _voiceExperience;
    public Animator animator;
    public TTSSpeaker speaker;
    private bool isTimerOn;
    private float timer;

    private int state; // 0 - init; 1 - stage1; 2 - stage2;

    // Start is called before the first frame update
    void Start()
    {
        this.state = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (isTimerOn)
        {
            timer += Time.deltaTime;
            timerText.text = timer.ToString("0.00s");
        }
        if (timer >= 1 * 60)
        {
            state = 0;
            speaker.Speak("times up, please start from the beginning");
            output.text = "times up, please start from the beginning";
            isTimerOn = false;
            timer = 0;
            timerText.text = timer.ToString("0.00s");
        }
        if (OVRInput.Get(OVRInput.Button.One))
        {
            animator.SetInteger("state", 1);
            if (state == 0) {
                speaker.Speak("Hello, nice to meet you. I am virtual assistant, What can I do for you?");
                output.text = ("Hello, nice to meet you. I am virtual assistant, What can I do for you?");
                
                isTimerOn = true;
                timer = 0;
                timerText.text = timer.ToString("0.00s");
            }
            debug.text = "activate";
            _voiceExperience.Activate();
        }
        if (OVRInput.Get(OVRInput.Button.Two))
        {
            animator.SetInteger("state", 0);
            speaker.Stop();
            debug.text = "deactivate, reset state to 0";
            state = 0;
            isTimerOn = false;
            timer = 0;
            timerText.text = timer.ToString("0.00s");

        }
    }

    // event: start listening
    public void OnStartListen()
    {
        Debug.Log("start listening");
        debug.text = "start listening";
    }

    // event: stop listening
    public void OnStopListen()
    {
        Debug.Log("stop listening ");
        debug.text += " (stop listening)";
    }

    // event: on partial transcription
    public void OnPartialTranscription(string s)
    {
        Debug.Log("got: " + s);
        debug.text = "got: " + s;
    }

    public void OnStage1Answer(string[] vals)
    {
        var yesOrNo = vals[0];
        output.text = string.Format("yesOrNo={0}", yesOrNo);
        if (state == 1)
        {
            if (yesOrNo.ToLower() == "yes")
            {
                speaker.Speak("Perfect, I have two options.");
                speaker.Speak("Option one let's do the breathing and try to calm down.");
                speaker.Speak("Option two let me tell you a story and do the focus changing.");
                speaker.Speak("Perfect, I have two options, which one would you like to try?");

                state = 2;
                output.text = "Perfect, I have two options, which one would you like to try?" + yesOrNo;
            }
            else if (yesOrNo.ToLower()  == "no")
            {
                speaker.Speak("It's fine if you don not want to try.Now that's the end of this timeout");
              
                state = 2;
                output.text = "It's fine if you don not want to try.Now that's the end of this timeout" + yesOrNo;
            }
        }
    }
    
    public void OnStage2Answer(string[] vals)
    {
        var options = vals[0];
        output.text = options;
        if (state == 2)
        {
            if (options.ToLower() == "option one")
            {
                speaker.Speak("ok, let's do breathing and calm down");
                output.text = "ok, let's do breathing and calm down";
            }
            if (options.ToLower() == "option two")
            {
                speaker.Speak("All right, now let me tell you a story");
                output.text = "All right, now let me tell you a story";
            }
        }
    }

    public void OnGiveContext(string[] vals)
    {
        var name = vals[0];
        var activity = vals[1];
        var reason = vals[2];
        var duration = vals[3];

        output.text = string.Format("name={0}, actvity={1}, reason={2}, duration={3}", name, activity, reason, duration);
        if (state == 0)
        {
            
            if (name != "")
            {
                animator.SetInteger("state", 2);
                // I understand that you are feeling upset and frustrated, but it is important to remember thatyelling and arguing are not the way to solve problems.
                var greeting = "Hello " + name + ", would you like to start the timeout process?";
                //var question = "Here are some ways to deal with your timeout, would you like to try, "+name+" ? ";
             
                speaker.Speak(greeting);
                //speaker.Speak(question);
                state = 1;
                output.text = greeting;
            }
            else
            {
                debug.text += "name is empty";
            }
        }
        

    }
}

