using System.Collections;
using System.Collections.Generic;
using Bolt;
using UnityEngine;
using vnc;

namespace Epiplon.Samples.Network
{
    public class NetworkedPlayer : Bolt.EntityBehaviour<IRetroControllerState>
    {
        [Header("Cameras")]
        public Camera _mainCamera;
        public Camera _gunCamera;

        [Header("Settings")]
        public RetroController retroController; // the controller used
        public NetworkedMouseLook mouseLook;             // mouse look
        public Transform playerView;            // the controller view

        [Space, Tooltip("Switch to ducking and standing by pressing once instead of holding")]
        public bool toggleDucking;

        [Header("Animation")]
        public Animator playerAnimator;
        public float animDelta = 6f;
        float animHorizontal, animVertical;

        bool _forward, _backward, _left, _right, _jump;
        float _yaw, _pitch;

        private void Start()
        {
            if (mouseLook)
            {
                mouseLook.Init(retroController, playerView);
                mouseLook.SetCursorLock(true);
            }
            else
            {
                Debug.LogWarning("No MouseLook assigned.");
            }

            //if (playerAnimator)
            //{
            //    retroController.OnJumpCallback.AddListener(() =>
            //    {
            //        playerAnimator.SetTrigger("Jump");
            //    });
            //}
        }

        // runs on client
        public void EnableClientCameras()
        {
            _mainCamera.gameObject.SetActive(true);
            _gunCamera.gameObject.SetActive(true);
            SetLayer(gameObject, 8);
        }

        public override void Attached()
        {
            state.SetTransforms(state.Transform, transform);
        }

        void PollKeys(bool mouse)
        {
            _forward = Input.GetKey(KeyCode.W);
            _backward = Input.GetKey(KeyCode.S);
            _left = Input.GetKey(KeyCode.A);
            _right = Input.GetKey(KeyCode.D);
            _jump = Input.GetKeyDown(KeyCode.Space);

            if (mouse)
            {
                _yaw += (Input.GetAxisRaw("Mouse X") * mouseLook.mouseSensitivity);
                _yaw %= 360f;

                _pitch += Input.GetAxisRaw("Mouse Y") * mouseLook.mouseSensitivity;
                _pitch = Mathf.Clamp(_pitch, -85f, +85f);
            }
        }

        private void Update()
        {
            PollKeys(true);
        }

        public override void SimulateController()
        {
            PollKeys(false);

            IPlayerCommandInput input = PlayerCommand.Create();

            input.forward = _forward;
            input.backward = _backward;
            input.left = _left;
            input.right = _right;
            input.jump = _jump;
            input.yaw = _yaw;
            input.pitch = _pitch;

            entity.QueueInput(input);
        }

        public override void ExecuteCommand(Command command, bool resetState)
        {
            PlayerCommand cmd = (PlayerCommand)command;

            if (resetState)
            {
                // we got a correction from the server, reset (this only runs on the client)
                retroController.FixedPosition = cmd.Result.position;
                retroController.Velocity = cmd.Result.velocity;
            }
            else
            {
                // apply movement (this runs on both server and client)
                float fwd = (cmd.Input.forward ? 1 : 0) - (cmd.Input.backward ? 1 : 0);
                float strafe = (cmd.Input.right ? 1 : 0) - (cmd.Input.left ? 1 : 0);
                bool jump = cmd.Input.jump;

                retroController.SetInput(fwd, strafe, 0, jump, false, false);
                mouseLook.LookRotation(cmd.Input.yaw, cmd.Input.pitch);
                retroController.UpdateController();

                // copy the controller state to the commands result (this gets sent back to the client)
                cmd.Result.position = retroController.FixedPosition;
                cmd.Result.velocity = retroController.Velocity;
                cmd.Result.isGrounded = retroController.IsGrounded;
//                cmd.Result.jumpFrames = motorState.jumpFrames;
            }
        }

        void SetLayer(GameObject GO, int layer)
        {
            GO.layer = layer;
            var children = GO.GetComponentsInChildren<Transform>();
            for (int i = 0; i < children.Length; i++)
            {
                if (children[i].gameObject == GO)
                    continue;

                SetLayer(children[i].gameObject, layer);
            }
        }
    }
}
