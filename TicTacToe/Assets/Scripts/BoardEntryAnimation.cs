using UnityEngine;

public class BoardEntryAnimation : Task
{
    private float _timeElapsed;
    private const float _animDuration = 1f;
    private const float _staggerTime = 0.3f;
    private float _totalDuration;
    private TileSpace[,] _map { get { return Services.GameScene.board.gameBoard; } }
    private int _largerDimension;
    private int _mapWidth;
    private int _mapHeight;
    private int _currentIndex;
    private bool[,] _tilesOn;

    private static Vector3 _offset = new Vector3(0, 5f, 0);
    private Vector3[,] _basePositions;

    private Color _startColor = new Color(1, 1, 1, 0);
    private Color _targetColor;


    protected override void Init()
    {
        _timeElapsed = 0;
        _currentIndex = 0;
        _mapWidth = _map.GetUpperBound(0)+1;
        _mapHeight = _map.GetUpperBound(1)+1;
        _totalDuration = _animDuration + (_staggerTime * (_mapHeight + _mapWidth));
        _tilesOn = new bool[_mapWidth, _mapHeight];
        _basePositions = new Vector3[_mapWidth, _mapHeight];
        _targetColor = _map[0, 0].renderer.color;
        for (int i = 0; i < _mapWidth; i++)
        {
            for (int j = 0; j < _mapHeight; j++)
            {
                _basePositions[i, j] = _map[i, j].transform.position;
                _map[i, j].renderer.color = new Color(0, 0, 0, 0);
            }
        }
    }

    internal override void Update()
    {
        _timeElapsed += Time.deltaTime;

        for (var i = 0; i < _mapWidth; i++)
        {
            for (var j = 0; j < _mapHeight; j++)
            {
                var mapTile = _map[i, j];
                if (i + j > _currentIndex) continue;
                if (!_tilesOn[i, j])
                {
                    _tilesOn[i, j] = true;
                    mapTile.gameObject.SetActive(true);
                }
                var progress = Mathf.Min((_timeElapsed - ((i + j) * _staggerTime)) / _animDuration, 1);

                _map[i, j].renderer.color = Color.Lerp(_map[i, j].renderer.color, _targetColor, EasingEquations.Easing.QuadEaseOut(progress / 100));
                mapTile.transform.position = Vector3.Lerp(_basePositions[i, j] + _offset,
                    _basePositions[i, j], EasingEquations.Easing.QuadEaseOut(progress));
            }
        }

        if (_timeElapsed >= ((_currentIndex + 1) * _staggerTime))
        {
            _currentIndex += 1;
        }
        if (_timeElapsed >= _totalDuration) SetStatus(TaskStatus.Success);
    }

    protected override void OnSuccess()
    {
        base.OnSuccess();
        for (var i = 0; i < _mapWidth; i++)
        {
            for (var j = 0; j < _mapHeight; j++)
            {
                _map[i, j].gameObject.SetActive(true);
                _map[i, j].transform.position = _basePositions[i, j];
            }
        }
    }

}
