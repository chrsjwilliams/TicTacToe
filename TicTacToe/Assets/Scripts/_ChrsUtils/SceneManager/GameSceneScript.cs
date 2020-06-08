using System.Collections;
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

        currentPlayerIndex = UnityEngine.Random.Range(0, 1);
        currentPlayer = players[currentPlayerIndex];

        if(currentPlayerIndex == 0)
        {
            currentPlayerIndex = (int)PlayerNum.PLAYER1;
            _turnIndicator.color = Services.GameManager.Player1Color[0];
            _turnIndicatorIcon.sprite = players[(int)PlayerNum.PLAYER1].PlayerIcon;
            _turnIndicatorIcon.color = Services.GameManager.Player1Color[0];
        }
        else
        {
            currentPlayerIndex = (int)PlayerNum.PLAYER2;
            _turnIndicator.color = Services.GameManager.Player2Color[0];
            _turnIndicatorIcon.sprite = players[(int)PlayerNum.PLAYER2].PlayerIcon;
            _turnIndicatorIcon.color = Services.GameManager.Player2Color[0];
        }

        Services.EventManager.Register<PlayMadeEvent>(OnPlayMade);
        Services.EventManager.Register<GameEndEvent>(OnGameEnd);
    }

    public void OnPlayMade(PlayMadeEvent e)
    {
        if(endGame) return;
        if (currentPlayerIndex == 0)
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

        }
        else
        {
            _turnIndicatorIcon.color = new Color(0, 0, 0, 0);
            _turnIndicator.color = new Color(127 / 256f, 127 / 256f, 127 / 256f);
            _turnIndicator.text = "TIE GAME";
        }
    }

    public void SetCurrentPlayerTurn(Player p)
    {

    }

    public void ReturnHome()
    {
        Services.AudioManager.SetVolume(1.0f);
        Services.Scenes.Swap<TitleSceneScript>();
    }

    public void SceneTransition()
    {
        _tm.Do
        (
            new ActionTask(ReturnHome)
        );
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
