using UnityEngine;

public interface IDialogBox : IService, IInputConsumer
{
    public int GetChosenIndex();
    public Coroutine DrawText(Effectiveness effectiveness, DialogBoxContinueMode continueMode, bool closeAfterFinish = false);
    public Coroutine DrawText(string text, DialogBoxContinueMode continueMode, bool closeAfterFinish = false);
    public Coroutine DrawText(string[] text, DialogBoxContinueMode continueMode, bool closeAfterFinish = false);
    public Coroutine DrawChoiceBox(string text, string[] choices, int chancelIndex = -1);
    public void Continue();
    public void Open();
    public void Close();
    public bool IsOpen();
}
