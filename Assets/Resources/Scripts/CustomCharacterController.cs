using UnityEngine;
using UnityStandardAssets.Utility;
using UnityStandardAssets.Characters.FirstPerson;
using System.Collections;
using System;

    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(AudioSource))]
public class CustomCharacterController : MonoBehaviour {

        private Camera m_Camera;
        private Vector3 m_Input;
        private bool inputDisabled;
        private Vector3 m_MoveDir = Vector3.zero;
        private CharacterController m_CharacterController;
        private CollisionFlags m_CollisionFlags;
        private Vector3 m_OriginalCameraPosition;
        private float m_StepCycle;
        private float m_StepInterval;
        private float m_NextStep;
        private MouseLook m_MouseLook;

        private int[] translationSpeeds = {CharacterControllerValues.INIT_TRANSLATION_SPEED_X, CharacterControllerValues.INIT_TRANSLATION_SPEED_Y, CharacterControllerValues.INIT_TRANSLATION_SPEED_Z};
        private float[] rotationSpeeds = { CharacterControllerValues.INIT_ROTATION_SPEED_X, CharacterControllerValues.INIT_ROTATION_SPEED_Y, CharacterControllerValues.INIT_ROTATION_SPEED_Z };
        private KeyPressDelay[] translationKeyPressDelay;
        private KeyPressDelay[] rotationKeyPressDelay;

        private bool smooth = true;
        private float smoothTime = 5f;

        private Quaternion m_CharacterTargetRot;
        private Quaternion m_CameraTargetRot;

        private ArrayList keyPressDelays;
        private SphereCollider proximityCollider; //Tests for proximity of objects to suit
        public static float proximityRadius = 5.0f;

        // Use this for initialization
        private void Start()
        {
            inputDisabled = false;
            m_CharacterController = GetComponent<CharacterController>();
            m_Camera = Camera.main;
            this.transform.position = Camera.main.gameObject.transform.position;
            Camera.main.gameObject.transform.parent = this.transform;
            Camera.main.gameObject.transform.localPosition = Vector3.zero;
            m_OriginalCameraPosition = m_Camera.transform.localPosition;
            m_StepCycle = 0f;
            m_StepInterval = 0;
            m_NextStep = m_StepCycle / 2f;
            m_CharacterTargetRot = transform.localRotation;
            m_CameraTargetRot = m_Camera.transform.localRotation;

            m_MouseLook = new MouseLook();
            m_MouseLook.clampVerticalRotation = false;
            m_MouseLook.smooth = true;
            m_MouseLook.Init(transform, Camera.main.transform);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            translationKeyPressDelay = new KeyPressDelay[CharacterControllerValues.TRANSLATION_KEYCODES.Length];
            rotationKeyPressDelay = new KeyPressDelay[CharacterControllerValues.ROTATION_KEYCODES.Length];
            int i;
            for (i = 0; i < translationKeyPressDelay.Length; i++)
                translationKeyPressDelay[i] = new KeyPressDelay();
            for (i = 0; i < rotationKeyPressDelay.Length; i++)
                rotationKeyPressDelay[i] = new KeyPressDelay();
            keyPressDelays = new ArrayList();
            InvokeRepeating("ResetKeyDelays", 0.5f, 0.5f);
            initProximityCollider();
        }

        private void initProximityCollider()
        {
            this.proximityCollider = this.gameObject.AddComponent<SphereCollider>();
            this.proximityCollider.radius = proximityRadius;
            this.proximityCollider.isTrigger = true;
        }
        void OnTriggerEnter(Collider collider)
        {
            GameObject.Find("SoundManager").GetComponent<SoundManager>().playProximityBeepSound(Camera.main.transform.position);
        }


        // Update is called once per frame
        private void Update()
        {
            m_MouseLook.LookRotation(transform, Camera.main.transform);
            GetInput();
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                 Application.Quit();
            }
        }

        private void FixedUpdate()
        {

            //LookRotation(transform, m_Camera.transform);

            // always move along the camera forward as it is the direction that it being aimed at
            Vector3 desiredMove = transform.forward * m_Input.z + transform.right * m_Input.x + transform.up * m_Input.y;

            // get a normal for the surface that is being touched to move along it
            RaycastHit hitInfo;
            Physics.SphereCast(transform.position, m_CharacterController.radius, Vector3.down, out hitInfo,
                               m_CharacterController.height / 2f);
            desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

            m_MoveDir.x = desiredMove.x * Math.Abs(translationSpeeds[0]);
            m_MoveDir.z = desiredMove.z * Math.Abs(translationSpeeds[2]);
            m_MoveDir.y = desiredMove.y * Math.Abs(translationSpeeds[1]);
            //desiredMove.x = desiredMove.x * Math.Abs(translationSpeeds[0]);
            //desiredMove.z = desiredMove.z * Math.Abs(translationSpeeds[2]);
            //desiredMove.y = desiredMove.y * Math.Abs(translationSpeeds[1]);


            m_CollisionFlags = m_CharacterController.Move(m_MoveDir * Time.fixedDeltaTime);

            Vector3 velocity = new Vector3(translationSpeeds[0], translationSpeeds[1], translationSpeeds[2]);
            float speed = velocity.magnitude;
            ProgressStepCycle(speed);
            handleSounds();
            
        }

        private void ProgressStepCycle(float speed)
        {
            if (m_CharacterController.velocity.sqrMagnitude > 0 && (m_Input.x != 0 || m_Input.y != 0 || m_Input.z != 0))
            {
                m_StepCycle += (m_CharacterController.velocity.magnitude + (speed)) *
                             Time.fixedDeltaTime;
            }

            if (!(m_StepCycle > m_NextStep))
                return;

            m_NextStep = m_StepCycle + m_StepInterval;
        }
        
        public void setInputDisabled(bool isDisabled)
        {
            inputDisabled = isDisabled;
        }

        public void addSpeed(int x, int y, int z)
        {
            translationSpeeds[0] += x;
            translationSpeeds[1] += y;
            translationSpeeds[2] += z;
        }

        public void setRotationSpeed(float x, float y)
        {
            rotationSpeeds[0] = x;
            rotationSpeeds[1] = y;
        }

        private void GetInput()
        {
            if (inputDisabled) return;
            int i;
            for (i = 0; i < CharacterControllerValues.ROTATION_KEYCODES.Length; i++)
            {
                if (Input.GetKey(CharacterControllerValues.ROTATION_KEYCODES[i]) && !rotationKeyPressDelay[i].isDelayed)
                {
                    float update = (Mathf.Pow(-1, i) * CharacterControllerValues.ROTATION_STEP[i / 2]);
                    rotationSpeeds[i / 2] += update;

                    if (rotationSpeeds[i / 2] > CharacterControllerValues.MAX_ROTATION_SPEED[i / 2])
                        rotationSpeeds[i / 2] -= update;

                    if (rotationSpeeds[i / 2] < -CharacterControllerValues.MAX_ROTATION_SPEED[i / 2])
                        rotationSpeeds[i / 2] -= update;

                    rotationKeyPressDelay[i].Enqueue();
                    keyPressDelays.Add(rotationKeyPressDelay[i]);

                    Debug.Log("Rotate " + (i / 2) + " with speed " + rotationSpeeds[i / 2]);
                }
            }
            for (i = 0; i < CharacterControllerValues.TRANSLATION_KEYCODES.Length; i++)
            {
                if (Input.GetKey(CharacterControllerValues.TRANSLATION_KEYCODES[i]) && !translationKeyPressDelay[i].isDelayed)
                {
                    int update = ((int)Mathf.Pow(-1, i) * CharacterControllerValues.TRANSLATION_STEP[i / 2]);
                    translationSpeeds[i / 2] += update;

                    if (translationSpeeds[i / 2] > CharacterControllerValues.MAX_TRANSLATION_SPEED[i / 2])
                        translationSpeeds[i / 2] -= update;

                    if (translationSpeeds[i / 2] < -CharacterControllerValues.MAX_TRANSLATION_SPEED[i / 2])
                        translationSpeeds[i / 2] -= update;

                    translationKeyPressDelay[i].Enqueue();
                    keyPressDelays.Add(translationKeyPressDelay[i]);

                    Debug.Log("Translate " + (i / 2) + " with speed " + translationSpeeds[i / 2]);
                }
            }
            if (Input.GetKeyDown(CharacterControllerValues.RESET_ORIENTATION))
            {
                for (i = 0; i < translationSpeeds.Length; i++)
                    translationSpeeds[i] /= 2;
                for (i = 0; i < rotationSpeeds.Length; i++)
                    rotationSpeeds[i] /= 2;

                InvokeRepeating("SmoothStoppingMotion", 1f, 2f);
                if(inputDisabled == false)
                {
                    playLongAirBurst();
                }
            }
            m_Input = new Vector3(translationSpeeds[0], translationSpeeds[1], translationSpeeds[2]);
            m_Input.Normalize();

        }

        private void ResetKeyDelays()
        {
            if (keyPressDelays == null) keyPressDelays = new ArrayList();
            for (int i = 0; i < keyPressDelays.Count; i++ )
            {
                KeyPressDelay item = keyPressDelays[i] as KeyPressDelay;
                if (item.justAdded)
                    item.PrepareForDequeue();
                else
                {
                    item.Dequeue();
                    keyPressDelays.RemoveAt(i);
                    i--;
                }
            }
        }

        private void SmoothStoppingMotion()
        {
            CancelInvoke("SmoothStoppingMotion");
            int i;
            for (i = 0; i < translationSpeeds.Length; i++)
                translationSpeeds[i] = 0;
            for (i = 0; i < rotationSpeeds.Length; i++)
                rotationSpeeds[i] = 0;
        }

        private void LookRotation(Transform character, Transform camera)
        {

            m_CharacterTargetRot *= Quaternion.Euler(0f, -rotationSpeeds[0], 0f);
            m_CameraTargetRot *= Quaternion.Euler(-rotationSpeeds[1], 0f, 0f);

            if (smooth)
            {
                character.localRotation = Quaternion.Slerp(character.localRotation, m_CharacterTargetRot,
                    smoothTime * Time.deltaTime);
                camera.localRotation = Quaternion.Slerp(camera.localRotation, m_CameraTargetRot,
                    smoothTime * Time.deltaTime);
            }
            else
            {
                character.localRotation = m_CharacterTargetRot;
                camera.localRotation = m_CameraTargetRot;
            }
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            Rigidbody body = hit.collider.attachedRigidbody;
            //dont move the rigidbody if the character is on top of it
            if (m_CollisionFlags == CollisionFlags.Below)
            {
                return;
            }

            if (body == null || body.isKinematic)
            {
                return;
            }
            body.AddForceAtPosition(m_CharacterController.velocity * 0.1f, hit.point, ForceMode.Impulse);
        }

        private void handleSounds()
        {
            for (int i = 0; i < CharacterControllerValues.ALL_MOVEMENT_KEYCODES.Length; i++)
            {
                if (Input.GetKeyDown(CharacterControllerValues.ALL_MOVEMENT_KEYCODES[i]) && inputDisabled == false)
                {
                    playShortAirBurst();
                }
            }
        }

        private void playShortAirBurst()
        {
            GameObject.Find("SoundManager").GetComponent<SoundManager>().playShortAirBurst(Camera.main.transform.position);
        }
        private void playLongAirBurst()
        {
            GameObject.Find("SoundManager").GetComponent<SoundManager>().playLongAirBurst(Camera.main.transform.position);
        }
}

