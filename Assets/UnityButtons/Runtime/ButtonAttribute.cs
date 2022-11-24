using System;
namespace Runtime
{
    
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class ButtonAttribute : Attribute
    {
        public readonly string Name;
        public readonly ButtonMode ButtonMode;

        public ButtonAttribute(){}

        public ButtonAttribute(string name) => Name = name;
        public ButtonAttribute(ButtonMode buttonMode) => ButtonMode = buttonMode;
        public ButtonAttribute(string name,ButtonMode buttonMode)
        {
            ButtonMode = buttonMode;
            Name = name;
        }
    }
    public enum ButtonMode
    {
        AlwaysEnabled,
        EnabledInPlayMode,
        DisabledInPlayMode
    }
}