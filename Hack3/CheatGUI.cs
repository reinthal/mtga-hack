using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Unity;
using TMPro;
using Wizards.Mtga;

namespace Hack3
{
    
    public class CheatGUI : MonoBehaviour
    {
        private Rect windowRect = new Rect(0, 0, 400, 400); // Window position and size
        private int tab = 0; // Current tab index
        private Color backgroundColor = Color.grey; // Background color
        private bool showMenu = true; // Whether to show the menu or not
        bool enableAccountInformationHack = false;
        bool enableAWSMatchResultHack = false;
        bool enableJoinEventHack = false;
        private EnvironmentDescription description = Pantry.CurrentEnvironment;
        void Start() // or "public override void OnInitializeMelon()" for melon mod or plugin
        {
            // Center the window on the screen
            windowRect.x = (Screen.width - windowRect.width) / 2;
            windowRect.y = (Screen.height - windowRect.height) / 2;
        }

        void OnUpdate() // for melon mod or plugin use "public override void OnUpdate()"
        {
            if (Input.GetKeyDown(KeyCode.Insert)) // Toggle the menu when the Tab key is pressed and for new key sys "if (Keyboard.current.insertKey.wasPressedThisFrame)"
            {
                showMenu = !showMenu;
            }
        }

        void OnGUI() // add "public override" to this for melon mod 
        {
            if (showMenu) // Only draw the menu when showMenu is true
            {
                // Set the background color
                GUI.backgroundColor = backgroundColor;
                string appData = "Alex's MTGA Hack, Unity Version: " + Application.unityVersion;
                windowRect = GUI.Window(0, windowRect, MenuWindow, appData); // Create the window with title "Menu"
            }
        }

        void MenuWindow(int windowID)
        {
            GUILayout.BeginHorizontal();

            // Create toggle buttons for each tab
            GUILayout.BeginVertical(GUILayout.Width(100));
            if (GUILayout.Toggle(tab == 0, "Main", "Button", GUILayout.ExpandWidth(true)))
            {
                tab = 0;
            }
            if (GUILayout.Toggle(tab == 1, "Esp", "Button", GUILayout.ExpandWidth(true)))
            {
                tab = 1;
            }

            GUILayout.EndVertical();

            // Display content for the selected tab

            GUILayout.BeginVertical();


            // Display content for the selected tab
            switch (tab)
            {
                case 0:
                    // Content for tab 1
                    GUILayout.Label(
                        "How to use this tool?\n" +
                        "HOME: Hide the too.\n" +
                        "END: Output API connection details to logger");

                    GUILayout.BeginVertical(GUI.skin.box);

                    GUILayout.BeginHorizontal();
                    GUILayout.BeginVertical();

                    GUILayout.Label("accountSystemId: ");
                    GUILayout.TextArea(this.description.accountSystemId);

                    GUILayout.Label("accountSystemSecret: ");
                    GUILayout.TextArea(this.description.accountSystemSecret);

                    enableAccountInformationHack = GUILayout.Toggle(enableAccountInformationHack, "AccountInformationHack on?");
                    enableAWSMatchResultHack = GUILayout.Toggle(enableAWSMatchResultHack, "AWS MatchResultHack on?");
                    enableJoinEventHack = GUILayout.Toggle(enableJoinEventHack, "JoinEventHack");
                    
                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                    GUILayout.EndVertical();

                    break;
                case 1:
                    // Content for tab 
                    if (GUILayout.Button(" 🐒 JoinEvent Hack 🐒"))
                    {
                        bool oldJoinEventHackB = enableJoinEventHack;
                        enableJoinEventHack = !enableJoinEventHack;
                        bool stateChanged = oldJoinEventHackB != enableJoinEventHack;

                        if (enableJoinEventHack && stateChanged)
                            FrontDoorConnectionHacker.DoPatching();
                        if (!enableJoinEventHack && stateChanged)
                            FrontDoorConnectionHacker.UndoPatching();
                    }
                    if (GUILayout.Button("🗲 AWS MatchResultHack 🗲"))
                    {
                        bool oldMatchResultB = enableAWSMatchResultHack;
                        enableAWSMatchResultHack = !enableAWSMatchResultHack;
                        bool stateChanged = oldMatchResultB != enableAWSMatchResultHack;
                        if (enableAWSMatchResultHack && stateChanged)
                            AWSMatchResultUtilsPatcher.DoPatching();
                        if (!enableAWSMatchResultHack && stateChanged)
                            AWSMatchResultUtilsPatcher.UndoPatching();

                    }
                    if (GUILayout.Button("☠ Hack AccountInformation ☠"))
                    {
                        bool oldAccInfoHackB = enableAccountInformationHack;
                        enableAccountInformationHack = !enableAccountInformationHack;
                        bool stateChanged = oldAccInfoHackB != enableAccountInformationHack;
                        if (enableAccountInformationHack && stateChanged)
                            AccountInformationPatcher.DoPatching();
                        if (!enableAccountInformationHack && stateChanged)
                            AccountInformationPatcher.UndoPatching();
                    }
                    break;
            }

            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
            GUI.DragWindow(); // Allow the user to drag the window around
        }
    }
}
