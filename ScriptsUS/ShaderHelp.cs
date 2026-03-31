using UnityEngine;
using UnityEditor;

namespace utopicsense
{
    public class ShaderHelp : EditorWindow
    {
        private Texture2D shaderExample;
        private Vector2 scrollPos;

        private GUIStyle titleStyle;
        private GUIStyle bodyStyle;
        private GUIStyle sectionStyle;
        private GUIStyle noteStyle;

        [MenuItem("Window/UtopicSense/Help/Shader Help")]
        public static void ShowWindow()
        {
            ShaderHelp window = GetWindow<ShaderHelp>("Shader Help");
            window.minSize = new Vector2(420, 600);
        }

        private void OnEnable()
        {
            shaderExample = Resources.Load<Texture2D>("example/shaderexample");
            InitStyles();
        }

        private void InitStyles()
        {
            titleStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 16,
                alignment = TextAnchor.MiddleCenter
            };

            sectionStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 13
            };

            bodyStyle = new GUIStyle(EditorStyles.label)
            {
                wordWrap = true,
                richText = true,
                fontSize = 12
            };

            noteStyle = new GUIStyle(bodyStyle)
            {
                fontStyle = FontStyle.Italic
            };
        }

        private void OnGUI()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            GUILayout.Space(10);
            GUILayout.Label("Shader Overview", titleStyle);
            GUILayout.Space(15);

            GUILayout.Label(
                "Here you select the <b>shader</b> that will be applied to your visual effect texture.",
                bodyStyle
            );

            GUILayout.Space(10);

            GUILayout.Label(
                "A <b>shader</b> is a small program responsible for how light, color and transparency behave in a visual effect.",
                bodyStyle
            );

            GUILayout.Space(15);
            GUILayout.Label("How shaders affect colors", sectionStyle);

            GUILayout.Label(
                "Color, shaders and particle effects work together to define the final look of the visual feedback.",
                bodyStyle
            );

            GUILayout.Space(10);

            if (shaderExample != null)
            {
                GUILayout.Label("Example (orange color):", EditorStyles.miniBoldLabel);
                GUILayout.Space(5);
                GUILayout.Label(shaderExample, GUILayout.MaxWidth(380));
            }

            GUILayout.Space(15);
            GUILayout.Label("Shader types", sectionStyle);

            GUILayout.Label(
                "<b>Default (Sprites/Default)</b>\n" +
                "Keeps colors exactly as selected. Recommended for accuracy.",
                bodyStyle
            );

            GUILayout.Space(8);

            GUILayout.Label(
                "<b>Additive</b>\n" +
                "Bright colors stand out more. Dark colors tend to become transparent.",
                bodyStyle
            );

            GUILayout.Space(8);

            GUILayout.Label(
                "<b>Multiply (Double)</b>\n" +
                "Colors are multiplied with the background, producing darker and blended results.",
                bodyStyle
            );

            GUILayout.Space(15);
            GUILayout.Label(
                "Tip: If you want color precision, start with the <b>Default</b> shader.",
                noteStyle
            );

            GUILayout.Space(20);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Close", GUILayout.Width(100)))
            {
                Close();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            EditorGUILayout.EndScrollView();
        }
    }
}
