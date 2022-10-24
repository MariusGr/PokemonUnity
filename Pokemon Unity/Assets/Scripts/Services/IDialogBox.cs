using UnityEngine;

public interface IDialogBox : IService
{
    public Coroutine DrawText(Effectiveness effectiveness, DialogBoxContinueMode continueMode, bool closeAfterFinish = false);
    public Coroutine DrawText(string text, DialogBoxContinueMode continueMode, bool closeAfterFinish = false);
    public Coroutine DrawText(string[] text, DialogBoxContinueMode continueMode, bool closeAfterFinish = false);
    public Coroutine DrawTextPausing(string[] text, DialogBoxContinueMode continueMode, bool closeAfterFinish = false);
    public void Continue();
    public void Open();
    public void Close();
    public bool IsOpen();
}
