using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;
using Rnd = UnityEngine.Random;
using Math = ExMath;

public class GuessTheFarewellRoom : MonoBehaviour {

   public KMBombInfo Bomb;
   public KMAudio Audio;
   public KMNeedyModule Needy;

   public KMSelectable LeftCPButton;
   public KMSelectable RightCPButton;
   public KMSelectable LeftNumberButton;
   public KMSelectable RightNumberButton;
   public TextMesh CPDisplay;
   public TextMesh NumberDisplay;
   public Renderer MainScreen;

   string GeneratedCheckpoint;
   int GeneratedNumber;
   int[] CPNumberLimit = new int[] { 14, 9, 13, 18, 14, 7, 18, 2 };
   int GeneratedCheckpointNumber;
   int roomVariation;
   string GeneratedRoom;
   string[] CPNames = new string[] { "SG", "PS", "RM", "EH", "DT", "SB", "RC", "FW" };
   string roomNumberStr;
   string GeneratedRoomFormat;

   int CPChosenRaw = 0;
   int CPChosen;

   int numberChosen = 1;
   string NumberChosen;

   string UserAnswer;

   static int ModuleIdCounter = 1;
   int ModuleId;
   private bool ModuleSolved;

   void Awake () { //Avoid doing calculations in here regarding edgework. Just use this for setting up buttons for simplicity.
        ModuleId = ModuleIdCounter++;
        Needy.OnNeedyActivation += OnNeedyActivation;
        Needy.OnNeedyDeactivation += OnNeedyDeactivation;
        Needy.OnTimerExpired += OnTimerExpired;

        LeftCPButton.OnInteract += delegate () { LeftCPPress(); return false; };
        RightCPButton.OnInteract += delegate () { RightCPPress(); return false; };
        LeftNumberButton.OnInteract += delegate () { LeftNumberPress(); return false; };
        RightNumberButton.OnInteract += delegate () { RightNumberPress(); return false; };

    }

    void LeftCPPress ()
    {
        CPChosenRaw--;
        CPChosen = ((CPChosenRaw % 8) + 8) % 8;
        GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
        CPDisplay.text = CPNames[CPChosen];
    }

    void RightCPPress()
    {
        CPChosenRaw++;
        CPChosen = ((CPChosenRaw % 8) + 8) % 8;
        GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
        CPDisplay.text = CPNames[CPChosen];
    }

    void LeftNumberPress()
    {
        numberChosen--;
        if (numberChosen == 19)
        {
            NumberChosen = "01";
            numberChosen = 1;
        }
        else if (numberChosen == 0)
        {
            NumberChosen = "18";
            numberChosen = 18;
        }
        else 
        {
            NumberChosen = numberChosen.ToString("D2");
        }
        NumberDisplay.text = NumberChosen;
        GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
    }

    void RightNumberPress()
    {
        numberChosen++;
        if (numberChosen == 19)
        {
            NumberChosen = "01";
            numberChosen = 1;
        }
        else if (numberChosen == 0)
        {
            NumberChosen = "18";
            numberChosen = 18;
        }
        else
        {
            NumberChosen = numberChosen.ToString("D2");
        }
        NumberDisplay.text = NumberChosen;
        GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
    }

    void OnDestroy () { //Shit you need to do when the bomb ends
        MainScreen.material.mainTexture = Resources.Load<Texture2D>("RectangularShadow");
        Needy.OnPass();
    }

   void Activate () { //Shit that should happen when the bomb arrives (factory)/Lights turn on

   }

   protected void OnNeedyActivation () { //Shit that happens when a needy turns on.
        GenerateRandomRoom();
        ShowRoomImage(GeneratedRoom);
   }

   protected void OnNeedyDeactivation () { //Shit that happens when a needy turns off.
        MainScreen.material.mainTexture = Resources.Load<Texture2D>("RectangularShadow");
    }

    protected void OnTimerExpired()
    { //Shit that happens when a needy turns off due to running out of time.
        MainScreen.material.mainTexture = Resources.Load<Texture2D>("RectangularShadow");
        NumberChosen = numberChosen.ToString("D2");
        UserAnswer = CPNames[CPChosen] + "-" + NumberChosen;
        Debug.LogFormat("[Guess The Farewell Room #{0}] User Answer: {1}", ModuleId, UserAnswer);
        if (UserAnswer == GeneratedRoomFormat)
        {
            Needy.OnPass();
            Debug.LogFormat("[Guess The Farewell Room #{0}] User guessed correctly. Module Pass.", ModuleId);
        }
        else
        {
            Strike();
            Debug.LogFormat("[Guess The Farewell Room #{0}] User guessed incorrectly. The room was {1}.", ModuleId, GeneratedRoomFormat);
        }
    }

   void Start () { //Shit that you calculate, usually a majority if not all of the module
        Needy.SetResetDelayTime(60f, 120f);
   }

   void Update () { //Shit that happens at any point after initialization

   }

   void Strike () {
      Needy.HandleStrike();
   }

    void GenerateRandomRoom()
    {
        //CP
        switch (Rnd.Range(0, 8))
        {
            case 0:
                GeneratedCheckpoint = "SG";
                GeneratedCheckpointNumber = 0;
                break;
            case 1:
                GeneratedCheckpoint = "PS";
                GeneratedCheckpointNumber = 1;
                break;
            case 2:
                GeneratedCheckpoint = "RM";
                GeneratedCheckpointNumber = 2;
                break;
            case 3:
                GeneratedCheckpoint = "EH";
                GeneratedCheckpointNumber = 3;
                break;
            case 4:
                GeneratedCheckpoint = "DT";
                GeneratedCheckpointNumber = 4;
                break;
            case 5:
                GeneratedCheckpoint = "SB";
                GeneratedCheckpointNumber = 5;
                break;
            case 6:
                GeneratedCheckpoint = "RC";
                GeneratedCheckpointNumber = 6;
                break;
            case 7:
                GeneratedCheckpoint = "FW";
                GeneratedCheckpointNumber = 7;
                break;
        }

        //Room
        GeneratedNumber = Rnd.Range(1, (CPNumberLimit[GeneratedCheckpointNumber] + 1));
        roomNumberStr = GeneratedNumber.ToString("D2");

        //Variation
        roomVariation = Rnd.Range(1, 3);

        GeneratedRoom = GeneratedCheckpoint + "-" + roomNumberStr + " " + roomVariation;
        GeneratedRoomFormat = GeneratedCheckpoint + "-" + roomNumberStr;

        Debug.LogFormat("[Guess The Farewell Room #{0}] Generated Room: {1}", ModuleId, GeneratedRoomFormat);

    }

    void ShowRoomImage(string imageName)
    {
        Texture2D tex = Resources.Load<Texture2D>("Farewell Rooms/" + imageName);
        MainScreen.material.mainTexture = tex;
    }

#pragma warning disable 414
    private readonly string TwitchHelpMessage = @"Use '!{0} guess <checkpoint>-<number>' to input your answer.";
#pragma warning restore 414

   IEnumerator ProcessTwitchCommand (string Command) {
        Command = Command.ToLowerInvariant().Trim();
        yield return null;

        Match match = Regex.Match(Command, @"^guess\s+(sg|ps|rm|eh|dt|sb|rc|fw)-(0?[1-9]|1[0-8])$");

        if (!match.Success)
            yield break;

        string checkpoint = match.Groups[1].Value;
        string number = match.Groups[2].Value;

        //cp
        if (checkpoint == "sg")
        {
            CPChosenRaw = 0;
            CPDisplay.text = CPNames[CPChosenRaw];
            GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);

        }
        else if (checkpoint == "ps")
        {
            CPChosenRaw = 1;
            CPDisplay.text = CPNames[CPChosenRaw];
            GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);

        }
        else if (checkpoint == "rm")
        {
            CPChosenRaw = 2;
            CPDisplay.text = CPNames[CPChosenRaw];
            GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);

        }
        else if (checkpoint == "eh")
        {
            CPChosenRaw = 3;
            CPDisplay.text = CPNames[CPChosenRaw];
            GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);

        }
        else if (checkpoint == "dt")
        {
            CPChosenRaw = 4;
            CPDisplay.text = CPNames[CPChosenRaw];
            GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);

        }
        else if (checkpoint == "sb")
        {
            CPChosenRaw = 5;
            CPDisplay.text = CPNames[CPChosenRaw];
            GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);

        }
        else if (checkpoint == "rc")
        {
            CPChosenRaw = 6;
            CPDisplay.text = CPNames[CPChosenRaw];
            GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);

        }
        else if (checkpoint == "fw")
        {
            CPChosenRaw = 7;
            CPDisplay.text = CPNames[CPChosenRaw];
            GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);

        }
        CPChosen = ((CPChosenRaw % 8) + 8) % 8;
        yield return null;
        //number
        numberChosen = int.Parse(number);
        NumberChosen = numberChosen.ToString("D2");    
        NumberDisplay.text = NumberChosen;
        GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);

}

   void TwitchHandleForcedSolve () { //Void so that autosolvers go to it first instead of potentially striking due to running out of time.
      StartCoroutine(HandleAutosolver());
   }

   IEnumerator HandleAutosolver () {
      yield return null;
      CPChosenRaw = GeneratedCheckpointNumber;
      CPDisplay.text = CPNames[CPChosenRaw];
      GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
      yield return null;
      numberChosen = GeneratedNumber;
      NumberChosen = numberChosen.ToString("D2");
      NumberDisplay.text = NumberChosen;
      GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);

    }
}
