using UnityEngine;

public class Billboard : MonoBehaviour
{
    public void doBillboarding()
    {
        if (Camera.main != null)
        {
            transform.LookAt(Camera.main.transform);
            transform.Rotate(0, 180, 0); 
        }
    }

    private void LateUpdate()
    {
        doBillboarding();
    }
}
