 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HUDControl : MonoBehaviour
{
    [HideInInspector] public StateMachine stateMachine = new StateMachine();

    public GameStateControl stateCon;
    PlayerControl player;
    Stats stats;

    public GameObject menuPanel;
    public GameObject systemPanel;
    public GameObject comingSoonPanel;
    public GameObject characterWindow;
    public Text enduranceText;
    public Text woundsText;

    [SerializeField] Text logText;

    void Awake()
    {
        stateMachine.ChangeState(new ClosedState(this));

        stateCon = FindObjectOfType<GameStateControl>();

        WriteToLog("Your Battle Begins!");
    }

    void Update()
    {
        stateMachine.Update();
        player = stateCon.activePlayer.GetComponent<PlayerControl>();
        stats = stateCon.activePlayer.GetComponent<Stats>();

        enduranceText.text = "Endurance: " + player.curEndurance + "/" + stats.endurance;
        woundsText.text = "Wounds: " + player.curWounds + "/" + stats.wounds;
    }

    void FixedUpdate()
    {
        stateMachine.FixedUpdate();
    }

    public void WriteToLog(string text)
    {
        logText.text += "\n" + text;
    }

    public void Resume()
    {
        stateMachine.ChangeState(new ClosedState(this));
    }

    public void Restart()
    {
        SceneManager.LoadScene("New Scene");
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void SystemOpen()
    {
        stateMachine.ChangeState(new SystemState(this));
    }

    public void ComingSoonOpen()
    {
        stateMachine.ChangeState(new ComingSoonState(this));
    }
}
