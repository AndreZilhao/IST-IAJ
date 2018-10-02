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
            Vector3 perpendicular = Util.MathHelper.PerpendicularVector2D(Character.velocity.normalized)*0.7f;

            //Main Ray
            Debug.DrawRay(this.Character.Position, this.Character.velocity.normalized * RayLength, Color.blue);
            if (Physics.Raycast(this.Character.Position, this.Character.velocity.normalized, out hit, RayLength))
            {
                //Sanitizing for the appropriate collision
                if (hit.collider.gameObject == Collider.gameObject)
                {

                    //Can't choose left or right? Tilt towards a random side. Else use normal.
                    Vector3 desiredPosition;
                    if (hit.normal.normalized == -Character.velocity.normalized)
                    {
                        Vector3 randomOffset;
                        if (Random.value < 0.5f)
                        { 
                            randomOffset = new Vector3(0, 0, 1);
                        }
                        else
                        { 
                            randomOffset = new Vector3(1, 0, 0);
                        }          
                        desiredPosition = hit.point + randomOffset * AvoidMargin;
                    } else
                    {
                        desiredPosition = hit.point + hit.normal * AvoidMargin;
                    }
                    
                    //Dynamic Seek Component
                    this.Output.linear = desiredPosition - this.Character.Position;

                    if (this.Output.linear.sqrMagnitude > 0)
                    {
                        this.Output.linear.Normalize();
                        this.Output.linear *= this.MaxAcceleration;
                    }

                    return this.Output;
                }
            }
            //Left Whisker
            Debug.DrawRay(this.Character.Position - perpendicular, Util.MathHelper.Rotate2D(this.Character.velocity.normalized, +angle) * WhiskerRayLength, Color.blue);
            if (Physics.Raycast(this.Character.Position - perpendicular, Util.MathHelper.Rotate2D(this.Character.velocity.normalized, +angle), out hit, RayLength))
            {
                //Sanitizing for the appropriate collision
                if (hit.collider.gameObject == Collider.gameObject)
                {

                    //Can't choose left or right? Tilt towards a random side. Else use normal.
                    Vector3 desiredPosition;
                    if (hit.normal.normalized == -Character.velocity.normalized)
                    {
                        Vector3 randomOffset;
                        if (Random.value < 0.5f)
                        {
                            randomOffset = new Vector3(0, 0, 1);
                        }
                        else
                        {
                            randomOffset = new Vector3(1, 0, 0);
                        }
                        desiredPosition = hit.point + randomOffset * AvoidMargin;
                    }
                    else
                    {
                        desiredPosition = hit.point + hit.normal * AvoidMargin;
                    }

                    //Dynamic Seek Component
                    this.Output.linear = desiredPosition - this.Character.Position;

                    if (this.Output.linear.sqrMagnitude > 0)
                    {
                        this.Output.linear.Normalize();
                        this.Output.linear *= this.MaxAcceleration;
                    }

                    return this.Output;
                }
            }
            //Right Whisker
            Debug.DrawRay(this.Character.Position + perpendicular, Util.MathHelper.Rotate2D(this.Character.velocity.normalized, -angle) * WhiskerRayLength, Color.blue);
            if (Physics.Raycast(this.Character.Position + perpendicular, Util.MathHelper.Rotate2D(this.Character.velocity.normalized, -angle), out hit, RayLength))
            {
                //Sanitizing for the appropriate collision
                if (hit.collider.gameObject == Collider.gameObject)
                {

                    //Can't choose left or right? Tilt towards a random side. Else use normal.
                    Vector3 desiredPosition;
                    if (hit.normal.normalized == -Character.velocity.normalized)
                    {
                        Vector3 randomOffset;
                        if (Random.value < 0.5f)
                        {
                            randomOffset = new Vector3(0, 0, 1);
                        }
                        else
                        {
                            randomOffset = new Vector3(1, 0, 0);
                        }
                        desiredPosition = hit.point + randomOffset * AvoidMargin;
                    }
                    else
                    {
                        desiredPosition = hit.point + hit.normal * AvoidMargin;
                    }

                    //Dynamic Seek Component
                    this.Output.linear = desiredPosition - this.Character.Position;

                    if (this.Output.linear.sqrMagnitude > 0)
                    {
                        this.Output.linear.Normalize();
                        this.Output.linear *= this.MaxAcceleration;
                    }

                    return this.Output;
                }
            }

            return new MovementOutput();
        }
    }
}

    