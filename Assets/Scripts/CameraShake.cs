using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField]
    private bool _isCameraShakeMove, _isCameraShakeRotate;

    [SerializeField, Range(0.1f, 5f)]
    private float _amplitudeRotation;
    [SerializeField, Range(0.001f, 0.1f)]
    private float _amplitudeMove;
    [SerializeField, Range(0.1f, 3f)]
    private float _frequency;

    private float _moveCounterX, _moveCounterY, _moveDurationX, _moveDurationY, _movePreviousX, _movePreviousY, _moveNextX, _moveNextY;
    private float _rotateCounterX, _rotateCounterY, _rotateDurationX, _rotateDurationY, _rotatePreviousX, _rotatePreviousY, _rotateNextX, _rotateNextY;

    private Vector3 _camShakePosition, _camShakeRotation;

    //private float 
    // Start is called before the first frame update
    void Start()
    {
        if(_isCameraShakeMove)
        {
            _moveDurationX = GetNewRND(_frequency, true);
            _moveDurationY = GetNewRND(_frequency, true);
            _moveNextX = GetNewRND(_amplitudeMove, false);
            _moveNextY = GetNewRND(_amplitudeMove, false);
        }

        if(_isCameraShakeRotate)
        {
            _rotateDurationX = GetNewRND(_frequency, true);
            _rotateDurationY = GetNewRND(_frequency, true);
            _rotateNextX = GetNewRND(_amplitudeRotation, false);
            _rotateNextY = GetNewRND(_amplitudeRotation, false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(_isCameraShakeMove)
        {
            _moveCounterX += Time.deltaTime / _moveDurationX;
            _moveCounterY += Time.deltaTime / _moveDurationY;

            if (_moveCounterX >= 1f)
            {
                _moveCounterX -= 1f;
                _moveDurationX = GetNewRND(_frequency, true);
                _movePreviousX = _moveNextX;
                _moveNextX = GetNewRND(_amplitudeMove, false);
            }

            if (_moveCounterY >= 1f)
            {
                _moveCounterY -= 1f;
                _moveDurationY = GetNewRND(_frequency, true);
                _movePreviousY = _moveNextY;
                _moveNextY = GetNewRND(_amplitudeMove, false);
            }

            _camShakePosition = new Vector3(Mathf.SmoothStep(_movePreviousX, _moveNextX, _moveCounterX), Mathf.SmoothStep(_movePreviousY, _moveNextY, _moveCounterY), 0f);
        }

        if(_isCameraShakeRotate)
        {
            _rotateCounterX += Time.deltaTime / _rotateDurationX;
            _rotateCounterY += Time.deltaTime / _rotateDurationY;

            if (_rotateCounterX >= 1f)
            {
                _rotateCounterX -= 1f;
                _rotateDurationX = GetNewRND(_frequency, true);
                _rotatePreviousX = _rotateNextX;
                _rotateNextX = GetNewRND(_amplitudeRotation, false);
            }

            if (_rotateCounterY >= 1f)
            {
                _rotateCounterY -= 1f;
                _rotateDurationY = GetNewRND(_frequency, true);
                _rotatePreviousY = _rotateNextY;
                _rotateNextY = GetNewRND(_amplitudeRotation, false);
            }

            _camShakeRotation = new Vector3(Mathf.SmoothStep(_rotatePreviousX, _rotateNextX, _rotateCounterX), Mathf.SmoothStep(_rotatePreviousY, _rotateNextY, _rotateCounterY), 0f);
        }
    }

    private float GetNewRND(float range, bool isAbsolute)
    {
        return isAbsolute ? Random.Range(range * 0.5f, range) : Random.Range(-range, range);
    }

    private void LateUpdate()
    {
        if(_isCameraShakeRotate)
            transform.localEulerAngles = _camShakeRotation;
        if(_isCameraShakeMove)
            transform.localPosition = _camShakePosition;
    }
}
