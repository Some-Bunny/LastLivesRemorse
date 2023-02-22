using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LastLivesRemorse
{
    public static class Toolbox
    {
        public static Vector2 GetUnitOnCircle(float angleDegrees, float radius)
        {

            // initialize calculation variables
            float _x = 0;
            float _y = 0;
            float angleRadians = 0;
            Vector2 _returnVector;

            // convert degrees to radians
            angleRadians = angleDegrees * Mathf.PI / 180.0f;

            // get the 2D dimensional coordinates
            _x = radius * Mathf.Cos(angleRadians);
            _y = radius * Mathf.Sin(angleRadians);

            // derive the 2D vector
            _returnVector = new Vector2(_x, _y);

            // return the vector info
            return _returnVector;
        }
    }
}
