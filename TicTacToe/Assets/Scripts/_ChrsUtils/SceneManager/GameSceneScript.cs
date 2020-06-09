﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;


public class GameSceneScript : Scene<TransitionData>
{
    public enum PlayerNum{PLAYER1 = 0, PLAYER2}
    public bool endGame;

    public static bool hasWon { get; private set; }

    public const int LEFT_CLICK = 0;
    public const int RIGHT_CLICK = 1;

    TaskManager _tm = new TaskManager();

    public GameBoard board;

    public Player[] players;

    public int currentPlayerIndex;
    public Player currentPlayer { get; private set; }

    [SerializeField] private TextMeshProUGUI _turnIndicator;
    public TextMeshProUGUI TurnIndicator
    {
        get { return _turnIndicator; }
    }

    [SerializeField] private Image _turnIndicatorIcon;
    [SerializeField] private Image _homeButtonIcon;
    [SerializeField] private Image _replayButtonIcon;
    [SerializeField] private SpriteRenderer _gradient;

    private Color _transparent = new Color(1, 1, 1, 0);
    private Color _iconGray = new Color(108 / 255f, 108 / 255f, 108 / 255f);

    internal override void OnEnter(TransitionData data)
    {
        Services.GameScene = this;
        players = new Player[Services.GameManager.TotalPlayers];
        for (int i = 0; i < Services.GameManager.TotalPlayers; i++)
        {
            players[i] = Instantiate(Services.Prefabs.Player, Vector3.zero, Quaternion.identity, transform);
            int playerNum = i + 1;
            players[i].name = "Player " + playerNum;
            Color[] colors;
            switch(i)
            {
                case 0: colors = Services.GameManager.Player1Color; break;
                case 1: colors = Services.GameManager.Player2Color; break;
                default: colors = Services.GameManager.Player1Color; break;
            }
            players[i].Init(playerNum, Services.GameManager.AvailableIcons[i], colors);
        }

        board = GetComponent<GameBoard>();
        BoardInfo info;
        info.row = 3;
        info.col = 3;


        board.Init(info);

        currentPlayerIndex = UnityEngine.Random.Range(0, 2);
        currentPlayer = players[currentPlayerIndex];

        if(currentPlayerIndex == 0)
        {
            currentPlayerIndex = (int)PlayerNum.PLAYER1;
            _turnIndicatorIcon.sprite = players[(int)PlayerNum.PLAYER1].PlayerIcon;
        }
        else
        {
            currentPlayerIndex = (int)PlayerNum.PLAYER2;
            _turnIndicatorIcon.sprite = players[(int)PlayerNum.PLAYER2].PlayerIcon;
        }

        Task fadeIndicatorIconTask = new LERPColor(_turnIndicatorIcon, _transparent, currentPlayer.playerColor[0], 3f);
        Task fadeIndicatorTextTask = new LERPColor(_turnIndicator, _transparent, currentPlayer.playerColor[0], 3f);
        Task fadeHomeButtonTask = new LERPColor(_homeButtonIcon, _transparent, _iconGray, 1f);
        Task fadeReplayButtonTask = new LERPColor(_replayButtonIcon, _transparent, _iconGray, 1f);


        TaskTree uiEntryTask = new TaskTree(new EmptyTask(), 
                                                new TaskTree(fadeIndicatorIconTask),
                                                new TaskTree(fadeIndicatorTextTask),
                                                new TaskTree(fadeHomeButtonTask),
                                                new TaskTree(fadeReplayButtonTask));

        _tm.Do(uiEntryTask);

        Services.EventManager.Register<PlayMadeEvent>(OnPlayMade);
        Services.EventManager.Register<GameEndEvent>(OnGameEnd);
    }

    internal override void OnExit()
    {
        Services.EventManager.Unregister<PlayMadeEvent>(OnPlayMade);
        Services.EventManager.Unregister<GameEndEvent>(OnGameEnd);
    }
    public void OnPlayMade(PlayMadeEvent e)
    {
        if(endGame) return;
        if (currentPlayerIndex == (int)PlayerNum.PLAYER1)
        {
            currentPlayerIndex = (int)PlayerNum.PLAYER2;
            _turnIndicator.color = Services.GameManager.Player2Color[0];
            _turnIndicatorIcon.sprite = players[(int)PlayerNum.PLAYER2].PlayerIcon;
            _turnIndicatorIcon.color = Services.GameManager.Player2Color[0];
        }
        else
        {
            currentPlayerIndex = (int)PlayerNum.PLAYER1;
            _turnIndicator.color = Services.GameManager.Player1Color[0];
            _turnIndicatorIcon.sprite = players[(int)PlayerNum.PLAYER1].PlayerIcon;
            _turnIndicatorIcon.color = Services.GameManager.Player1Color[0];
        }

        currentPlayer = players[currentPlayerIndex];
        
    }

    public void OnGameEnd(GameEndEvent e)
    {
        endGame = true;
        if(e.winner != null)
        {
            _turnIndicatorIcon.sprite = e.winner.PlayerIcon;
            _turnIndicatorIcon.color = e.winner.playerColor[0];
            _turnIndicator.color = e.winner.playerColor[0];
            _turnIndicator.text = "    WINS";
            WinnerConfetti winnerConfetti = Instantiate(Services.Prefabs.WinnerConfetti);
            winnerConfetti.Init(e.winner);
            Task fadeGradient = new LERPColor(_gradient, _transparent, e.winner.playerColor[0], 0.75f);
            _tm.Do(fadeGradient);
        }
        else
        {
            _turnIndicatorIcon.color = new Color(0, 0, 0, 0);
            _turnIndicator.color = new Color(127 / 256f, 127 / 256f, 127 / 256f);
            _turnIndicator.text = "TIE GAME";
            Task fadeGradient = new LERPColor(_gradient, _transparent, _iconGray, 0.75f);
            _tm.Do(fadeGradient);
        }
    }

    public void SetCurrentPlayerTurn(Player p)
    {

    }

    public void OnRestartPressed()
    {
        Services.EventManager.Fire(new RefreshGameBaord());

        Task fadeIndicatorIconTask = new LERPColor(_turnIndicatorIcon, _turnIndicatorIcon.color, _transparent, 0.5f);
        Task fadeIndicatorTextTask = new LERPColor(_turnIndicator, _turnIndicator.color,_transparent, 0.5f);
        Task fadeHomeButtonTask = new LERPColor(_homeButtonIcon, _iconGray, _transparent,0.5f);
        Task fadeReplayButtonTask = new LERPColor(_replayButtonIcon, _iconGray, _transparent,0.5f);
        Task fadeGradient = new LERPColor(_gradient, _gradient.color, _transparent, 0.75f);

        TaskTree restartGameTasks = new TaskTree(new EmptyTask(), 
                                                new TaskTree(fadeIndicatorIconTask),
                                                new TaskTree(fadeIndicatorTextTask),
                                                new TaskTree(fadeHomeButtonTask),
                                                new TaskTree(fadeReplayButtonTask),
                                                new TaskTree(fadeGradient),
                                                new TaskTree(new BoardEntryAnimation()),
                                                new TaskTree(new Wait(3),
                                                    new TaskTree(new ActionTask(ResetGameScene))));

        _tm.Do(restartGameTasks);

    }

    public void ResetGameScene()
    {
        Services.Scenes.Swap<GameSceneScript>();
    }

    public void OnHomePressed()
    {
        Services.EventManager.Fire(new RefreshGameBaord());

        Task fadeIndicatorIconTask = new LERPColor(_turnIndicatorIcon, _turnIndicatorIcon.color, _transparent, 0.5f);
        Task fadeIndicatorTextTask = new LERPColor(_turnIndicator, _turnIndicator.color,_transparent, 0.5f);
        Task fadeHomeButtonTask = new LERPColor(_homeButtonIcon, _iconGray, _transparent,0.5f);
        Task fadeReplayButtonTask = new LERPColor(_replayButtonIcon, _iconGray, _transparent,0.5f);
        Task fadeGradient = new LERPColor(_gradient, _gradient.color, _transparent, 0.75f);

        TaskTree returnHomeTasks = new TaskTree(new EmptyTask(), 
                                                new TaskTree(fadeIndicatorIconTask),
                                                new TaskTree(fadeIndicatorTextTask),
                                                new TaskTree(fadeHomeButtonTask),
                                                new TaskTree(fadeReplayButtonTask),
                                                new TaskTree(fadeGradient),
                                                new TaskTree(new BoardEntryAnimation()),
                                                new TaskTree(new Wait(3),
                                                    new TaskTree(new ActionTask(ReturnHome))));
        _tm.Do(returnHomeTasks);
    }

    public void ReturnHome()
    {
        Services.Scenes.Swap<TitleSceneScript>();
    }

    public void ToggleMute()
    {
        Services.GameManager.ToggleMute();
    }
    private void EndGame()
    {
        Services.AudioManager.FadeAudio();

    }

    public void EndTransition()
    {

    }
    
	// Update is called once per frame
	void Update ()
    {
        _tm.Update();
	}
}
