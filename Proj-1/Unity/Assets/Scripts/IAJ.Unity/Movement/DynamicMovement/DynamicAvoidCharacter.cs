using UnityEngine;
using UnityEditor;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement
{

    public class DynamicAvoidCharacter : DynamicMovement
    {
        public float CollisionRadius { get; set; }
        public float MaxTimeLookAhead { get; set; }
        public KinematicData destinationTarget { get; set; }

        public override string Name
        {
            get { return "AvoidCharacter"; }
        }

        public DynamicAvoidCharacter(KinematicData otherCharacter)
        {
            destinationTarget = otherCharacter;
            this.Output = new MovementOutput();
        }

        public override MovementOutput GetMovement()
        {
            Vector3 deltaPos = destinationTarget.Position - Character.Position;
            Vector3 deltaVel = destinationTarget.velocity - Character.velocity;
            float deltaSqrSpeed = deltaVel.sqrMagnitude;

            if (deltaSqrSpeed == 0) return new MovementOutput(); //velocity check

            float timeToClosest = -Vector3.Dot(deltaPos, deltaVel) / deltaSqrSpeed;

            if(timeToClosest > MaxTimeLookAhead) return new MovementOutput(); // time check

            Vector3 futureDeltaPos = deltaPos + deltaVel * timeToClosest;
            float futureDistance = futureDeltaPos.magnitude;

            if (futureDistance > 2 * CollisionRadius) return new MovementOutput(); // check collision radius

            if (futureDistance <= 0 || deltaPos.magnitude < 2 * CollisionRadius)
            {
                //exact or immediate collisions
                this.Output.linear = Character.Position - destinationTarget.Position;
            }
            else
            {
                this.Output.linear = futureDeltaPos * -1;
            }
            this.Output.linear = Output.linear.normalized * MaxAcceleration;
            return this.Output;
        }
    }
}