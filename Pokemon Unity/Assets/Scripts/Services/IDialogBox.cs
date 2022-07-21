using UnityEngine;

public interface IDialogBox : IService
{
    public Coroutine DrawText(Effectiveness effectiveness, DialogBoxCloseMode closeMode);
    public Coroutine DrawText(string text, DialogBoxCloseMode closeMode);
    public Coroutine DrawText(string[] text, DialogBoxCloseMode closeMode);
    public Coroutine DrawTextPausing(string[] text, DialogBoxCloseMode closeMode);
    public void Open();
    public void Close();
    public bool IsOpen();
}
