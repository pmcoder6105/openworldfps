/// <summary>
/// This script belongs to cowsins� as a part of the cowsins� FPS Engine. All rights reserved. 
/// </summary>
using cowsins;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class WeaponSway : MonoBehaviour
{
    #region shared
    [System.Serializable]
    public enum SwayMethod
    {
        Simple, PivotBased
    }
    public SwayMethod swayMethod;
    public delegate void Sway();

    public Sway sway;
    #endregion
    #region simple
    [Header("Position")]
    [SerializeField] private float amount = 0.02f;

    [SerializeField] private float maxAmount = 0.06f;

    [SerializeField] private float smoothAmount = 6f;


    [Header("Tilting")]
    [SerializeField] private float tiltAmount = 4f;

    [SerializeField] private float maxTiltAmount = 5f;

    [SerializeField] private float smoothTiltAmount = 12f;

    private WeaponController player;

    private Vector3 initialPosition;

    private Quaternion initialRotation;

    private float InputX;

    private float InputY;

    private float playerMultiplier;
    #endregion
    #region pivotBased
    [SerializeField] private Transform pivot;

    [SerializeField] private float swaySpeed;

    [SerializeField] private Vector2 swayMovementAmount;

    [SerializeField] private Vector2 swayRotationAmount;

    [SerializeField] private float swayTiltAmount;

    private Rigidbody rb;
    #endregion

    private void Start()
    {
        if (swayMethod == SwayMethod.Simple)
        {
            initialPosition = transform.localPosition;
            initialRotation = transform.localRotation;
            player = GameObject.Find("Player").GetComponent<WeaponController>();
            sway = SimpleSway;
        }
        else
        {
            rb = PlayerStates.instance.GetComponent<Rigidbody>();
            sway = PivotSway;
        }
    }

    private void Update()
    {
        if (!PlayerStats.Controllable) return;
        sway?.Invoke();
    }
    private void SimpleSway()
    {
        CalculateSway();
        MoveSway();
        TiltSway();
    }
    private void CalculateSway()
    {
        InputX = -InputManager.mousex / 10 - 2 * InputManager.controllerx;
        InputY = -InputManager.mousey / 10 - 2 * InputManager.controllery;

        if (player.isAiming) playerMultiplier = 5f;
        else playerMultiplier = 1f;
    }

    private void MoveSway()
    {

        float moveX = Mathf.Clamp(InputX * amount, -maxAmount, maxAmount) / playerMultiplier;
        float moveY = Mathf.Clamp(InputY * amount, -1, 1) / playerMultiplier;

        Vector3 finalPosition = new Vector3(moveX, moveY, 0);

        transform.localPosition = Vector3.Lerp(transform.localPosition, finalPosition + initialPosition, Time.fixedDeltaTime * smoothAmount * playerMultiplier);

    }

    private void TiltSway()
    {
        float moveX = Mathf.Clamp(InputX * tiltAmount, -maxTiltAmount, maxTiltAmount) / playerMultiplier;

        Quaternion finalRotation = Quaternion.Euler(0, 0, moveX);

        transform.localRotation = Quaternion.Lerp(transform.localRotation, finalRotation * initialRotation, Time.fixedDeltaTime * smoothTiltAmount * playerMultiplier);
    }

    private void PivotSway()
    {
        HandleSwayLocation();
        HandleSwayRotation();
    }
    private void HandleSwayRotation()
    {
        var right = Camera.main.transform.right;
        right.y = 0f;
        right.Normalize();

        // HANDLE HORIZONTAL ROTATION
        transform.RotateAround(pivot.position, new Vector3(0, 1, 0), Time.fixedDeltaTime * swayRotationAmount.x * -InputManager.mousex);
        // HANDLE VERTICAL ROTATION
        transform.RotateAround(pivot.position, right, Time.fixedDeltaTime * swayRotationAmount.y * InputManager.mousey);
        // HANDLE TILT ROTATION
        Quaternion swayRot = Quaternion.Lerp(transform.localRotation,
            Quaternion.Euler(new Vector3(transform.localRotation.eulerAngles.x, transform.localRotation.eulerAngles.y, InputManager.mousex * swayTiltAmount)),
            Time.deltaTime * swaySpeed);

        swayRot = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(Vector3.zero), Time.deltaTime * swaySpeed);

        transform.localRotation = swayRot;
    }

    private void HandleSwayLocation()
    {
        Vector3 finalPosition = new Vector3(-InputManager.mousex, InputManager.mousey,0) / 100;
        finalPosition.x = Mathf.Clamp(finalPosition.x, -1, 1) * swayMovementAmount.x;
        finalPosition.y = Mathf.Clamp(finalPosition.y, -1, 1) * swayMovementAmount.y;

        transform.localPosition = Vector3.Lerp(transform.localPosition, finalPosition, swaySpeed * Time.deltaTime);
    }

}
#if UNITY_EDITOR
[CustomEditor(typeof(WeaponSway))]
public class WeaponSwayEditor : Editor
{
	override public void OnInspectorGUI()
	{
		serializedObject.Update();
		var myScript = target as WeaponSway;

		EditorGUILayout.LabelField("WEAPON SWAY", EditorStyles.boldLabel);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("swayMethod"));
		EditorGUILayout.Space(10f);  

		if (myScript.swayMethod == WeaponSway.SwayMethod.Simple)
		{
			EditorGUI.indentLevel++;
			EditorGUILayout.LabelField("POSITION");
			EditorGUILayout.PropertyField(serializedObject.FindProperty("amount"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("maxAmount"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("smoothAmount"));
			EditorGUILayout.LabelField("ROTATION");
			EditorGUILayout.PropertyField(serializedObject.FindProperty("tiltAmount"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("maxTiltAmount"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("smoothTiltAmount"));
			EditorGUI.indentLevel--;
		}
		else
		{
			EditorGUI.indentLevel++;
			EditorGUILayout.PropertyField(serializedObject.FindProperty("pivot"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("swaySpeed"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("swayMovementAmount"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("swayRotationAmount"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("swayTiltAmount"));
			EditorGUI.indentLevel--;

		}
		EditorGUILayout.Space(5f);


		serializedObject.ApplyModifiedProperties();

	}
}
#endif
