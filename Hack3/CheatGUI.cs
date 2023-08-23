using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Unity;
namespace Hack3
{

    public class CheatGUI : MonoBehaviour
    {
        private Rect windowRect = new Rect(0, 0, 400, 400); // Window position and size
        private int tab = 0; // Current tab index
        private Color backgroundColor = Color.grey; // Background color
        private bool showMenu = true; // Whether to show the menu or not
        bool temp = false;
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

                windowRect = GUI.Window(0, windowRect, MenuWindow, "Unity version: " + Application.unityVersion); // Create the window with title "Menu"
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
                    GUILayout.Label("Tab 1 Content");
                    GUILayout.BeginVertical(GUI.skin.box);

                    GUILayout.BeginHorizontal();
                    GUILayout.BeginVertical();
                    temp = GUILayout.Toggle(temp, "temp");
                    temp = GUILayout.Toggle(temp, "temp");
                    temp = GUILayout.Toggle(temp, "temp");
                    temp = GUILayout.Toggle(temp, "temp");
                    temp = GUILayout.Toggle(temp, "temp");
                    temp = GUILayout.Toggle(temp, "temp");

                    GUILayout.EndVertical();

                    GUILayout.Space(10);

                    GUILayout.BeginVertical();
                    temp = GUILayout.Toggle(temp, "temp");
                    temp = GUILayout.Toggle(temp, "temp");
                    temp = GUILayout.Toggle(temp, "temp");
                    temp = GUILayout.Toggle(temp, "temp");
                    temp = GUILayout.Toggle(temp, "temp");
                    temp = GUILayout.Toggle(temp, "temp");
                    GUILayout.EndVertical();

                    GUILayout.EndHorizontal();

                    GUILayout.EndVertical();

                    break;
                case 1:
                    // Content for tab 2
                    GUILayout.Label("Tab 2 Content");
                    GUILayout.BeginVertical(GUI.skin.box);

                    GUILayout.BeginHorizontal();
                    GUILayout.BeginVertical();
                    temp = GUILayout.Toggle(temp, "temp");
                    temp = GUILayout.Toggle(temp, "temp");
                    temp = GUILayout.Toggle(temp, "temp");
                    temp = GUILayout.Toggle(temp, "temp");
                    temp = GUILayout.Toggle(temp, "temp");
                    temp = GUILayout.Toggle(temp, "temp");

                    GUILayout.EndVertical();

                    GUILayout.Space(10);

                    GUILayout.BeginVertical();
                    temp = GUILayout.Toggle(temp, "temp");
                    temp = GUILayout.Toggle(temp, "temp");
                    temp = GUILayout.Toggle(temp, "temp");
                    temp = GUILayout.Toggle(temp, "temp");
                    temp = GUILayout.Toggle(temp, "temp");
                    temp = GUILayout.Toggle(temp, "temp");
                    GUILayout.EndVertical();

                    GUILayout.EndHorizontal();

                    GUILayout.EndVertical();

                    if (GUILayout.Button("temp Button"))
                    {
                        temp = !temp;


                    }



                    break;
            }

            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
            GUI.DragWindow(); // Allow the user to drag the window around
        }
    }
}
