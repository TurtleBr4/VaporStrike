using UnityEngine;

public class PlayerStateController : MonoBehaviour
{
    [SerializeField] private bool isActionable = true;
    [SerializeField] private bool isGrounded = false;
    [SerializeField] private bool isFacingRight = true;
    [SerializeField] private bool isMoving = false;
    [SerializeField] private bool isHitstun = false;
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

    public void setHitstun(bool s)
    {
        isHitstun = s;
    }

    public bool getHitstun()
    {
        return isHitstun;
    }
}
