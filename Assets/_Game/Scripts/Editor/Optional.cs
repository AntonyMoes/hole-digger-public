namespace _Game.Scripts.Editor {
    // [CustomPropertyDrawer(typeof(Optional<>))]
    // public class OptionalPropertyDrawer : PropertyDrawer {
    //     public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
    //         var enumerator = property.GetEnumerator();
    //         enumerator.MoveNext();
    //         var toggleProperty = (SerializedProperty) enumerator.Current;
    //         
    //         // EditorGUI.BeginProperty(position, label, property);
    //         var togglePosition = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
    //         toggleProperty!.boolValue  = EditorGUI.Toggle(togglePosition, toggleProperty.boolValue);
    //         // EditorGUI.EndProperty();
    //
    //         const float toggleWidth = 30;
    //         var toggleWidthVector = toggleWidth * Vector2.right;
    //         var labelPosition = new Rect(togglePosition.position + toggleWidthVector, togglePosition.size - toggleWidthVector);
    //         // value = EditorGUI.IntField(rect, value);
    //         // rect = new Rect(rect.x + labelSize, rect.y, fieldSize, rect.height);
    //         EditorGUI.LabelField(labelPosition, "AAAAA");
    //         
    //         const float labelWidth = 100;
    //         var labelWidthVector = labelWidth * Vector2.right;
    //         var propertyPosition = new Rect(labelPosition.position + labelWidthVector, togglePosition.size - labelWidthVector);
    //
    //         enumerator.MoveNext();
    //         var valueProperty = (SerializedProperty) enumerator.Current;
    //             // valueProperty.
    //         EditorGUI.PropertyField(propertyPosition, valueProperty, GUIContent.none, true);
    //         
    //         // if (PropertyDrawerProxy.TryFind(valueProperty, out var drawer)) {
    //         //     drawer.OnGUI(propertyPosition, valueProperty, GUIContent.none);
    //         // }
    //         
    //         var a = 2;
    //     }
    // }
}