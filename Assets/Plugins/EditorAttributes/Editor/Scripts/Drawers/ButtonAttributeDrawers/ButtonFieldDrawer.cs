using UnityEditor;
using UnityEngine.UIElements;
using EditorAttributes.Editor.Utility;

namespace EditorAttributes.Editor
{
	[CustomPropertyDrawer(typeof(ButtonFieldAttribute))]
    public class ButtonFieldDrawer : PropertyDrawerBase
    {
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var buttonFieldAttribute = attribute as ButtonFieldAttribute;

			var path = property.propertyPath.Split('.');
			object ownerObject = null;

			if (path.Length == 1)
			{
				ownerObject = property.serializedObject.targetObject;
			}
			else
			{
				// Get the object that the property is a member of
				var type = ReflectionUtility.GetNestedObjectType(property, out ownerObject);

				if (type == null)
					return new HelpBox("Field must be a member of a class", HelpBoxMessageType.Error);
			}

			var function = ReflectionUtility.FindFunction(buttonFieldAttribute.FunctionName, ownerObject);
			var functionParameters = function.GetParameters();

			var root = new VisualElement();

			if (functionParameters.Length == 0)
			{
				var button = new Button(() => function.Invoke(ownerObject, null)) { text = string.IsNullOrWhiteSpace(buttonFieldAttribute.ButtonLabel) ? function.Name : buttonFieldAttribute.ButtonLabel };

				button.style.height = buttonFieldAttribute.ButtonHeight;

				root.Add(button);
			}
			else
			{
				root.Add(new HelpBox("Function cannot have parameters", HelpBoxMessageType.Error));
			}

			return root;
		}
	}
}
