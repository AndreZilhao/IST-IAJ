using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement

{
    public class DynamicArrive : DynamicVelocityMatch
    {

        public override string Name
        {
            get { return "DynamicArrive"; }
        }

        public float MaxSpeed { get; set; }
        public float StopRadius { get; set; }
        public float SlowRadius { get; set; }
        public KinematicData DestinationTarget { get; set; }

        public DynamicArrive()
        {
            MaxSpeed = 20f;
            StopRadius = 2f;
            SlowRadius = 30f;
            this.Output = new MovementOutput();
        }

        public override MovementOutput GetMovement()
        {

            Vector3 direction = DestinationTarget.Position - Character.Position;
            float distance = direction.magnitude;
            float desiredSpeed = 0;
            if (distance < StopRadius)
            {
                desiredSpeed = 0;
            }
            else if (distance > SlowRadius)
            {
                desiredSpeed = MaxSpeed;
            }
            else
            {
                desiredSpeed = MaxSpeed * (distance / (SlowRadius*2));
            }
            Vector3 desiredVelocity = direction.normalized * desiredSpeed;

            //Same as Velocity Matching, but now using desiredVelocity
            this.Output.linear = (desiredVelocity - this.Character.velocity) / this.TimeToDesiredSpeed;

            if (this.Output.linear.sqrMagnitude > this.MaxAcceleration * this.MaxAcceleration)
            {
                this.Output.linear = this.Output.linear.normalized * this.MaxAcceleration;
            }
            this.Output.angular = 0;
            return this.Output;
        }
    }
}