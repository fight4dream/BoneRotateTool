using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

[EditorTool("Bone Rotate Tool")]
public class BoneRotateTool : EditorTool
{
    private GUIContent m_IconContent;

    private void OnEnable()
    {
        m_IconContent = EditorGUIUtility.IconContent("JointAngularLimits", "Bone Rotate Tool");
    }

    public override GUIContent toolbarIcon
    {
        get { return m_IconContent; }
    }

    public override bool IsAvailable()
    {
        return base.IsAvailable() && Tools.pivotMode == PivotMode.Pivot && Tools.pivotRotation == PivotRotation.Local;
    }

    public override void OnToolGUI(EditorWindow window)
    {
        if (Selection.activeTransform == null)
        {
            return;
        }
        if (!IsAvailable())
        {
            Handles.Label(Tools.handlePosition, "Only available in Pivot Local");
            return;
        }

        EditorGUI.BeginChangeCheck();
        Quaternion to = Handles.RotationHandle(Tools.handleRotation, Tools.handlePosition);

        if (EditorGUI.EndChangeCheck())
        {
            Quaternion from = Selection.activeTransform.rotation;
            Quaternion q = Quaternion.Inverse(from) * to;
            q.ToAngleAxis(out float angle, out Vector3 axis);

            Transform[] transforms = Selection.GetTransforms(SelectionMode.Editable | SelectionMode.ExcludePrefab);
            Undo.RecordObjects(transforms, "Bone Rotate");
            foreach (var transform in transforms)
            {
                transform.Rotate(axis, angle, Space.Self);
            }
        }
    }
}
