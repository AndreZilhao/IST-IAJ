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
            Debug.DrawRay(this.Character.Position, this.Character.velocity.normalized * MaxLookAhead, Color.blue);
            if (Physics.Raycast(this.Character.Position, this.Character.velocity.normalized, out hit, MaxLookAhead))
            {
                //Sanitizing for the appropriate collision
              
                if (hit.collider.gameObject == Collider.gameObject)
                {
                    Vector3 desiredPosition;                    
                    
                    //Can't choose left or right? Tilt towards a random side.
                    if(hit.normal.normalized == -Character.velocity.normalized)
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
                    
                    //Dynamic Seek
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

    