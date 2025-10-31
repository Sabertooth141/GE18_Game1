//作成者 : 植村将太
using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// EnumFlagsをLayerMaskの様にInspectorで扱えるEnumFlagsAttribute
/// </summary>
[AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field)]
public sealed class EnumFlagsAttribute : PropertyAttribute
{
}

#if UNITY_EDITOR
/// <summary>
/// EnumFlagsをLayerMaskの様にInspectorで扱えるEnumFlagsAttributeDrawer
/// </summary>
[CustomPropertyDrawer(typeof(EnumFlagsAttribute))]
public sealed class EnumFlagsAttributeDrawer : PropertyDrawer
{
	/// <summary>[OnGUI]</summary>
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
		//MaskFieldで値を取得
		property.intValue = EditorGUI.MaskField(
            position,
            label,
            property.intValue,
            property.enumNames
        );
    }
}
#endif