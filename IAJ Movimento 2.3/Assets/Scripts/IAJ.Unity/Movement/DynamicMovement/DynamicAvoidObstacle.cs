using UnityEngine;
namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement
{
    public class DynamicAvoidObstacle : DynamicSeek
    {
        public DynamicAvoidObstacle(Obstacle obstacle)
        {
            Collider = obstacle.Collider;
        }

        public override string Name
        {
            get { return "AvoidObstacle"; }
        }

        public float AvoidMargin { get; set; }
        public float MaxLookAhead { get; set; }
        Collider Collider { get; set; }


        //use singular avoid obstacle instead of a for loop IMPORTANT OPTIMIZATION
        public override MovementOutput GetMovement()
        {
            RaycastHit hit;
            float RayLength = MaxLookAhead + Character.velocity.magnitude /2;
            float WhiskerRayLength = MaxLookAhead*0.75f;
            float angle = 0.1f; //Whisker angular offset
            float epsilon = 0.01f;
            Vector3 charPos = this.Character.Position;
            Vector3 charOri = this.Character.velocity.normalized;
            Vector3 perpendicular = Util.MathHelper.PerpendicularVector2D(charOri)*0.7f; // Whisker positional offset
            
            //If speed is too low to consider obstacle collisions, return
            if(this.Character.velocity.magnitude < epsilon)
            {
                return new MovementOutput();
            }
            //Main Ray
            Debug.DrawRay(charPos, charOri * RayLength, Color.blue);
            if (Physics.Raycast(charPos, charOri, out hit, RayLength))
            {
                if (hit.collider.gameObject == Collider.gameObject)
                {
                    //Can't choose left or right? Tilt towards a random side. Else use normal.
                    if (hit.normal.normalized == -charOri)
                    {
                        Vector3 randomOffset;
                        if (Random.value < 0.5f)
                            randomOffset = perpendicular;
                        else
                            randomOffset = -perpendicular;
                        base.Target.Position = hit.point + randomOffset * AvoidMargin;
                    }
                    else
                        base.Target.Position = hit.point + hit.normal * AvoidMargin;
                    return base.GetMovement();
                }
            }
            //Left Whisker
            Debug.DrawRay(charPos - perpendicular, Util.MathHelper.Rotate2D(charOri, +angle) * WhiskerRayLength, Color.blue);
            if (Physics.Raycast(charPos - perpendicular, Util.MathHelper.Rotate2D(charOri, +angle), out hit, RayLength))
            {
                if (hit.collider.gameObject == Collider.gameObject)
                {
                    base.Target.Position = hit.point + hit.normal * AvoidMargin;
                    return base.GetMovement();
                }
            }
            //Right Whisker
            Debug.DrawRay(charPos + perpendicular, Util.MathHelper.Rotate2D(charOri, -angle) * WhiskerRayLength, Color.blue);
            if (Physics.Raycast(charPos + perpendicular, Util.MathHelper.Rotate2D(charOri, -angle), out hit, RayLength))
            {
                if (hit.collider.gameObject == Collider.gameObject)
                {
                    base.Target.Position = hit.point + hit.normal * AvoidMargin;
                    return base.GetMovement();
                }
            }
            return new MovementOutput();
        }
    }
}

    