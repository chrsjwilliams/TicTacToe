using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TitleEntryAnimation : Task
{
    private float _timeElapsed;
    private const float _animDuration = 0.5f;
    private const float _staggerTime = 0.3f;
    private float _totalDuration;

    private float initalOffset = -3000;

    private int _currentIndex;
    private TextMeshProUGUI[] _titleText;
    private Vector3[] _startPos;
    private Vector3[] _targetPos;

    private bool _transitionOut;

    public TitleEntryAnimation(TextMeshProUGUI[] tt, bool transitionOut = false)
    {
        _titleText = tt;
        _transitionOut = transitionOut;
        _startPos = new Vector3[_titleText.Length];
        _targetPos = new Vector3[_titleText.Length];

    }


    protected override void Init()
    {
        _timeElapsed = 0;
        _currentIndex = 0;

        _totalDuration = _animDuration + (_staggerTime * _titleText.Length);
    
        for (int i = 0; i < _titleText.Length; i++)
        {
            if (_transitionOut)
            {
                _startPos[i] = _titleText[i].transform.localPosition;
                _targetPos[i] = new Vector3( _titleText[i].transform.localPosition.x,
                                            _titleText[i].transform.localPosition.y + initalOffset,
                                            0);
                _titleText[i].transform.localPosition = new Vector3(_startPos[i].x, _startPos[i].y, _startPos[i].z);
            }
            else
            {
                _targetPos[i] = _titleText[i].transform.localPosition;
                _startPos[i] = new Vector3( _titleText[i].transform.localPosition.x,
                                            _titleText[i].transform.localPosition.y + initalOffset,
                                            0);
                _titleText[i].transform.localPosition = new Vector3(_startPos[i].x, _startPos[i].y, _startPos[i].z);   
            }
        }
    }

    internal override void Update()
    {
        _timeElapsed += Time.deltaTime;

        for (var i = 0; i < _titleText.Length; i++)
        {
            var progress = Mathf.Min((_timeElapsed - (i * _staggerTime)) / _animDuration, 1);
            _titleText[i].transform.localPosition = Vector3.Lerp(_startPos[i], _targetPos[i], EasingEquations.Easing.QuadEaseOut(progress));

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
        if(!_transitionOut)
        {
            Services.EventManager.Fire(new GameLoadEvent());
        }
        for (var i = 0; i < _titleText.Length; i++)
        {
            _titleText[i].transform.localPosition = _targetPos[i];
        }
    }
}
