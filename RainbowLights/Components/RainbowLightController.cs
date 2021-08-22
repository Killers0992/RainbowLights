using MapGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RainbowLights.Components
{
    public class RainbowLightController : MonoBehaviour
    {
        public static readonly List<RainbowLightController> Instances = new List<RainbowLightController>();
        public float _saturation = 1f;
        public float _hueShiftSpeed = 0.2f;
        public float _value = 1f;
        private bool state = false;

        private FlickerableLightController _light;
        public FlickerableLightController light
        {
            get
            {
                if (_light == null)
                    _light = GetComponent<FlickerableLightController>();
                return _light;
            }
        }

        private RoomIdentifier _room;
        public RoomIdentifier room
        {
            get
            {
                if (_room == null)
                    _room = GetComponent<RoomIdentifier>();
                return _room;
            }
        }

        public void SetState(bool state)
        {
            this.state = state;
            light.Network_warheadLightOverride = state;
        }

        private void Awake()
        {
            Instances.Add(this);
        }

        private void OnDestroy()
        {
            Instances.Remove(this);
        }

        private void Update()
        {
            if (!state)
                return;
            
            float amountToShift = _hueShiftSpeed * Time.deltaTime;
            Color newColor = ShiftHueBy(light.Network_warheadLightColor, amountToShift);
            light.Network_warheadLightColor = newColor;
        }

        private Color ShiftHueBy(Color color, float amount)
        {
            // convert from RGB to HSV
            Color.RGBToHSV(color, out float hue, out float sat, out float val);

            // shift hue by amount
            hue += amount;
            sat = _saturation;
            val = _value;

            // convert back to RGB and return the color
            return Color.HSVToRGB(hue, sat, val);
        }
    }
}
