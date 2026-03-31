using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace utopicsense 
{
    public class CircleParticleBehavior : MonoBehaviour
    {
        private float tempo;
        private ParticleSystem particulas;
        // Start is called before the first frame update
        void Start()
        {
            particulas = this.GetComponent<ParticleSystem>();
            tempo = particulas.main.duration + 0.2f;
        }

        // Update is called once per frame
        void Update()
        {
            tempo -= Time.deltaTime;
            if (tempo <= 0)
            {

                Destroy(this.gameObject);
            }
        }
    }
}

