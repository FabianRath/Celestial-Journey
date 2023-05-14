using UnityEngine;

public class CameraAnimationPlayer : MonoBehaviour
{
    public Animation cameraAnimation;

    private float speedMultiplier = 0.1f;

    public void PlayAnimation()
    {
        cameraAnimation["cameraRotation"].speed = speedMultiplier;
        cameraAnimation.Play("cameraRotation");
    }
}
