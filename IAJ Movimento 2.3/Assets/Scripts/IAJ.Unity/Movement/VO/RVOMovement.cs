//class adapted from the HRVO library http://gamma.cs.unc.edu/HRVO/
//adapted to IAJ classes by João Dias

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.IAJ.Unity.Movement.DynamicMovement;
using Assets.Scripts.IAJ.Unity.Util;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Movement.VO {
    public class RVOMovement : DynamicVelocityMatch {
        public override string Name {
            get { return "RVO"; }
        }

        protected List<KinematicData> Characters { get; set; }
        protected List<StaticData> Obstacles { get; set; }
        public float CharacterSize { get; set; }
        public float IgnoreDistance { get; set; }
        public float MaxSpeed { get; set; }
        //create additional properties if necessary

        protected DynamicMovement.DynamicMovement DesiredMovement { get; set; }

		public MovementOutput DesiredOutput { get; set; }
		public Vector3 DesiredVelocity { get; set; }
		public MovementOutput DesiredMovementOutput { get; set; }

		public List<Vector3> samples;
		public int numSamples;

		private float angle;
		private float magnitude;
		private Vector3 velocitySample;
		private Vector3 bestSample;
		private float minimumPenalty, maximumTimePenalty;
		private float distancePenalty;
		private Vector3 deltaP;
		private Vector3 rayVector;
		private float tc, timePenalty, W, O;
		private float penalty;

        public RVOMovement(DynamicMovement.DynamicMovement goalMovement, List<KinematicData> movingCharacters, List<StaticData> obstacles) {
            this.DesiredMovement = goalMovement;
            this.Characters = movingCharacters;
            this.Obstacles = obstacles;
            base.Target = new KinematicData();

            //initialize other properties if you think is relevant
			samples = new List<Vector3>();

			numSamples = 5;
			W = 50f;
            O = 50f;
        }

        public override MovementOutput GetMovement() {
			DesiredOutput = DesiredMovement.GetMovement ();

			DesiredVelocity = this.Character.velocity + DesiredOutput.linear;

			if (DesiredVelocity.magnitude > MaxSpeed) {
				DesiredVelocity.Normalize ();
				DesiredVelocity *= MaxSpeed;
			}
			samples.Add (DesiredVelocity);

			for (int i = 0; i < numSamples; i++) {
				angle = Random.Range (0, MathConstants.MATH_2PI);
				magnitude = Random.Range (0, MaxSpeed);
				velocitySample = MathHelper.ConvertOrientationToVector (angle) * magnitude;
				samples.Add (velocitySample);
			}
            base.Target.velocity = GetBestSample (samples, DesiredVelocity);	
			return base.GetMovement();
        }

		public Vector3 GetBestSample(List<Vector3> Samples, Vector3 desiredVelocity){
			bestSample = Vector3.zero;
			minimumPenalty = Mathf.Infinity;

			foreach (var sample in Samples) {
                //OPTIMIZATION -> If the distance penalty by itself is worse than our current minimum penalty, no point in checking any further.
                distancePenalty = (desiredVelocity - sample).magnitude;
                if (distancePenalty > minimumPenalty)
                    continue;
                //END OPTIMIZATION
				maximumTimePenalty = 0.0f;

                //MOBILE OBSTACLE DETECTION
                foreach (var b in Characters) {
					deltaP = b.Position - this.Character.Position;
					if (deltaP.magnitude > IgnoreDistance)
						continue;

					rayVector = 2.0f * sample - this.Character.velocity - b.velocity;
					tc = MathHelper.TimeToCollisionBetweenRayAndCircle (this.Character.Position, rayVector, b.Position, CharacterSize*2.0f);

					if (tc > 0.0f)
						timePenalty = W / tc;
					else if (tc == 0.0f)
						timePenalty = Mathf.Infinity;
					else
						timePenalty = 0.0f;

					if (timePenalty > maximumTimePenalty)
						maximumTimePenalty = timePenalty;
				}

                //STATIC OBSTACLE DETECTION
                foreach (var b in Obstacles)
                { 
                    deltaP = b.Position - this.Character.Position;
                    if (deltaP.magnitude > 5)
                        continue;

                    rayVector = 2.0f * sample - this.Character.velocity;
                    tc = MathHelper.TimeToCollisionBetweenRayAndCircle(this.Character.Position, rayVector, b.Position, CharacterSize * 1.5f);
                    if (tc > 0.0f)
                        timePenalty = O / tc;
                    else if (tc == 0.0f)
                        timePenalty = Mathf.Infinity;
                    else
                        timePenalty = 0.0f;

                    if (timePenalty > maximumTimePenalty)
                        maximumTimePenalty = timePenalty;
                }
                penalty = distancePenalty + maximumTimePenalty;

				if (penalty < minimumPenalty) {
					minimumPenalty = penalty;
					bestSample = sample;
				}
			}
			return bestSample;
		}
    }
}