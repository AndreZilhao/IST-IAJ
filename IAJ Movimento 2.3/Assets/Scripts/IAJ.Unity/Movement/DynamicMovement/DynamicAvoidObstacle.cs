using UnityEngine;
namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement
{
    public class DynamicAvoidObstacle : DynamicSeek
    {
        public DynamicAvoidObstacle(Obstacle obstacle)
        {
            Collider = obstacle.Collider;
        }

        public float AvoidMargin { get; set; }
        public float MaxLookAhead { get; set; }
        Collider Collider { get; set; }



        public override MovementOutput GetMovement()
        {

            RaycastHit hit;
            float RayLength = MaxLookAhead + Character.velocity.magnitude /2;
            float WhiskerRayLength = MaxLookAhead*0.75f;
            float angle = 0.1f;//Util.MathConstants.MATH_PI_4;
            bool isHit = false;
            Vector3 perpendicular = Util.MathHelper.PerpendicularVector2D(Character.velocity.normalized)*0.7f;
            Vector3 desiredPosition = new Vector3();
            Vector3 charPos = this.Character.Position;
            Vector3 charOri = this.Character.velocity.normalized;

            //Left Whisker
            Debug.DrawRay(charPos - perpendicular, Util.MathHelper.Rotate2D(charOri, +angle) * WhiskerRayLength, Color.blue);
            if (Physics.Raycast(this.Character.Position - perpendicular, Util.MathHelper.Rotate2D(this.Character.velocity.normalized, +angle), out hit, RayLength))
            {
                if (hit.collider.gameObject == Collider.gameObject)
                {
                    desiredPosition = hit.point + hit.normal * AvoidMargin;
                    isHit = true;
                }
            }
            //Right Whisker
            Debug.DrawRay(charPos + perpendicular, Util.MathHelper.Rotate2D(charOri, -angle) * WhiskerRayLength, Color.blue);
            if (Physics.Raycast(charPos + perpendicular, Util.MathHelper.Rotate2D(charOri, -angle), out hit, RayLength))
            {
                if (hit.collider.gameObject == Collider.gameObject)
                {
                    desiredPosition = hit.point + hit.normal * AvoidMargin;
                    isHit = true;
                }
            }
            //Main Ray
            Debug.DrawRay(charPos, charOri * RayLength, Color.blue);
            if (Physics.Raycast(charPos, charOri, out hit, RayLength))
            { 
                if (hit.collider.gameObject == Collider.gameObject)
                {
                    //Can't choose left or right? Tilt towards a random side. Else use normal.
                    isHit = true;
                    if (hit.normal.normalized == -charOri)
                    {
                        Vector3 randomOffset;
                        if (Random.value < 0.5f)
                            randomOffset = new Vector3(0, 0, 1);
                        else
                            randomOffset = new Vector3(1, 0, 0);
                        desiredPosition = hit.point + randomOffset * AvoidMargin;
                    }
                    else
                        desiredPosition = hit.point + hit.normal * AvoidMargin;
                }
            }
            if (isHit)
            {
                this.Output.linear = desiredPosition - charPos;
                if (this.Output.linear.sqrMagnitude > 0)
                {
                    this.Output.linear.Normalize();
                    this.Output.linear *= this.MaxAcceleration;
                }
                return this.Output;
            }
            return new MovementOutput();
        }
    }
}

    