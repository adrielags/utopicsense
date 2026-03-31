using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace utopicsense 
{
    public class UtopicSenseVisual : ScriptableObject
    {
        private const int COLOR_NUM = 14;
        private const int NUM_TEX = 5;
        public Color[] colors = new Color[COLOR_NUM];
        public bool simplified;
        public bool utopicActive;
        public int typeShader;

    }
}

