using UnityEngine;
using UnityEditor;

namespace utopicsense
{
    public class ColorHelp : EditorWindow
    {
        private Texture2D gradientExample;
        private Vector2 scrollPos;

        private GUIStyle titleStyle;
        private GUIStyle bodyStyle;
        private GUIStyle sectionStyle;
        private GUIStyle noteStyle;

        [MenuItem("Window/UtopicSense/Help/Color Help")]
        public static void ShowWindow()
        {
            ColorHelp window = GetWindow<ColorHelp>("Color Help");
            window.minSize = new Vector2(420, 600);
        }

        private void OnEnable()
        {
            gradientExample = Resources.Load<Texture2D>("example/gradientexample");
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
            GUILayout.Label("Color Mapping Overview", titleStyle);
            GUILayout.Space(15);

            GUILayout.Label(
                "Here you select the <b>colors</b> that will be applied to your visual sound effects.",
                bodyStyle
            );

            GUILayout.Space(10);

            GUILayout.Label(
                "Each effect uses a <b>Beginning Color</b> and an <b>Ending Color</b> to form a color gradient.",
                bodyStyle
            );

            GUILayout.Space(10);

            GUILayout.Label(
                "For example, if the beginning color is <b>blue</b> and the ending color is <b>red</b>, the generated gradient will look like this:",
                bodyStyle
            );

            GUILayout.Space(8);

            if (gradientExample != null)
            {
                GUILayout.Label(gradientExample, GUILayout.MaxWidth(380));
            }

            GUILayout.Space(15);
            GUILayout.Label("How gradients work", sectionStyle);

            GUILayout.Label(
                "The gradient represents the <b>color over the lifetime</b> of a particle.\n" +
                "In this example, the particle starts blue, transitions through purple, and ends red.",
                bodyStyle
            );

            GUILayout.Space(15);
            GUILayout.Label("Mapping modes", sectionStyle);

            GUILayout.Label(
                "<b>Standard</b>\n" +
                "All frequency bands are available. Particle colors are dynamically mapped based on the sound pitch.",
                bodyStyle
            );

            GUILayout.Space(8);

            GUILayout.Label(
                "<b>Simple</b>\n" +
                "All sounds use a single color gradient, without pitch-based variation.",
                bodyStyle
            );

            GUILayout.Space(15);
            GUILayout.Label("Health warning", sectionStyle);

            GUILayout.Label(
                "A small percentage of people may experience discomfort or seizures when exposed to certain color patterns or flashing visuals.",
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
