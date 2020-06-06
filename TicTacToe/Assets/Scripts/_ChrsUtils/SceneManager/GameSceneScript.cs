using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


public class GameSceneScript : Scene<TransitionData>
{
    public bool endGame;

    public static bool hasWon { get; private set; }

    public const int LEFT_CLICK = 0;
    public const int RIGHT_CLICK = 1;

    TaskManager _tm = new TaskManager();

    public GameBoard board;

    public Player[] players;

    public int currentPlayerIndex;
    public Player currentPlayer { get; private set; }

    internal override void OnEnter(TransitionData data)
    {

        players = new Player[Services.GameManager.TotalPlayers];
        for (int i = 0; i < Services.GameManager.TotalPlayers; i++)
        {
            players[i] = Instantiate(Services.Prefabs.Player, Vector3.zero, Quaternion.identity, transform);
            int playerNum = i + 1;
            players[i].name = "Player " + playerNum;
            players[i].Init(playerNum, Services.GameManager.AvailableIcons[i]);
        }

        board = GetComponent<GameBoard>();
        BoardInfo info;
        info.row = 3;
        info.col = 3;
        Services.GameBoard = board;

        board.Init(info);

        currentPlayerIndex = UnityEngine.Random.Range(0, 1);
        currentPlayer = players[currentPlayerIndex];
    }

    public void SwapScene()
    {
        Services.AudioManager.SetVolume(1.0f);
        Services.Scenes.Swap<TitleSceneScript>();
    }

    public void SceneTransition()
    {
        _tm.Do
        (
            new ActionTask(SwapScene)
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
