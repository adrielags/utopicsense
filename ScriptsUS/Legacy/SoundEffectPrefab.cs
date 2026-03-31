using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace utopicsense {
    public class SoundEffectPrefab : MonoBehaviour
    // LEGACY SCRIPT
    // Used in early UtopicSense versions for prefab-only workflows.
    // Kept for reference and backward compatibility. Not used in 1.1+

    {
        private const int NUM_FREQ = 8;
        private GameObject visualEffect;


        public float[] valoresFreq = new float[NUM_FREQ];

        private const int COLOR_NUM = 14;
        public Color[] colors = new Color[COLOR_NUM];
        public bool simplified;

        //Analyse sound
        private const int SAMPLE_SIZE = 512;
        private float[] samples = new float[SAMPLE_SIZE];
        private float[] spectrum = new float[SAMPLE_SIZE];
        private float sampleRate;
        private float pitchValue;
        private AudioSource[] parentAudio;
        private Transform parent;
        private GameObject visualEffectClone;
        private int updateNumberParticle;

        // Start is called before the first frame update
        void Awake()
        {
            parent = this.transform.parent;
            parentAudio = this.GetComponents<AudioSource>();
            sampleRate = AudioSettings.outputSampleRate;


            if (transform.tag == "visualUS1")
            {
                visualEffect = Resources.Load<GameObject>("ParticlesUS/CircleParticle1");
            }
            else if (transform.tag == "visualUS2")
            {
                visualEffect = Resources.Load<GameObject>("ParticlesUS/CircleParticle2");

            }
            else if (transform.tag == "visualUS3")
            {
                visualEffect = Resources.Load<GameObject>("ParticlesUS/CircleParticle3");

            }
            else if (transform.tag == "visualUS4")
            {
                visualEffect = Resources.Load<GameObject>("ParticlesUS/CircleParticle4");
            }
            else if (transform.tag == "visualUS5")
            {
                visualEffect = Resources.Load<GameObject>("ParticlesUS/CircleParticle5");
            }
            else
            {
                Destroy(this);
            }


            updateNumberParticle = 0;
            valoresFreq[0] = 20.0f;
            valoresFreq[1] = 100.0f;
            valoresFreq[2] = 500.0f;
            valoresFreq[3] = 1000.0f;
            valoresFreq[4] = 2000.0f;
            valoresFreq[5] = 5000.0f;
            valoresFreq[6] = 9000.0f;
            valoresFreq[7] = 20000.0f;

        }

        // Update is called once per frame
        void Update()
        {

            if (parentAudio != null)
            {

                float[] radius = new float[parentAudio.Length];
                float[] volume = new float[parentAudio.Length];
                for (int i = 0; i < parentAudio.Length; i++)
                {
                    radius[i] = parentAudio[i].maxDistance;
                    volume[i] = parentAudio[i].volume;
                }

                for (int i = 0; i < parentAudio.Length; i++)
                {
                    generateEffectToPlayingAudio(parentAudio[i], volume[i], radius[i]);
                }

            }

        }


        private void AnalyzeSoundSpectrum(AudioSource parentAudio)
        {
            // Get sound spectrum 

            parentAudio.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);

            // find pitch

            float maxV = 0;
            var maxN = 0;

            for (int i = 0; i < SAMPLE_SIZE; i++)
            {
                if (!(spectrum[i] > maxV) || !(spectrum[i] > 0.0f))
                    continue;

                maxV = spectrum[i];
                maxN = i;

            }

            float freqN = maxN;
            if (maxN > 0 && maxN < SAMPLE_SIZE - 1)
            {

                var dL = spectrum[maxN - 1] / spectrum[maxN];
                var dR = spectrum[maxN + 1] / spectrum[maxN];

                freqN += 0.5f * (dR * dR - dL * dL);
            }

            pitchValue = freqN * (sampleRate / 2) / SAMPLE_SIZE;
            //print(pitchValue);

        }


        private Gradient generateGradient(Color color1, Color color2, float volume)
        {

            Gradient grad = new Gradient();
            grad.SetKeys(new GradientColorKey[] { new GradientColorKey(color1, 0.0f), new GradientColorKey(color2, 1.0f) }, new GradientAlphaKey[] { new GradientAlphaKey(1.0f * volume, 0.0f), new GradientAlphaKey(0.0f, 1.0f) });
            return grad;

        }

        private void generateEffectToPlayingAudio(AudioSource parentAudio, float volume, float radius)
        {

            if (parentAudio != null && parentAudio.clip.length > 0)
            {

                var particle = visualEffect.GetComponent<ParticleSystem>();
                var col = particle.colorOverLifetime;
                col.enabled = true;

                if (parentAudio.isPlaying && !simplified)
                    AnalyzeSoundSpectrum(parentAudio);

                //how to change color gradient and transparency based on volume
                if (volume > 0)
                {

                    if (simplified)
                    {
                        col.color = generateGradient(colors[0], colors[1], volume);
                    }
                    else
                    {

                        if (parentAudio.isPlaying)
                        {

                            if (valoresFreq[0] < pitchValue && pitchValue < valoresFreq[1])
                            {
                                col.color = generateGradient(colors[0], colors[1], volume);
                            }

                            else if (valoresFreq[1] < pitchValue && pitchValue < valoresFreq[2])
                            {
                                col.color = generateGradient(colors[2], colors[3], volume);
                            }

                            else if (valoresFreq[2] < pitchValue && pitchValue < valoresFreq[3])
                            {
                                col.color = generateGradient(colors[4], colors[5], volume);
                            }

                            else if (valoresFreq[3] < pitchValue && pitchValue < valoresFreq[4])
                            {
                                col.color = generateGradient(colors[6], colors[7], volume);
                            }

                            else if (valoresFreq[4] < pitchValue && pitchValue < valoresFreq[5])
                            {
                                col.color = generateGradient(colors[8], colors[9], volume);
                            }

                            else if (valoresFreq[5] < pitchValue && pitchValue < valoresFreq[6])
                            {
                                col.color = generateGradient(colors[10], colors[11], volume);
                            }

                            else if (valoresFreq[6] < pitchValue && pitchValue < valoresFreq[7])
                            {
                                col.color = generateGradient(colors[12], colors[13], volume);
                            }
                            else
                            {
                                col.color = generateGradient(Color.clear, Color.clear, volume);
                            }
                        }
                    }

                }


                ParticleSystem.MainModule psmain = particle.main;
                psmain.startSize = (radius + 6) * 2;

                if (parentAudio.isPlaying)
                {
                    if (updateNumberParticle == 0)
                        visualEffectClone = Instantiate(visualEffect, new Vector3(transform.position.x, transform.position.y, transform.position.z), transform.rotation);
                    if (parentAudio.clip.length < 0.1)
                        visualEffectClone = Instantiate(visualEffect, new Vector3(transform.position.x, transform.position.y, transform.position.z), transform.rotation);
                }

            }
            updateNumberParticle++;

            Debug.Log(parentAudio.clip.length);



            if (updateNumberParticle == 5) 
            {
                updateNumberParticle = 0;
            }
        }
    }
}

