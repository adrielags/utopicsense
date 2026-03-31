using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace utopicsense
{
    public class UWindow : EditorWindow
    {
        private GameObject utopicSense;
        UtopicSenseVisual utopicSenseVisualScript;
        private String previousScene;

        private const int COLOR_NUM = 14;
        private const int NUM_TEX = 5;

        private const int USENSE_WIDTH = 460;
        private const int USENSE_HEIGHT = 610;

        private const int RESET_DEFAULT_X = 10;
        private const int RESET_DEFAULT_Y = 770;

        private Color[] colors = new Color[COLOR_NUM];
        private bool simplified = false;

        private int defaultColorDetailsX = 810;

        private const string UTOPIC_ASSET_PATH =
         "Assets/UtopicSense/Resources/PrefabUS/UtopicSense.asset";




        private UtopicSenseVisual checkBefore;
        int selection;
        int selectType;
        int selectIntensity;
        int selectionShader;
        string[] options = new string[] { "Standard", "Simple" };
        string[] optionsType = new string[] { "Effect 1", "Effect 2", "Effect 3", "Effect 4", "Effect 5" };
        string[] optionsIntensity = new string[] { "Low", "Medium", "High", "Extreme" };
        private List<GameObject> rootGameObjects = new List<GameObject>();
        private System.Object[] toDelete;

        //textures

        Texture[] textura = new Texture[NUM_TEX];
        private GameObject[] particleTypes = new GameObject[NUM_TEX];
        public Renderer[] rend = new Renderer[NUM_TEX];

        //shaders
        Shader shaderD;
        Shader shaderM;
        Shader shaderA;
        List<string> shaderOptions = new List<string>();

        private Vector2 scrollPos;

        [MenuItem("Window/UtopicSense/Open UtopicSense", priority = 0)]
        public static void ShowWindow()
        {
            GetWindow<UWindow>("UtopicSense");
        }

        public void Awake()
        {

            for (int j = 0; j < NUM_TEX; j++)
            {
                //atributting type to objects in resources
                particleTypes[j] = Resources.Load<GameObject>(String.Concat("ParticlesUS/CircleParticle", j + 1));
                rend[j] = particleTypes[j].GetComponent<Renderer>();
                textura[j] = rend[j].sharedMaterial.mainTexture;
            }
            shaderD = Shader.Find("Sprites/Default");
            if (shaderD != null)
                shaderOptions.Add("Default");

            shaderM = Shader.Find("Legacy Shaders/Particles/Multiply (Double)");
            if (shaderM != null)
                shaderOptions.Add("Multiply");

            shaderA = Shader.Find("Legacy Shaders/Particles/Additive");
            if (shaderA != null)
                shaderOptions.Add("Additive");

            utopicSenseVisualScript =
                AssetDatabase.LoadAssetAtPath<UtopicSenseVisual>(
                    "Assets/UtopicSense/Resources/PrefabUS/UtopicSense.asset"
                );

            if (utopicSenseVisualScript == null)
            {
                createUtopicSense();
                putToDefaultColors();
                SaveStateToAsset();
            }
            else
            {
                LoadStateFromAsset();
            }

        }

        //Window code
        private void OnGUI()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos,
                                                          false,
                                                          false);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Welcome to UtopicSense", EditorStyles.boldLabel);
            EditorGUILayout.EndHorizontal();

            GUILayout.BeginArea(new Rect(10, 20, 270, 300));
            GUILayout.Label("UtopicSense is a tool to map sounds to \n"
                           + "visual effects (particles)\n"
                           + "> Use it in objects with AudioSources\n"
                           + "> It can be used in prefabs too!", EditorStyles.miniBoldLabel);
            GUILayout.EndArea();

            EditorGUI.BeginChangeCheck();
            selection = GUI.SelectionGrid(new Rect(265, 30, 200, 20), selection, options, 2);
            if (EditorGUI.EndChangeCheck())
            {

                if (selection == 0)
                {
                    simplified = false;
                }
                else if (selection == 1)
                {
                    simplified = true;
                }
                else
                {
                    Debug.Log("Error");
                }
                SaveStateToAsset();
            }

            // AtivoInativo
            GUILayout.BeginArea(new Rect(290, 65, 200, 30));
            EditorGUI.BeginChangeCheck();
            utopicSenseVisualScript.utopicActive = EditorGUILayout.Toggle("UtopicSense Active", utopicSenseVisualScript.utopicActive);
            if (EditorGUI.EndChangeCheck())
            {
                SaveStateToAsset();
            }
            GUILayout.EndArea();

            //selecao de shaders

            GUILayout.BeginArea(new Rect(10, 200, 270, 300));
            GUILayout.Label("Select a shader:\n", EditorStyles.miniBoldLabel);
            GUILayout.EndArea();

            string[] sOptions = shaderOptions.ToArray();
            EditorGUI.BeginChangeCheck();
            selectionShader = GUI.SelectionGrid(new Rect(110, 200, 300, 17), selectionShader, sOptions, 3);
            if (EditorGUI.EndChangeCheck())
            {
                if (selectionShader == 0)
                {

                    for (int i = 0; i < NUM_TEX; i++)
                    {
                        rend[i].sharedMaterial.shader = shaderD;
                    }

                }
                else if (selectionShader == 1)
                {
                    for (int i = 0; i < NUM_TEX; i++)
                    {
                        rend[i].sharedMaterial.shader = shaderM;
                    }

                }

                else if (selectionShader == 2)
                {
                    for (int i = 0; i < NUM_TEX; i++)
                    {
                        rend[i].sharedMaterial.shader = shaderA;
                    }
                }

                else
                {
                    for (int i = 0; i < NUM_TEX; i++)
                    {
                        rend[i].material.shader = shaderD;
                    }
                }

                SaveStateToAsset(); 
            }

            // Highlight different areas

            GUILayout.BeginArea(new Rect(245, 230, 245, 30));
            GUILayout.Label("APPLY effect to objects", EditorStyles.boldLabel);
            GUILayout.EndArea();

            // BOX 1
            //parte interna
            EditorGUI.DrawRect(new Rect(245, 250, 245, 140), new Color(0.55f, 0.68f, 0.90f, 1.0f));
            //bordas
            EditorGUI.DrawRect(new Rect(245, 250, 1, 140), Color.black);
            EditorGUI.DrawRect(new Rect(490, 250, 1, 140), Color.black);
            EditorGUI.DrawRect(new Rect(245, 250, 245, 1), Color.black);
            EditorGUI.DrawRect(new Rect(245, 390, 245, 1), Color.black);

            GUILayout.BeginArea(new Rect(245, 410, 275, 30));
            GUILayout.Label(("Change effect emission SPEED"), EditorStyles.boldLabel);
            GUILayout.EndArea();

            // BOX 2
            //parte interna
            EditorGUI.DrawRect(new Rect(245, 430, 245, 140), new Color(0.57f, 0.57f, 0.57f, 1.0f));
            //bordas
            EditorGUI.DrawRect(new Rect(245, 430, 1, 140), Color.black);
            EditorGUI.DrawRect(new Rect(490, 430, 1, 140), Color.black);
            EditorGUI.DrawRect(new Rect(245, 430, 245, 1), Color.black);
            EditorGUI.DrawRect(new Rect(245, 570, 245, 1), Color.black);


            GUILayout.BeginArea(new Rect(245, 620, 245, 30));
            GUILayout.Label("DELETE effects", EditorStyles.boldLabel);
            GUILayout.EndArea();

            // BOX 3
            //parte interna
            EditorGUI.DrawRect(new Rect(245, 640, 245, 120), new Color(0.57f, 0.57f, 0.57f, 1.0f));
            //bordas
            EditorGUI.DrawRect(new Rect(245, 640, 1, 120), Color.black);
            EditorGUI.DrawRect(new Rect(490, 640, 1, 120), Color.black);
            EditorGUI.DrawRect(new Rect(245, 640, 245, 1), Color.black);
            EditorGUI.DrawRect(new Rect(245, 760, 245, 1), Color.black);

            //ResetDefaultColors details
            EditorGUI.DrawRect(new Rect(10, defaultColorDetailsX, 14.28f, 15), new Color(0.9866f, 0.0f, 1.0f, 1.0f)); //7,14
            EditorGUI.DrawRect(new Rect(24.28f, defaultColorDetailsX, 14.28f, 15), new Color(1.0f, 0.0f, 0.5785f, 1.0f));
            EditorGUI.DrawRect(new Rect(38.56f, defaultColorDetailsX, 14.28f, 15), new Color(1.0f, 0.0f, 0.2964f, 1.0f));
            EditorGUI.DrawRect(new Rect(52.84f, defaultColorDetailsX, 14.28f, 15), new Color(1.0f, 0.1216f, 0.0f, 1.0f));
            EditorGUI.DrawRect(new Rect(67.12f, defaultColorDetailsX, 14.28f, 15), new Color(1.0f, 0.3279f, 0.0f, 1.0f));
            EditorGUI.DrawRect(new Rect(81.4f, defaultColorDetailsX, 14.28f, 15), new Color(1.0f, 0.5504f, 0.0156f, 1.0f));
            EditorGUI.DrawRect(new Rect(95.68f, defaultColorDetailsX, 14.28f, 15), new Color(1.0f, 0.8853f, 0.0156f, 1.0f));
            EditorGUI.DrawRect(new Rect(109.96f, defaultColorDetailsX, 14.28f, 15), new Color(0.7819f, 1.0f, 0.0f, 1.0f));
            EditorGUI.DrawRect(new Rect(124.24f, defaultColorDetailsX, 14.28f, 15), new Color(0.1153f, 1.0f, 0.0f, 1.0f));
            EditorGUI.DrawRect(new Rect(138.52f, defaultColorDetailsX, 14.28f, 15), new Color(0.0f, 1.0f, 0.6069f, 1.0f));
            EditorGUI.DrawRect(new Rect(152.8f, defaultColorDetailsX, 14.28f, 15), new Color(0.0f, 0.3061f, 0.9725f, 1.0f));
            EditorGUI.DrawRect(new Rect(167.08f, defaultColorDetailsX, 14.28f, 15), new Color(0.0f, 0.0f, 1.0f, 1.0f));
            EditorGUI.DrawRect(new Rect(181.36f, defaultColorDetailsX, 14.28f, 15), new Color(0.3078f, 0.0f, 1.0f, 1.0f));
            EditorGUI.DrawRect(new Rect(195.64f, defaultColorDetailsX, 14.28f, 15), new Color(0.5144f, 0.0f, 1.0f, 1.0f));

            //Highlight save button
            //EditorGUI.DrawRect(new Rect(10, 198, 200, 21), Color.red);



            //All help buttons here
            GUILayout.BeginArea(new Rect(470, 30, 20, 20));
            if (GUILayout.Button("?"))
                EditorUtility.DisplayDialog("Standard and Simple", "The mode determines how the colors are picked for the particles.\n\n " +
                    "Standard: the dominant frequency of the audio are identified and mapped to colors\n\n" +
                    "Simple: Simple: all particles will start with one color and end with another ", "OK");
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(190, 68, 20, 30));
            if (GUILayout.Button("?"))
                EditorUtility.DisplayDialog("Particle textures", "Here you can select any texture in your project to be the an effect to be applied.\n\n "
                    + "The particle effect has the selected texture in it's material, that means that this is how the effect will look like ingame.\n\n"
                    + "You can have 5 different effects to apply in your project", "OK");
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(420, 200, 20, 30));
            if (GUILayout.Button("?"))
            {
                ShaderHelp inst = ScriptableObject.CreateInstance<ShaderHelp>();
                inst.Show();
            }
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(195, 233, 20, 30));
            if (GUILayout.Button("?"))
            {
                ColorHelp inst = ScriptableObject.CreateInstance<ColorHelp>();
                inst.Show();
            }

            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(470, 227, 20, 30));
            if (GUILayout.Button("?"))
                EditorUtility.DisplayDialog("Effect to objects", "Here you can apply any of the effects created to your objects. " +
                    "\n\n The UtopicSense effect is the particle effect material from the textures selected above.\n\n"
                    + "Select the effect you want to apply then select the objects and after that click the button 'Apply Effect'.\n\n If your object contains an AudioSource then the effect will be active on" +
                    "screen when the sound plays. The texture of the effect will be the UtopicSense selected above. To change the effect just select the object again and apply another effect.", "OK");
            GUILayout.EndArea();


            GUILayout.BeginArea(new Rect(470, 408, 20, 30));
            if (GUILayout.Button("?"))
                EditorUtility.DisplayDialog("Effect intensity", "Sometimes you might want to have more particles in an object than in others.\n\n " +
                    "More particles will be generated on 'high','medium' will generate average value, and less particles will be generated on 'low'(standard value).\n\n" +
                    " 'Extreme' is used to generate the maximum number of particles." +
                    "\n\n If an effect is not working as it should it is recommended to apply 'Extreme' to be sure of it's behavior!", "OK");
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(470, 618, 20, 30));
            if (GUILayout.Button("?"))
                EditorUtility.DisplayDialog("Deactivate effects from this scene", "Here you find options to delete the effects from certain objects or all effects from active scene. ", "OK");
            GUILayout.EndArea();

            //Inside selections

            GUILayout.Space(50.0f);

            //Standard selection
            if (selection == 0)
            {

                //simplified = false;
                // Area to select particle textures types

                GUILayout.Label("SELECT the particle texture", EditorStyles.boldLabel);

                EditorGUILayout.BeginHorizontal();
                for (int i = 0; i < NUM_TEX; i++)
                {
                    GUILayout.BeginVertical();
                    var style = new GUIStyle(GUI.skin.label);
                    style.alignment = TextAnchor.UpperCenter;
                    style.fixedWidth = 70;
                    GUILayout.Label(optionsType[i], style);
                    EditorGUI.BeginChangeCheck();
                    textura[i] = (Texture)EditorGUILayout.ObjectField(textura[i], typeof(Texture), false, GUILayout.Width(70), GUILayout.Height(70));
                    if (EditorGUI.EndChangeCheck())
                    {
                        rend[i].sharedMaterial.mainTexture = textura[i];
                    }
                    GUILayout.EndVertical();
                }
                EditorGUILayout.EndHorizontal();


                GUILayout.Space(50.0f);

                GUILayout.BeginArea(new Rect(10, 230, 200, 80));
                GUILayout.Space(5.0f);
                GUILayout.Label("COLOR gradients for effects", EditorStyles.boldLabel);
                GUILayout.EndArea();

                //COLOR AREA

                GUILayout.BeginArea(new Rect(10, 280, 200, 500));
                EditorGUI.BeginChangeCheck();

                GUILayout.Label("Pitch between 20Hz - 100Hz", EditorStyles.miniBoldLabel);
                GUILayout.Space(2.0f);
                colors[0] = EditorGUILayout.ColorField("Beginning color", colors[0]);
                colors[1] = EditorGUILayout.ColorField("Ending color", colors[1]);

                GUILayout.Label("Pitch between 100Hz - 500Hz", EditorStyles.miniBoldLabel);
                GUILayout.Space(2.0f);
                colors[2] = EditorGUILayout.ColorField("Beginning color", colors[2]);
                colors[3] = EditorGUILayout.ColorField("Ending color", colors[3]);
                GUILayout.Space(2.0f);

                GUILayout.Label("Pitch between 500Hz - 1000Hz", EditorStyles.miniBoldLabel);
                GUILayout.Space(2.0f);
                colors[4] = EditorGUILayout.ColorField("Beginning color", colors[4]);
                colors[5] = EditorGUILayout.ColorField("Ending color", colors[5]);
                GUILayout.Space(2.0f);

                GUILayout.Label("Pitch between 1000Hz - 2000Hz", EditorStyles.miniBoldLabel);
                GUILayout.Space(2.0f);
                colors[6] = EditorGUILayout.ColorField("Beginning color", colors[6]);
                colors[7] = EditorGUILayout.ColorField("Ending color", colors[7]);
                GUILayout.Space(2.0f);

                GUILayout.Label("Pitch between 2000Hz - 5000Hz", EditorStyles.miniBoldLabel);
                GUILayout.Space(2.0f);
                colors[8] = EditorGUILayout.ColorField("Beginning color", colors[8]);
                colors[9] = EditorGUILayout.ColorField("Ending color", colors[9]);
                GUILayout.Space(2.0f);

                GUILayout.Label("Pitch between 5000Hz - 9000Hz", EditorStyles.miniBoldLabel);
                GUILayout.Space(2.0f);
                colors[10] = EditorGUILayout.ColorField("Beginning color", colors[10]);
                colors[11] = EditorGUILayout.ColorField("Ending color", colors[11]);
                GUILayout.Space(2.0f);

                GUILayout.Label("Pitch between 9000Hz - 20000Hz", EditorStyles.miniBoldLabel);
                GUILayout.Space(2.0f);
                colors[12] = EditorGUILayout.ColorField("Beginning color", colors[12]);
                colors[13] = EditorGUILayout.ColorField("Ending color", colors[13]);
                GUILayout.Space(2.0f);
                if (EditorGUI.EndChangeCheck())
                {

                    SaveStateToAsset();

                }
                GUILayout.EndArea();


                GUILayout.BeginArea(new Rect(RESET_DEFAULT_X, RESET_DEFAULT_Y, 200, 100));

                GUILayout.Label("Default UtopicSense Colors", EditorStyles.miniBoldLabel);
                GUILayout.Space(10.0f);

                if (GUILayout.Button("Reset default colors"))
                {
                    if (EditorUtility.DisplayDialog("Warning", "Are you sure you want to reset to default colors?", "Yes", "No"))
                    {
                        putToDefaultColors();
                        SaveStateToAsset();
                    }
                }

                GUILayout.EndArea();

                // box aside the colors

                GUILayout.BeginArea(new Rect(250, 250, 300, 500));

                GUILayout.Label("SELECT effect to be applied to objects", EditorStyles.miniBoldLabel);

                GUILayout.BeginArea(new Rect(10, 20, 250, 60));

                selectType = GUI.SelectionGrid(new Rect(0, 0, 150, 50), selectType, optionsType, 2);

                GUILayout.EndArea();



                GUILayout.BeginArea(new Rect(10, 80, 200, 100));

                GUILayout.Label("Select objects in scene to apply", EditorStyles.miniBoldLabel);
                GUILayout.Space(5.0f);

                if (GUILayout.Button("Apply effect"))
                {

                    if (Selection.gameObjects.Length == 0)
                    {
                        EditorUtility.DisplayDialog("Error", "There are no selected objects or prefabs for this operation", "OK");
                    }
                    else
                    {
                        foreach (GameObject obj in Selection.gameObjects)

                        {
                            AudioSource verification = obj.GetComponent<AudioSource>();

                            if (verification == null)
                            {
                                if (!EditorUtility.DisplayDialog("Warning", "There is no AudioSource in: '' " + obj.transform.name + "'', do still want to apply the effect?\n\n" +
                                    "(The effect will not work until there is an AudioSource that plays an AudioClip in the GameObject/Prefab)", "Don't apply", "Apply anyway"))
                                {

                                    SoundEffect reference = obj.GetComponent<SoundEffect>();
                                    if (reference == null)
                                    {
                                        SoundEffect effects = obj.gameObject.AddComponent<SoundEffect>();
                                        effects.particleType = selectType + 1;
                                        AssetDatabase.SaveAssets();
                                        AssetDatabase.Refresh();
                                        EditorSceneManager.MarkSceneDirty(obj.scene);
                                    }
                                    else
                                    {
                                        DestroyImmediate(reference);
                                        SoundEffect effects = obj.gameObject.AddComponent<SoundEffect>();
                                        AssetDatabase.SaveAssets();
                                        AssetDatabase.Refresh();
                                        EditorSceneManager.MarkSceneDirty(obj.scene);

                                        effects.particleType = selectType + 1;
                                    }
                                }
                            }
                            else
                            {

                                SoundEffect reference = obj.GetComponent<SoundEffect>();
                                if (reference == null)
                                {
                                    SoundEffect effects = obj.gameObject.AddComponent<SoundEffect>();
                                    effects.particleType = selectType + 1;
                                    AssetDatabase.SaveAssets();
                                    AssetDatabase.Refresh();
                                    EditorSceneManager.MarkSceneDirty(obj.scene);
                                }
                                else
                                {
                                    DestroyImmediate(reference);
                                    SoundEffect effects = obj.gameObject.AddComponent<SoundEffect>();
                                    AssetDatabase.SaveAssets();
                                    AssetDatabase.Refresh();
                                    EditorSceneManager.MarkSceneDirty(obj.scene);

                                    effects.particleType = selectType + 1;
                                }
                            }

                        }
                    }

                }

                GUILayout.EndArea();

                //Select intensity to apply to objects
                GUILayout.BeginArea(new Rect(10, 180, 200, 200));

                GUILayout.Label("Select effect emission SPEED", EditorStyles.miniBoldLabel);
                selectIntensity = GUI.SelectionGrid(new Rect(0, 20, 200, 50), selectIntensity, optionsIntensity, 2);
                GUILayout.Space(60.0f);
                GUILayout.Label("Select objects in scene to apply", EditorStyles.miniBoldLabel);
                GUILayout.Space(5.0f);

                if (GUILayout.Button("Apply changes to objects"))
                {
                    if (Selection.gameObjects.Length == 0)
                    {
                        EditorUtility.DisplayDialog("Error", "There are no selected objects or prefabs for this operation", "OK");
                    }
                    else
                    {
                        int semselecao = 0;
                        foreach (GameObject obj in Selection.gameObjects)
                        {
                            SoundEffect currentEffect = obj.gameObject.GetComponent<SoundEffect>();
                            if (currentEffect != null)
                            {

                                if (selectIntensity == 0)
                                {
                                    currentEffect.extremeEnabled = false;
                                    currentEffect.pIntensity = 60;
                                }
                                else if (selectIntensity == 1)
                                {
                                    currentEffect.extremeEnabled = false;
                                    currentEffect.pIntensity = 30;
                                }
                                else if (selectIntensity == 2)
                                {
                                    currentEffect.extremeEnabled = false;
                                    currentEffect.pIntensity = 15;
                                }
                                else if (selectIntensity == 3)
                                    currentEffect.extremeEnabled = true;

                                AssetDatabase.SaveAssets();
                                AssetDatabase.Refresh();
                                EditorSceneManager.MarkSceneDirty(obj.scene);
                            }
                            else
                            {
                                semselecao++;
                            }
                        }

                        if (semselecao > 0)
                        {
                            EditorUtility.DisplayDialog("Warning", "From the selected objects " + semselecao.ToString() + " objects don't have the effect applied on them. Apply effect on them and try again here.", "OK");
                        }
                    }
                }

                GUILayout.EndArea();


                //Deletion Area

                GUILayout.BeginArea(new Rect(15, 400, 200, 100));

                GUILayout.Label("Delete Effect in selected objects", EditorStyles.miniBoldLabel);
                if (GUILayout.Button("Delete effect in object"))
                {
                    if (Selection.gameObjects.Length == 0)
                    {
                        EditorUtility.DisplayDialog("Error", "There are no selected objects or prefabs for this operation", "OK");
                    }
                    else
                    {

                        if (EditorUtility.DisplayDialog("Warning", "Are you sure you want to delete the effect in selected objects", "Yes", "No"))
                        {
                            foreach (GameObject obj in Selection.gameObjects)
                            {
                                int semselecao = 0;
                                SoundEffect currentEffect = obj.gameObject.GetComponent<SoundEffect>();
                                if (currentEffect != null)
                                {
                                    DestroyImmediate(currentEffect);
                                    AssetDatabase.SaveAssets();
                                    AssetDatabase.Refresh();
                                    EditorSceneManager.MarkSceneDirty(obj.scene);
                                }
                                else
                                {
                                    semselecao++;
                                }

                                if (semselecao > 0)
                                {
                                    EditorUtility.DisplayDialog("Warning", "From the selected objects " + semselecao.ToString() + " objects already didn't have the effect to delete.", "OK");
                                }
                            }

                        }
                    }
                }
                GUILayout.Space(5.0f);
                GUILayout.Label("Delete all effects in Scene", EditorStyles.miniBoldLabel);
                GUILayout.Space(5.0f);
                if (GUILayout.Button("Delete all effects in this Scene"))
                {
                    if (EditorUtility.DisplayDialog("Warning", "Are you sure you want to deactivate UtopicSense in this scene (all saved data will be lost)", "Yes", "No"))
                    {
                        deactivateUtopicSenseInCurrentScene();
                    }
                }
                GUILayout.EndArea();

                GUILayout.EndArea();

            }

            if (selection == 1)
            {
                //simplified = true;
                // Area to select particle textures types

                GUILayout.Label("SELECT the particle texture", EditorStyles.boldLabel);

                EditorGUILayout.BeginHorizontal();
                for (int i = 0; i < NUM_TEX; i++)
                {
                    GUILayout.BeginVertical();
                    var style = new GUIStyle(GUI.skin.label);
                    style.alignment = TextAnchor.UpperCenter;
                    style.fixedWidth = 70;
                    GUILayout.Label(optionsType[i], style);
                    EditorGUI.BeginChangeCheck();
                    textura[i] = (Texture)EditorGUILayout.ObjectField(textura[i], typeof(Texture), false, GUILayout.Width(70), GUILayout.Height(70));
                    if (EditorGUI.EndChangeCheck())
                    {
                        rend[i].sharedMaterial.mainTexture = textura[i];
                    }
                    GUILayout.EndVertical();
                }
                EditorGUILayout.EndHorizontal();


                GUILayout.Space(50.0f);


                //SAVE BUTTON AREA

                GUILayout.BeginArea(new Rect(10, 230, 200, 80));
                GUILayout.Space(5.0f);
                GUILayout.Label("COLOR gradients for effects", EditorStyles.boldLabel);
                GUILayout.EndArea();

                //COLOR AREA

                GUILayout.BeginArea(new Rect(10, 280, 200, 500));

                GUILayout.Label("General colors", EditorStyles.miniBoldLabel);
                GUILayout.Space(2.0f);
                EditorGUI.BeginChangeCheck();
                colors[0] = EditorGUILayout.ColorField("Beginning color", colors[0]);
                colors[1] = EditorGUILayout.ColorField("Ending color", colors[1]);
                if (EditorGUI.EndChangeCheck())
                {
                    SaveStateToAsset();
                }

                GUILayout.Space(2.0f);

                GUILayout.EndArea();

                GUILayout.BeginArea(new Rect(RESET_DEFAULT_X, RESET_DEFAULT_Y, 200, 100));

                GUILayout.Label("Default UtopicSense Colors", EditorStyles.miniBoldLabel);
                GUILayout.Space(10.0f);

                if (GUILayout.Button("Reset default colors"))
                {
                    if (EditorUtility.DisplayDialog("Warning", "Are you sure you want to reset to default colors?", "Yes", "No"))
                    {
                        putToDefaultColors();
                        SaveStateToAsset();
                    }
                }

                GUILayout.EndArea();

                // box aside the colors

                GUILayout.BeginArea(new Rect(250, 250, 300, 500));

                GUILayout.Label("SELECT effect to be applied to objects", EditorStyles.miniBoldLabel);

                GUILayout.BeginArea(new Rect(10, 20, 250, 60));

                selectType = GUI.SelectionGrid(new Rect(0, 0, 150, 50), selectType, optionsType, 2);

                GUILayout.EndArea();



                GUILayout.BeginArea(new Rect(10, 80, 200, 100));

                GUILayout.Label("Select objects in scene to apply", EditorStyles.miniBoldLabel);
                GUILayout.Space(5.0f);

                if (GUILayout.Button("Apply effect"))
                {

                    if (Selection.gameObjects.Length == 0)
                    {
                        EditorUtility.DisplayDialog("Error", "There are no selected objects or prefabs for this operation", "OK");
                    }
                    else
                    {
                        foreach (GameObject obj in Selection.gameObjects)

                        {

                            AudioSource verification = obj.GetComponent<AudioSource>();

                            if (verification == null)
                            {
                                if (!EditorUtility.DisplayDialog("Warning", "There is no AudioSource in: '' " + obj.transform.name + "'', do still want to apply the effect?\n\n" +
                                    "(The effect will not work until there is an AudioSource that plays an AudioClip in the GameObject/Prefab)", "Don't apply", "Apply anyway"))
                                {

                                    SoundEffect reference = obj.GetComponent<SoundEffect>();
                                    if (reference == null)
                                    {
                                        SoundEffect effects = obj.gameObject.AddComponent<SoundEffect>();
                                        effects.particleType = selectType + 1;
                                        AssetDatabase.SaveAssets();
                                        AssetDatabase.Refresh();
                                        EditorSceneManager.MarkSceneDirty(obj.scene);
                                    }
                                    else
                                    {
                                        DestroyImmediate(reference);
                                        SoundEffect effects = obj.gameObject.AddComponent<SoundEffect>();
                                        AssetDatabase.SaveAssets();
                                        AssetDatabase.Refresh();
                                        EditorSceneManager.MarkSceneDirty(obj.scene);

                                        effects.particleType = selectType + 1;
                                    }
                                }
                            }
                            else
                            {

                                SoundEffect reference = obj.GetComponent<SoundEffect>();
                                if (reference == null)
                                {
                                    SoundEffect effects = obj.gameObject.AddComponent<SoundEffect>();
                                    effects.particleType = selectType + 1;
                                    AssetDatabase.SaveAssets();
                                    AssetDatabase.Refresh();
                                    EditorSceneManager.MarkSceneDirty(obj.scene);
                                }
                                else
                                {
                                    DestroyImmediate(reference);
                                    SoundEffect effects = obj.gameObject.AddComponent<SoundEffect>();
                                    AssetDatabase.SaveAssets();
                                    AssetDatabase.Refresh();
                                    EditorSceneManager.MarkSceneDirty(obj.scene);

                                    effects.particleType = selectType + 1;
                                }
                            }

                        }
                    }

                }

                GUILayout.EndArea();

                //Select intensity to apply to objects
                GUILayout.BeginArea(new Rect(10, 180, 200, 200));

                GUILayout.Label("Select particle emission SPEED", EditorStyles.miniBoldLabel);
                selectIntensity = GUI.SelectionGrid(new Rect(0, 20, 200, 50), selectIntensity, optionsIntensity, 2);
                GUILayout.Space(60.0f);
                GUILayout.Label("Select objects in scene to apply", EditorStyles.miniBoldLabel);
                GUILayout.Space(5.0f);

                if (GUILayout.Button("Apply changes to objects"))
                {
                    if (Selection.gameObjects.Length == 0)
                    {
                        EditorUtility.DisplayDialog("Error", "There are no selected objects or prefabs for this operation", "OK");
                    }
                    else
                    {
                        int semselecao = 0;
                        foreach (GameObject obj in Selection.gameObjects)
                        {
                            SoundEffect currentEffect = obj.gameObject.GetComponent<SoundEffect>();
                            if (currentEffect != null)
                            {

                                if (selectIntensity == 0)
                                    currentEffect.pIntensity = 15;
                                else if (selectIntensity == 2)
                                    currentEffect.pIntensity = 3;
                                else
                                    currentEffect.pIntensity = 5;

                                AssetDatabase.SaveAssets();
                                AssetDatabase.Refresh();
                                EditorSceneManager.MarkSceneDirty(obj.scene);

                            }
                            else
                            {
                                semselecao++;
                            }
                        }

                        if (semselecao > 0)
                        {
                            EditorUtility.DisplayDialog("Warning", "From the selected objects " + semselecao.ToString() + " objects don't have the effect applied on them. Apply effect on them and try again here.", "OK");
                        }
                    }
                }

                GUILayout.EndArea();


                //Deletion Area

                GUILayout.BeginArea(new Rect(15, 400, 200, 100));

                GUILayout.Label("Delete Effect in selected objects", EditorStyles.miniBoldLabel);
                if (GUILayout.Button("Delete effect in object"))
                {
                    if (Selection.gameObjects.Length == 0)
                    {
                        EditorUtility.DisplayDialog("Error", "There are no selected objects or prefabs for this operation", "OK");
                    }
                    else
                    {

                        if (EditorUtility.DisplayDialog("Warning", "Are you sure you want to delete the effect in selected objects", "Yes", "No"))
                        {
                            foreach (GameObject obj in Selection.gameObjects)
                            {
                                int semselecao = 0;
                                SoundEffect currentEffect = obj.gameObject.GetComponent<SoundEffect>();
                                if (currentEffect != null)
                                {
                                    DestroyImmediate(currentEffect);
                                    AssetDatabase.SaveAssets();
                                    AssetDatabase.Refresh();
                                    EditorSceneManager.MarkSceneDirty(obj.scene);
                                }
                                else
                                {
                                    semselecao++;
                                }

                                if (semselecao > 0)
                                {
                                    EditorUtility.DisplayDialog("Warning", "From the selected objects " + semselecao.ToString() + " objects already didn't have the effect to delete.", "OK");
                                }
                            }
                        }
                    }
                }
                GUILayout.Space(5.0f);
                GUILayout.Label("Delete all effects in Scene", EditorStyles.miniBoldLabel);
                GUILayout.Space(5.0f);
                if (GUILayout.Button("Delete all effects in this Scene"))
                {
                    if (EditorUtility.DisplayDialog("Warning", "Are you sure you want to deactivate UtopicSense in this scene (all saved data will be lost)", "Yes", "No"))
                    {
                        deactivateUtopicSenseInCurrentScene();
                    }
                }

                GUILayout.EndArea();

                GUILayout.EndArea();

            }

            GUILayout.Label("", GUILayout.Width(USENSE_WIDTH), GUILayout.Height(USENSE_HEIGHT));
            EditorGUILayout.EndScrollView();
        }


        private void createUtopicSense()
        {
            utopicSenseVisualScript = CreateInstance<UtopicSenseVisual>();

            // estado inicial consistente
            utopicSenseVisualScript.utopicActive = true;
            utopicSenseVisualScript.simplified = false;
            utopicSenseVisualScript.typeShader = 0;

            putToDefaultColors();

            // copia defensiva
            utopicSenseVisualScript.colors = (Color[])colors.Clone();

            AssetDatabase.CreateAsset(
                utopicSenseVisualScript,
                "Assets/UtopicSense/Resources/PrefabUS/UtopicSense.asset"
            );

            EditorUtility.SetDirty(utopicSenseVisualScript);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private void deactivateUtopicSenseInCurrentScene()
        {
            SoundEffect[] effectsUsed =
                UnityEngine.Object.FindObjectsByType<SoundEffect>(
                    FindObjectsSortMode.None
                );

            if (effectsUsed == null || effectsUsed.Length == 0)
                return;

            for (int i = 0; i < effectsUsed.Length; i++)
            {
                DestroyImmediate(effectsUsed[i]);
            }
        }

        private void putToDefaultColors()
        {
            colors[0] = new Color(0.9866f, 0.0f, 1.0f, 1.0f);
            colors[1] = new Color(1.0f, 0.0f, 0.5785f, 1.0f);

            colors[2] = new Color(1.0f, 0.0f, 0.2964f, 1.0f);
            colors[3] = new Color(1.0f, 0.1216f, 0.0f, 1.0f);

            colors[4] = new Color(1.0f, 0.3279f, 0.0f, 1.0f);
            colors[5] = new Color(1.0f, 0.5504f, 0.0156f, 1.0f);

            colors[6] = new Color(1.0f, 0.8853f, 0.0156f, 1.0f);
            colors[7] = new Color(0.7819f, 1.0f, 0.0f, 1.0f);

            colors[8] = new Color(0.1153f, 1.0f, 0.0f, 1.0f);
            colors[9] = new Color(0.0f, 1.0f, 0.6069f, 1.0f);

            colors[10] = new Color(0.0f, 0.3061f, 0.9725f, 1.0f);
            colors[11] = new Color(0.0f, 0.0f, 1.0f, 1.0f);

            colors[12] = new Color(0.3078f, 0.0f, 1.0f, 1.0f);
            colors[13] = new Color(0.5144f, 0.0f, 1.0f, 1.0f);
        }

        private void LoadStateFromAsset()
        {
            if (utopicSenseVisualScript == null)
                return;

            colors = new Color[COLOR_NUM];
            utopicSenseVisualScript.colors.CopyTo(colors, 0);

            simplified = utopicSenseVisualScript.simplified;
            selection = simplified ? 1 : 0;
            selectionShader = utopicSenseVisualScript.typeShader;
        }

        private void SaveStateToAsset()
        {
            if (utopicSenseVisualScript == null)
                return;

            if (utopicSenseVisualScript.colors == null ||
                utopicSenseVisualScript.colors.Length != COLOR_NUM)
            {
                utopicSenseVisualScript.colors = new Color[COLOR_NUM];
            }

            colors.CopyTo(utopicSenseVisualScript.colors, 0);

            utopicSenseVisualScript.simplified = simplified;
            utopicSenseVisualScript.typeShader = selectionShader;

            EditorUtility.SetDirty(utopicSenseVisualScript);
            AssetDatabase.SaveAssets();
        }
    }

}
