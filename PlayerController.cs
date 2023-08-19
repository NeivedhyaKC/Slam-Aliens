using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace PlayerClasses
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] GameObject swordHinge;
        [SerializeField] GameObject swordRender;
        [SerializeField] float sensitivity = 0.0f;
        Vector2 inputDeltaPosition = Vector2.zero;
        Vector2 prevMousePosition = Vector2.zero;

        private Vector3 moveVector;

        private Vector3 swordRenderInitialLocalPos = Vector3.zero;

        public enum PowerUpState { NoPowerUp ,BatSpin, BatLength, Invisible, x2Multiplier, Magnet,Shield, Beyblade/*, PowerUpFrequency*/ };
        public enum PowerUpStateStatus { Enter, Stay, Exit };
        public struct PowerUpStruct
        {
            public bool active;
            public float timer;
            public PowerUpStateStatus status;
        }
        PowerUpStruct batSpinStruct;
        PowerUpStruct batLenghtStruct;
        PowerUpStruct invisibilityStruct;
        PowerUpStruct x2MultiplierStruct;
        PowerUpStruct magnetStruct;

        public PowerUpStruct BatSpinStruct { get { return batSpinStruct; } }
        public float GetSensitivity()
        {
            return sensitivity;
        }
        public Transform GetActiveWeaponTransform()
        {
            if (swordHinge.activeSelf)
            {
                return swordHinge.transform;
            }
            //else if (bigSword.activeSelf)
            //{
            //    return bigSword.transform;
            //}
            else
            {
                return null;
            }
        }
        public Vector3 GetDeltaMoveVector()
        {
            return moveVector * Time.deltaTime;
        }

        void Start()
        {
            swordRenderInitialLocalPos = swordRender.transform.localPosition;
            Camera.main.orthographicSize = 5.9f * Screen.height / Screen.width * 0.5f;
        }


        private void Update()
        {
            if (Input.touchCount > 0)
            {
                inputDeltaPosition += Input.GetTouch(0).deltaPosition;
            }
            Vector2 mouseVector = Input.mousePosition;
            //inputDeltaPosition += (mouseVector - prevMousePosition) * 2.0f;
            prevMousePosition = mouseVector;
        }
        void FixedUpdate()
        {
            float camUpSpeed = GameManager.Instance.GetCamUpSpeed();
            moveVector = new Vector3(inputDeltaPosition.x * sensitivity, inputDeltaPosition.y * sensitivity, 0);
            inputDeltaPosition = Vector2.zero;
            //if (batLenghtStruct.active == true)
            //{
            //    if (batLenghtStruct.status == PowerUpStateStatus.Enter)
            //    {
            //        bigSword.SetActive(true);
            //        smallSword.SetActive(false);
            //        batLenghtStruct.status = PowerUpStateStatus.Stay;
            //    }
            //    batLenghtStruct.timer += Time.fixedDeltaTime;
            //    if (batLenghtStruct.timer > 7.5f)
            //    {
            //        batLenghtStruct.status = PowerUpStateStatus.Exit;
            //    }
            //    if (batLenghtStruct.status == PowerUpStateStatus.Exit)
            //    {
            //        bigSword.SetActive(false);
            //        smallSword.SetActive(true);
            //        batLenghtStruct.timer = 0.0f;
            //    }

            //}
            if (batSpinStruct.active)
            {
                if (batSpinStruct.status == PowerUpStateStatus.Enter)
                {
                    JointMotor2D jointMotor2D = new JointMotor2D();
                    jointMotor2D.maxMotorTorque = 50000;
                    jointMotor2D.motorSpeed = swordHinge.GetComponent<HingeJoint2D>().jointSpeed >= 0 ? 50000 : -50000;
                    swordHinge.GetComponent<HingeJoint2D>().motor = jointMotor2D;
                    batSpinStruct.status = PowerUpStateStatus.Stay;
                    swordRender.transform.localPosition = new Vector3(0.6f, 0.0f, 0.0f);
                }
                batSpinStruct.timer += Time.fixedDeltaTime;
                if (batSpinStruct.timer > 7.5f)
                {
                    batSpinStruct.status = PowerUpStateStatus.Exit;
                }
                if (batSpinStruct.status == PowerUpStateStatus.Exit)
                {
                    batSpinStruct.timer = 0.0f;
                    swordHinge.GetComponent<HingeJoint2D>().useMotor = false;
                    swordRender.transform.localPosition = swordRenderInitialLocalPos;
                    batSpinStruct.active = false;
                }
            }

            Vector3 playerPosition = transform.position + moveVector * Time.deltaTime;
            playerPosition.y += camUpSpeed * Time.deltaTime;
            playerPosition.y = Mathf.Clamp(playerPosition.y, EventManager.Instance.camBottomLeftPos.y, EventManager.Instance.camTopRightPos.y);
            playerPosition.x = Mathf.Clamp(playerPosition.x, EventManager.Instance.camBottomLeftPos.x, EventManager.Instance.camTopRightPos.x);
            GetComponent<Rigidbody2D>().MovePosition(playerPosition);
        }
        public void PlayerDie()
        {
            Debug.Log("Game Over");
            LeanTween.cancelAll();
            EventManager.Instance.onPlayerDeath();
            //Destroy(gameObject);
            gameObject.SetActive(false);
            //SceneManager.LoadScene(0);
        }
        
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Enemy") || collision.CompareTag("Bullet") || collision.CompareTag("Trap") || collision.CompareTag("Spike")
                || collision.CompareTag("Laser") || collision.CompareTag("Enemy Weapon"))
            {
                PlayerDie();
            }
            else if (collision.CompareTag("Coin"))
            {
                GameManager.Instance.AddCoin();
                Destroy(collision.gameObject);
            }
            else if (collision.CompareTag("PowerUp"))
            {
                switch (collision.GetComponent<PowerUpScript>().state)
                {
                    case PowerUpState.BatSpin:
                        batSpinStruct.active = true;
                        batSpinStruct.timer = 0.0f;
                        batSpinStruct.status = PowerUpStateStatus.Enter;
                        break;
                    case PowerUpState.BatLength:
                        batLenghtStruct.active = true;
                        batLenghtStruct.status = PowerUpStateStatus.Enter;
                        batLenghtStruct.timer = 0.0f;
                        break;
                    case PowerUpState.Invisible:
                        invisibilityStruct.active = true;
                        invisibilityStruct.timer = 0.0f;
                        invisibilityStruct.status = PowerUpStateStatus.Enter;
                        break;
                    case PowerUpState.x2Multiplier:
                        x2MultiplierStruct.active = true;
                        x2MultiplierStruct.timer = 0.0f;
                        x2MultiplierStruct.status = PowerUpStateStatus.Enter;
                        break;
                    case PowerUpState.Magnet:
                        magnetStruct.active = true;
                        magnetStruct.status = PowerUpStateStatus.Enter;
                        magnetStruct.timer = 0.0f;
                        break;
                }
            }
        }
    }
}

