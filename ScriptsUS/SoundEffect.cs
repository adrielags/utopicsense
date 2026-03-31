using UnityEngine;

namespace utopicsense
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundEffect : MonoBehaviour
    {
        // VISUAL SETTINGS

        [Header("Visual Effect")]
        [Range(1, 5)]
        public int particleType = 1;

        public bool simplified = false;
        public bool extremeEnabled = false;

        [Tooltip("Lower = more particles")]
        public int pIntensity = 15;


        // COLOR SETTINGS

        [Header("Colors")]
        public Color[] colors = new Color[14];

        // INTERNAL CONSTANTS

        private const int SAMPLE_SIZE = 512;

        private readonly float[] freqBands =
        {
            20f, 100f, 500f, 1000f, 2000f, 5000f, 9000f, 20000f
        };

        // INTERNAL STATE


        private float[] spectrum = new float[SAMPLE_SIZE];
        private float sampleRate;

        private AudioSource[] audioSources;
        private GameObject particlePrefab;

        private int particleCounter = 0;


        // Global config
        private UtopicSenseVisual globalConfig;
        [SerializeField] private UtopicSenseVisual visualConfig;

        void Awake()
        {
            audioSources = GetComponents<AudioSource>();
            sampleRate = AudioSettings.outputSampleRate;
            particlePrefab = LoadParticlePrefab(particleType);

            if (visualConfig == null)
            {
                visualConfig = Resources.Load<UtopicSenseVisual>(
                    "PrefabUS/UtopicSense"
                );
            }

            if (visualConfig != null)
            {
                colors = visualConfig.colors;
                simplified = visualConfig.simplified;
            }
        }

        void Update()
        {
            if (particlePrefab == null)
                return;

            foreach (var audio in audioSources)
            {
                if (audio == null || !audio.isPlaying)
                    continue;

                float pitch = simplified ? 0f : AnalyzePitch(audio);
                int band = simplified ? 0 : GetFrequencyBand(pitch);

                TrySpawnVisual(audio, band);
            }
        }

        void LoadGlobalConfig()
        {
            globalConfig = Resources.Load<UtopicSenseVisual>(
                "PrefabUS/UtopicSense"
            );

            if (globalConfig == null)
            {
                Debug.LogWarning(
                    "[UtopicSense] Global config not found. Using local values."
                );
                return;
            }

            // Only override if colors are empty
            if (colors == null || colors.Length != globalConfig.colors.Length)
            {
                colors = globalConfig.colors;
            }

            simplified = globalConfig.simplified;
        }

        float AnalyzePitch(AudioSource audio)
        {
            audio.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);

            float maxV = 0f;
            int maxN = 0;

            for (int i = 0; i < spectrum.Length; i++)
            {
                if (spectrum[i] > maxV)
                {
                    maxV = spectrum[i];
                    maxN = i;
                }
            }

            return maxN * (sampleRate / 2f) / SAMPLE_SIZE;
        }

        int GetFrequencyBand(float pitch)
        {
            for (int i = 0; i < freqBands.Length - 1; i++)
            {
                if (pitch > freqBands[i] && pitch < freqBands[i + 1])
                    return i;
            }

            return 0;
        }

        void TrySpawnVisual(AudioSource audio, int band)
        {
            if (!extremeEnabled)
            {
                particleCounter++;
                if (particleCounter < Mathf.Max(1, EffectiveIntensity()))
                    return;

                particleCounter = 0;
            }

            GameObject instance = Instantiate(
                particlePrefab,
                transform.position,
                Quaternion.identity
            );

            var ps = instance.GetComponent<ParticleSystem>();
            if (ps == null)
                return;

            var main = ps.main;
            main.startSize = (audio.maxDistance + 6f) * 2f;

            var col = ps.colorOverLifetime;
            col.enabled = true;
            col.color = GenerateColor(band, audio.volume);
        }


        Gradient GenerateColor(int band, float volume)
        {
            if (colors == null || colors.Length < 2)
                return TransparentGradient();

            if (simplified || band * 2 + 1 >= colors.Length)
            {
                return TwoColorGradient(colors[0], colors[1], volume);
            }

            return TwoColorGradient(
                colors[band * 2],
                colors[band * 2 + 1],
                volume
            );
        }

        Gradient TwoColorGradient(Color a, Color b, float v)
        {
            Gradient g = new Gradient();
            g.SetKeys(
                new[]
                {
                    new GradientColorKey(a, 0f),
                    new GradientColorKey(b, 1f)
                },
                new[]
                {
                    new GradientAlphaKey(v, 0f),
                    new GradientAlphaKey(0f, 1f)
                }
            );
            return g;
        }

        Gradient TransparentGradient()
        {
            Gradient g = new Gradient();
            g.SetKeys(
                new[] { new GradientColorKey(Color.clear, 0f) },
                new[] { new GradientAlphaKey(0f, 0f) }
            );
            return g;
        }



        GameObject LoadParticlePrefab(int type)
        {
            if (type < 1 || type > 5)
                return null;

            return Resources.Load<GameObject>(
                $"ParticlesUS/CircleParticle{type}"
            );
        }

        int EffectiveIntensity()
        {
            if (extremeEnabled)
                return 1;

            // Simplified more call → more spacing
            return simplified
                ? pIntensity * 2
                : pIntensity;
        }
    }
}
