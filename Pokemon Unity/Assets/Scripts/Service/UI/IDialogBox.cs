using UnityEngine;
using System.Collections;

public interface IDialogBox : IUIView, IInputConsumer
{
    public int GetChosenIndex();
    public void DrawTextPausing(string text, DialogBoxContinueMode continueMode = DialogBoxContinueMode.User, bool closeAfterFinish = false, int lines = 2);
    public Coroutine DrawText(Effectiveness effectiveness, DialogBoxContinueMode continueMode = DialogBoxContinueMode.User, bool closeAfterFinish = false, int lines = 2);
    public Coroutine DrawText(string text, DialogBoxContinueMode continueMode = DialogBoxContinueMode.User, bool closeAfterFinish = false, int lines = 2);
    public Coroutine DrawText(string[] text, DialogBoxContinueMode continueMode = DialogBoxContinueMode.User, bool closeAfterFinish = false, int lines = 2);
    public IEnumerator DrawChoiceBox(string text, int chancelIndex = 1, int lines = 2);
    public IEnumerator DrawChoiceBox(string text, string[] choices, int chancelIndex = -1, int lines = 2);
    public void Continue();
    public bool IsOpen();
}
