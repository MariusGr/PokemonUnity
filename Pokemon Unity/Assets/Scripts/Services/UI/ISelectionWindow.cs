using System;

public interface ISelectionWindow : IUIView
{
    public void Open(Action<ISelectableUIElement, bool> callback);
    public void Open(Action<ISelectableUIElement, bool> callback, bool forceSelection);
    public void Open(Action<ISelectableUIElement, bool> callback, int startSelection);
    public void Open(Action<ISelectableUIElement, bool> callback, bool forceSelection, int startSelection);
}
