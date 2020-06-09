using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TitleSceneScript : Scene<TransitionData>
{
    public KeyCode startGame = KeyCode.Space;

    public bool hasLoadGame = false;
    [SerializeField]private float SECONDS_TO_WAIT = 0.1f;

    private TaskManager _tm = new TaskManager();

    [SerializeField] private TextMeshProUGUI[] titleText;

    [SerializeField] private TextMeshProUGUI[] buttonText;

    private Color transparent = new Color(0, 0, 0, 0);
    private float t = 0;
    internal override void OnEnter(TransitionData data)
    {

        Services.EventManager.Register<GameLoadEvent>(OnGameLoad);

        for (int i = 0; i < titleText.Length; i++)
        {
            titleText[i].gameObject.SetActive(false);
        }

        buttonText[0].color = new Color(0, 0, 0, 0);

        TaskQueue titleEntryTasks = new TaskQueue();
        Task slideTitleIn = new TitleEntryAnimation(titleText);
        titleEntryTasks.Add(slideTitleIn);


        _tm.Do(titleEntryTasks);
    
    }

    internal override void OnExit()
    {

    }

    public void OnGameLoad(GameLoadEvent e)
    {
        hasLoadGame = true;
    }

    public void StartGame()
    {
        hasLoadGame = false;
        TaskQueue startGameTasks = new TaskQueue();
        Task slideTitleOut = new TitleEntryAnimation(titleText, true);
        Task fadeStartText = new LERPColor(buttonText, buttonText[0].color, transparent, 0.3f);
        Task beginGame = new ActionTask(TransitionToGame);

        startGameTasks.Add(fadeStartText);
        startGameTasks.Add(slideTitleOut);
        startGameTasks.Add(beginGame);

        _tm.Do(startGameTasks);
    }

    public void ToggleMute()
    {
        Services.GameManager.ToggleMute();
    }

    private void TitleTransition()
    {

    }

    private void TransitionToGame()
    {
        Services.Scenes.Swap<GameSceneScript>();
    }

    private void Update()
    {
        _tm.Update();
        if(hasLoadGame)
        {
            t += Time.deltaTime;
            buttonText[0].color = Color.Lerp(transparent, Color.black, Mathf.PingPong(t,1.5f));
        }
    }
}
