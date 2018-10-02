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
            SlowRadius = 10f;
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
                desiredSpeed = MaxSpeed * (distance / SlowRadius);
            }
            base.Character.velocity = direction.normalized * desiredSpeed;

            return base.GetMovement();
        }
    }
}