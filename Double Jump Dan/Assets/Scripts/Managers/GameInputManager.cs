using UnityEngine;

public class GameInputManager : MonoBehaviour
{
    public static GameInputManager Instance;
    Camera _camera;

    void Awake()
    {
        Instance = this;
        _camera = Camera.main;
    }

    public float GetHorizontalInput()
    {
        return Input.GetAxisRaw("Horizontal");
    }

    public float GetVerticalInput()
    {
        return Input.GetAxisRaw("Vertical");
    }

    public bool JumpButtonDown()
    {
        return Input.GetButtonDown("Jump");
    }

    public bool JumpButtonUp()
    {
        return Input.GetButtonUp("Jump");
    }

    public bool StrafeButton()
    {
        return Input.GetButton("Strafe");
    }

    public bool ShootButton()
    {
        return Input.GetButton("Shoot");
    }

    public bool ShootButtonDown()
    {
        return Input.GetButtonDown("Shoot");
    }

    public bool ShootButtonUp()
    {
        return Input.GetButtonUp("Shoot");
    }


    public bool ReloadButtonDown()
    {
        return Input.GetButtonDown("Reload");
    }

    public bool EscapeButtonDown()
    {
        return Input.GetKeyDown(KeyCode.Escape);
    }

    public bool ReturnButtonDown()
    {
        return Input.GetKeyDown(KeyCode.Return);
    }

    public bool PauseButtonDown()
    {
        return Input.GetButtonDown("Pause");
    }

    public bool ScreenshotButtonDown()
    {
        return Input.GetKeyDown(KeyCode.C);
    }

    public Vector3 GetRealMousePosition()
    {
        return _camera.ScreenToWorldPoint(Input.mousePosition);
    }
}
