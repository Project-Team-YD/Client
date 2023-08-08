using System.Collections;
using UnityEngine;

namespace HSMLibrary.Extensions
{
    public static class MathHelper
    {
        public static float Distance(Vector2 _curPos, Vector2 _targetPos)
        {
            float xx = _targetPos.x - _curPos.x;
            float yy = _targetPos.y - _curPos.y;

            float len = Mathf.Sqrt((xx * xx) + (yy * yy));

            return len;
        }

        public static int TwoPointAngleDirection(Vector2 _targetPos, Vector2 _curPos)
        {
            float angle = TwoPointAngle(_targetPos, _curPos);

            if (angle != 0.0f)
            {
                if (Mathf.Abs(angle) < 45.0f)
                {
                    return 1;
                }

                if (Mathf.Abs(angle) > 135.0f)
                {
                    return 3;
                }
            }

            return -1;
        }

        public static float TwoPointAngle(Vector2 _targetPos, Vector2 _curPos)
        {
            float xx = _targetPos.x - _curPos.x;
            float yy = _targetPos.y - _curPos.y;

            float rad = Mathf.Atan2(yy, xx);
            float angle = rad * 180f / Mathf.PI;

            return angle;
        }

        public static float TwoPointRadian(Vector2 _targetPos, Vector2 _curPos)
        {
            float xx = _targetPos.x - _curPos.x;
            float yy = _targetPos.y - _curPos.y;

            float rad = Mathf.Atan2(yy, xx);

            return rad;
        }

        public static float TwoPointAngle360(Vector2 _targetPos, Vector2 _curPos)
        {
            float xx = _targetPos.x - _curPos.x;
            float yy = _targetPos.y - _curPos.y;

            float rad = Mathf.Atan2(yy, xx);
            float angle = rad * 180f / Mathf.PI;
            if (angle < 0f)
                angle = 360f + angle;

            return angle;
        }

        public static float TwoPointAngle360Nomalize(Vector2 _targetPos, Vector2 _curPos)
        {
            float angle = TwoPointAngle360(_targetPos, _curPos);
            int quadrant = (int)(angle / 90f);

            angle = 90f * (quadrant + 1);

            return angle;
        }

        public static float AngleNomalize(float _minAngle, float _maxAngle)
        {
            return (_minAngle + _maxAngle) * 0.5f;
        }

        public static float TwoPointLength(Vector2 _targetPos, Vector2 _curPos)
        {
            float xx = _targetPos.x - _curPos.x;
            float yy = _targetPos.y - _curPos.y;
            float len = Mathf.Sqrt((xx * xx) + (yy * yy));

            return len;
        }

        public static float TwoPointLengthScreen(Vector2 _targetPos, Vector2 _curPos)
        {
            float targetX = _targetPos.x * (float)(Screen.width * 0.5f);
            float targetY = _targetPos.y * (float)(Screen.height * 0.5f);
            Vector2 targetVec = new Vector2(targetX, targetY);

            float curX = _curPos.x * (float)(Screen.width * 0.5f);
            float curY = _curPos.y * (float)(Screen.height * 0.5f);
            Vector2 curVec = new Vector2(curX, curY);

            float xx = targetVec.x - curVec.x;
            float yy = targetVec.y - curVec.y;
            float len = Mathf.Sqrt((xx * xx) + (yy * yy));

            return len;
        }

        public static Vector2 DirVector(Vector2 _targetPos, Vector2 _curPos)
        {
            float xx = _targetPos.x - _curPos.x;
            float yy = _targetPos.y - _curPos.y;
            float len = Mathf.Sqrt((xx * xx) + (yy * yy));

            float dx = xx / len;
            float dy = yy / len;

            //.. FIXME? :: Remove IsNan
            if (float.IsNaN(dx) || float.IsNaN(dy))
            {
                dx = 0.0f;
                dy = 0.0f;
            }

            return new Vector2(dx, dy);
        }
    }
}