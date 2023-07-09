using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;

namespace GPHive.Core
{
    public class LevelSettings : MonoBehaviour
    {
        public LevelConfig levelConfig;
        public List<GameObject> Enemies = new List<GameObject>();

        public Color basketZoonParticleColor;
        [SerializeField] ParticleSystem[] basketZoons;

        private void OnEnable()
        {
            foreach (var item in basketZoons)
            {
                item.startColor = basketZoonParticleColor;
                item.GetComponent<Renderer>().material.SetColor("_EmissionColor", basketZoonParticleColor * 1);
            }

            if (levelConfig.enableVolume)
                FindObjectOfType<Volume>().profile = levelConfig.volumeProfile;


            if (levelConfig.changeSkybox)
                RenderSettings.skybox = levelConfig.skybox;

            RenderSettings.fog = levelConfig.fog;

            if (levelConfig.fog)
            {
                RenderSettings.fogColor = levelConfig.fogColor;

                if (levelConfig.fogMode == FogMode.Linear)
                {
                    RenderSettings.fogStartDistance = levelConfig.fogStart;
                    RenderSettings.fogEndDistance = levelConfig.fogEnd;
                }
                else
                    RenderSettings.fogDensity = levelConfig.fogDensity;
            }
        }
        private void OnDestroy()
        {
            foreach (var item in Enemies)
            {
                GameObject.Destroy(item);
            }
        }

        private void Reset()
        {
            levelConfig.fogMode = FogMode.Linear;
        }
    }
}