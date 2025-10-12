using UnityEngine;

public class CloudAnimator : MonoBehaviour
{
    [SerializeField] private CloudAnimatorDefinition _cloudAnimatorDefinition;

    private float _moveUnitPerMinute;
    private float _turnThresholdInMinutes;
    private float _elapsedSecondsFromLastTurn;
    private bool _moveLeft;

    void Start()
    {
        _moveUnitPerMinute = Random.Range(_cloudAnimatorDefinition.MinMoveUnitPerMinute, _cloudAnimatorDefinition.MaxMoveUnitPerMinute);
        _turnThresholdInMinutes = Random.Range(_cloudAnimatorDefinition.MinMinutesToTurn, _cloudAnimatorDefinition.MaxMinutesToTurn);
        _moveLeft = Random.value > 0.5;
    }

    void Update()
    {
        float deltaX = _moveUnitPerMinute / 60f * Time.deltaTime;
        transform.position = transform.position + new Vector3(_moveLeft ? deltaX : -deltaX, 0, 0);
        _elapsedSecondsFromLastTurn += Time.deltaTime;
        if (_elapsedSecondsFromLastTurn / 60f >= _turnThresholdInMinutes)
        {
            _elapsedSecondsFromLastTurn = 0;
            _moveLeft = !_moveLeft;
        }

    }
}
