using System;


namespace EyeOut.mot
{
    public struct S_EulerAngles
    {
        public S_EulerAngles(float yaw, float pitch, float roll)
            : this()
        {
            Yaw = yaw;
            Pitch = pitch;
            Roll = roll;
        }

        public float Yaw { get; set; }
        public float Pitch { get; set; }
        public float Roll { get; set; }

        public static bool operator !=(S_EulerAngles _eulerAngle1, S_EulerAngles _eulerAngle2)
        {
            return _eulerAngle1.Equals(_eulerAngle2) == false;
        }

        public static bool operator ==(S_EulerAngles _eulerAngle1, S_EulerAngles _eulerAngle2)
        {
            return _eulerAngle1.Equals(_eulerAngle2);
        }
        /*
        public bool Equals(S_EulerAngles other)
        {
            return Pitch.IsNearlyEqual(other.Pitch) && Yaw.IsNearlyEqual(other.Yaw) &&
                Roll.IsNearlyEqual(other.Roll);
        }

        public override bool Equals(object other)
        {
            return other is S_EulerAngles ? Equals((S_EulerAngles)other) : base.Equals(other);
        }
        */
        public override int GetHashCode()
        {
            return Pitch.GetHashCode() ^ Yaw.GetHashCode() ^ Roll.GetHashCode();
        }

        public override string ToString()
        {
            return "Pitch: " + Pitch + " Yaw: " + Yaw + " Roll: " + Roll;
        }
    }
}
