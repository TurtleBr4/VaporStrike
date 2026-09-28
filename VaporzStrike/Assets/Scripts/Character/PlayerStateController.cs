using UnityEngine;

public class PlayerStateController : MonoBehaviour
{
    [SerializeField] private bool isActionable;
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool isFacingRight;
    [SerializeField] private bool isMoving;
    public bool justJumped = false;
    public bool justLanded = false;

    //Deliberately avoided using the c# get set commands so i can explicitly trace all references using the visual studio reference tracker thing

    public bool getActionablility()
    {
        return isActionable;
    }

    public void setActionability(bool s)
    {
        isActionable = s;
    }

    public bool getGrounded()
    {
        return isGrounded;
    }

    public void setGrounded(bool s)
    {
        isGrounded = s;
    }

    public bool getFacingRight()
    {
        return isFacingRight;
    }

    public void setFacingRight(bool s)
    {
        isFacingRight = s;
    }

    public bool getMoving()
    {
        return isMoving;
    }

    public void setMovin(bool s)
    {
        isMoving = s;
    }
}
