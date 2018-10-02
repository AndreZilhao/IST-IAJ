using Assets.Scripts.IAJ.Unity.Util;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement
{

    public class DynamicWander : DynamicSeek
    {
        public float WanderOffset { get; set; }
        public float WanderRadius { get; set; }
        public float WanderRate { get; set; }
        public float WanderAngle { get; protected set; }
        public Vector3 CircleCenter { get; private set; }

        public GameObject DebugTarget { get; set; }

        public DynamicWander()
        {
            this.Target = new KinematicData();
            this.WanderAngle = 0f;
        }

        public override string Name
        {
            get { return "Wander"; }
        }


        public override MovementOutput GetMovement()
        {
            //TODO implement Dynamic Wander movement
            Debug.Log("WanderRate: " + WanderRate);
            WanderAngle += RandomHelper.RandomBinomial() * WanderRate;
            float TargetOrientation = WanderAngle + this.Character.Orientation;

            CircleCenter = this.Character.Position + WanderOffset * MathHelper.ConvertOrientationToVector(Character.Orientation);
            base.Target.Position = CircleCenter + WanderRadius * MathHelper.ConvertOrientationToVector(TargetOrientation);

            if(this.DebugTarget != null)
            {
                this.DebugTarget.transform.position = this.Target.Position;
                
            }
            return base.GetMovement();
        }
    }
}
