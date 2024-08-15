using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace com.ethnicthv.Game.Cube
{
    public enum CubeDirection
    {
        Up,
        Down,
        Left,
        Right,
        Forward,
        Backward
    }

    public enum CubeState
    {
        Moving,
        Static,
        Bouncing
    }

    public class CubeController : MonoBehaviour
    {
        public string enableTag = "Cube";
        public string disableTag = "DisableCube";
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private CubeState state = CubeState.Static;

        public CubeState cubeState
        {
            get => state;
            private set => state = value;
        }

        private (int, int, int) _key;
        private CubeDirection _direction;

        public Color cubeColor
        {
            get => meshRenderer.sharedMaterial.color;
            set => meshRenderer.sharedMaterial.color = value;
        }

        #region Setup

        public void OnDisable()
        {
            Reset();
        }

        public void Reset()
        {
            cubeState = CubeState.Static;
            transform.DOKill();
            meshRenderer.material.DOKill();
            meshRenderer.material.color = Color.white;
        }

        public void Setup((int, int, int) key, Color[] nearColor, CubeDirection direction)
        {
            _key = key;
            gameObject.name = $"Cube_{key.Item1}_{key.Item2}_{key.Item3}_{direction}";
            SetupColor(nearColor);
            SetupDirection(direction);
            _direction = direction;
        }

        private void SetupColor(Color[] nearColor)
        {
            foreach (var c in Colors)
            {
                if (nearColor.Contains(c)) continue;
                meshRenderer.material.color = c;
                break;
            }
        }

        private void SetupDirection(CubeDirection direction)
        {
            transform.rotation = Directions[direction];
        }

        #endregion

        #region Actions

        public void Appear()
        {
            transform.rotation = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
            transform.localScale = Vector3.zero;
            gameObject.SetActive(true);

            transform.DORotate(Directions[_direction].eulerAngles, 1);
            transform.DOScale(1, 1).SetEase(Ease.OutBounce).OnComplete(() => { gameObject.tag = enableTag; });
        }

        public void Disappear(Action onComplete = null)
        {
            //set random rotation
            gameObject.tag = disableTag;
            transform.DOScale(0f, 1).SetEase(Ease.InBack).OnComplete(() =>
            {
                gameObject.SetActive(false);
                transform.localScale = Vector3.one;
                onComplete?.Invoke();
            });
        }

        public void FadeOut(float duration = 1, Action onComplete = null)
        {
            DOTween.ToAlpha(
                () => meshRenderer.material.GetColor(Color1),
                x => meshRenderer.material.SetColor(Color1, x),
                0, duration
            ).OnComplete(() =>
            {
                gameObject.SetActive(false);
                meshRenderer.material.SetColor(Color1, Color.white);
                onComplete?.Invoke();
            });
        }

        public void Move()
        {
            if (cubeState is CubeState.Bouncing or CubeState.Moving) return;
            cubeState = CubeState.Moving;

            var cubes = CubeUtil.GetCubeOn(_key, _direction);
            var direction = CubeUtil.DirectionMapping[(int)_direction];

            var bounceObject = this;
            var goTo = transform.localPosition + direction * CubeManager.instance.cubeMoveDistance;
            var obstacle = false;
            foreach (var cube in cubes)
            {
                if (cube.state is not (CubeState.Bouncing or CubeState.Static)) continue;
                goTo = cube.transform.position - direction;
                Debug.Log(cube.transform.position);
                obstacle = true;
                bounceObject = cube;
                break;
            }

            if (obstacle)
            {
                if (goTo == transform.localPosition)
                {
                    Bounce(direction);
                    return;
                }

                var temp = transform.position - goTo;
                var moveValue = (float)CubeUtil.GetMovingValue((int)temp.x, (int)temp.y, (int)temp.x, _direction);
                transform.DOLocalMove(goTo, moveValue / CubeManager.instance.cubeMoveSpeed).SetEase(Ease.Linear)
                    .OnComplete(() =>
                    {
                        bounceObject.Bounce(direction);
                        transform.DOLocalMove(
                                new Vector3(_key.Item1, _key.Item2, _key.Item3),
                                0.5f)
                            .SetEase(Ease.Linear)
                            .OnComplete(() => { cubeState = CubeState.Static; });
                    });
                return;
            }

            meshRenderer.material.DOFade(0, CubeManager.instance.cubeMoveDuration / 2)
                .SetDelay(CubeManager.instance.cubeMoveDuration / 2)
                .OnComplete(() => { meshRenderer.material.DOFade(1, 0); });
            transform.DOLocalMove(goTo, CubeManager.instance.cubeMoveDuration)
                .SetEase(Ease.Linear)
                .OnComplete(() => { CubeManager.instance.DestroyCube(_key.Item1, _key.Item2, _key.Item3); });
        }


        private void Bounce(Vector3 direction, bool recursive = true)
        {
            if (cubeState == CubeState.Bouncing) return;
            cubeState = CubeState.Bouncing;
            transform.DOKill();
            transform.DOLocalMove(transform.localPosition + direction * 0.2f, 0.2f)
                .SetEase(Ease.OutCirc).SetLoops(2, LoopType.Yoyo).OnComplete(() => { cubeState = CubeState.Static; });
            if (recursive)
            {
                DOVirtual.DelayedCall(0.03f, () =>
                {
                    var nextBounce = (_key.Item1 + (int)direction.x, _key.Item2 + (int)direction.y,
                        _key.Item3 + (int)direction.z);
                    var nextCube = CubeManager.instance.GetCube(nextBounce);
                    if (nextCube == null) return;
                    if (nextCube.cubeState != CubeState.Static) return;
                    nextCube.Bounce(direction);
                });
            }
        }

        #endregion

        #region Constants

        public static readonly Color[] Colors =
        {
            Color.red,
            Color.green,
            Color.blue,
            Color.yellow,
            Color.magenta,
            Color.cyan
        };

        public static readonly Dictionary<CubeDirection, Quaternion> Directions =
            new()
            {
                { CubeDirection.Up, Quaternion.Euler(-90, 0, 0) },
                { CubeDirection.Down, Quaternion.Euler(90, 0, 0) },
                { CubeDirection.Left, Quaternion.Euler(0, -90, 0) },
                { CubeDirection.Right, Quaternion.Euler(0, 90, 0) },
                { CubeDirection.Forward, Quaternion.Euler(0, 0, 0) },
                { CubeDirection.Backward, Quaternion.Euler(180, 0, 0) }
            };

        private static readonly int Color1 = Shader.PropertyToID("_Color");

        #endregion
    }
}