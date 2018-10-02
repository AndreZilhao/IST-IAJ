using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement
{
    public abstract class DynamicMovement
    {
        protected MovementOutput Output { get; set; }

        public abstract string Name { get; }

        public KinematicData Character { get; set; }
        public KinematicData Target { get; set; }

        public float MaxAcceleration { get; set; }

        public DynamicMovement(){}

        public abstract MovementOutput GetMovement();

    }
}
