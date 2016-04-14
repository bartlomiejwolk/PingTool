using System;
using System.Text;
using UnityEngine;

namespace PingTool {

    public sealed class PingTool : MonoBehaviour {
        
        #region CONSTANTS

        // area dimentions for the GUI elements
        private const int areaWidth = 250;
        private const int areaHeight = 300;

        #endregion CONSTANTS

        #region INSPECTOR

        [Tooltip("Interval between displaying output in sec.")]
        [SerializeField]
        private float outputInterval = 1f;

        [Tooltip("Time in ms")]
        [SerializeField]
        private int pingTimeout = 3000;

        [Tooltip("Max number of ping results being displayed at once")]
        [SerializeField]
        private int maxDisplayedPings = 10;

        [Space(10)]
        [SerializeField]
        private int horizontalPosition = 10;

        [SerializeField]
        private int verticalPosition = 10;

        #endregion

        #region FIELDS

        private string remoteIp = "127.0.0.1";
        private PingResult pingResult;
        private CustomPing currentPing;

        // if active then continuous sending/receiving pings is in progress
        private bool active;

        #endregion FIELDS

        #region CALLBACKS

        private Action StateChanged { get; set; }
        private Action PingReceived { get; set; }
        private Action PingTimeout { get; set; }

        #endregion CALLBACKS

        #region PROPERTIES

        private bool Active {
            get { return active; }

            set {
                var oldValue = active;
                active = value;

                if (oldValue != value) {
                    StateChanged();
                }
            }
        }

        #endregion PROPERTIES

        #region UNITY MESSAGES

        private void Awake() {
            pingResult = new PingResult(maxDisplayedPings);

            StateChanged += StateChangedHandler;
            PingReceived += PingReceivedHandler;
            PingTimeout += PingTimeoutHandler;
        }

        private void Start() {
            InvokeRepeating("Execute", 0, outputInterval);
        }

        private void OnGUI() {
            var areaRect = new Rect(horizontalPosition, verticalPosition, areaWidth, areaHeight);
            GUILayout.BeginArea(areaRect);

            GUILayout.BeginHorizontal();

            DrawRemoteIpTextField();
            DrawPingButton(PingButtonPressedHandler);

            GUILayout.EndHorizontal();

            DrawOutput();

            GUILayout.EndArea();
        }

        #endregion

        #region DRAWING

        private void DrawOutput() {
            GUILayout.Label(pingResult.Output, GUILayout.Width(areaWidth));
        }

        private void DrawPingButton(Action buttonPressedCallback) {
            string btnText;
            if (Active) {
                btnText = "Stop";
            }
            else {
                btnText = "Ping";
            }

            var pingBtn = GUILayout.Button(btnText, GUILayout.Width(80));

            if (pingBtn) {
                buttonPressedCallback();
            }
        }

        private void DrawRemoteIpTextField() {
            remoteIp = GUILayout.TextField(remoteIp, 15, GUILayout.Width(120));
        }

        #endregion

        #region PRIVATE METHODS

        private void Execute() {
            if (!Active) {
                return;
            }

            CheckForPingReturned();
            CheckForTimeout();
            HandleSendPing();
        }

        private void HandleSendPing() {
            if (currentPing == null) {
                currentPing = new CustomPing(remoteIp);
            }
        }

        private void HandleClearOutputScreen() {
            if (active) {
                pingResult.Clear();
            }
        }

        private void ToggleState() {
            Active = !Active;
        }

        private void CheckForPingReturned() {
            if (currentPing == null) {
                return;
            }

            if (currentPing.IsDone) {
                PingReceived();
            }
        }

        private void CheckForTimeout() {
            if (currentPing == null) {
                return;
            }

            if (currentPing.TimeSinceCreation >= pingTimeout) {
                PingTimeout();
            }
        }

        private void LogSuccess() {
            if (currentPing == null) {
                return;
            }

            var output = new StringBuilder();
            output.Append("Reply from ");
            output.Append(remoteIp);
            output.Append(": time < ");
            output.Append(currentPing.RoundTripTime);
            output.AppendLine();

            pingResult.AddLine(output);
        }

        private void LogTimeout() {
            var output = new StringBuilder();
            output.Append("Timeout");
            output.AppendLine();

            pingResult.AddLine(output);
        }

        #endregion

        #region CALLBACK HANDLERS

        private void PingButtonPressedHandler() {
            ToggleState();
        }

        private void StateChangedHandler() {
            HandleClearOutputScreen();
        }

        private void PingTimeoutHandler() {
            LogTimeout();
            currentPing = null;
        }

        private void PingReceivedHandler() {
            LogSuccess();
            currentPing = null;
        }

        #endregion CALLBACK HANDLERS
    }

}