using UnityEngine;

public class PlayerStateController : MonoBehaviour
{
    [SerializeField] private bool isActionable = true;
    [SerializeField] private bool isGrounded = false;
    [SerializeField] private bool isFacingRight = true;
    [SerializeField] private bool isMoving = false;
    [SerializeField] private bool isHitstun = false;
    [SerializeField] private bool isParrying = false;
    [SerializeField] private bool isGrabbing = false;
    [SerializeField] private bool isGettingGrabbed = false;
    [SerializeField] private bool isInvincible = false;
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

    public bool getInvincible()
    {
        return isInvincible;
    }

    public void setInvincible(bool s)
    {
        isInvincible = s;
    }

    public bool getParrying()
    {
        return isParrying;
    }

    public void setParrying(bool s)
    {
        isParrying = s;
    }

    public bool getGrabbing()
    {
        return isGrabbing;
    }

    public void setGrabbing(bool s)
    {
        isGrabbing = s;
    }

    public bool getGettingGrabbed()
    {
        return isGettingGrabbed;
    }

    public void setGettingGrabbed(bool s)
    {
        isGettingGrabbed = s;
    }
}
