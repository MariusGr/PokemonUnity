using UnityEngine;
using System.Collections;

public interface IDialogBox : IUIView, IInputConsumer
{
    public int GetChosenIndex();
    public Coroutine DrawText(Effectiveness effectiveness, DialogBoxContinueMode continueMode, bool closeAfterFinish = false);
    public Coroutine DrawText(string text, DialogBoxContinueMode continueMode, bool closeAfterFinish = false);
    public Coroutine DrawText(string[] text, DialogBoxContinueMode continueMode, bool closeAfterFinish = false);
    public IEnumerator DrawChoiceBox(string text, string[] choices, int chancelIndex = -1);
    public void Continue();
    public bool IsOpen();
}
