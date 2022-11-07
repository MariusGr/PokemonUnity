//Original Scripts by IIColour (IIColour_Spectrum)

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class DialogBox : MonoBehaviour, IDialogBox
{
    [SerializeField] Transform dialogBoxT;
    [SerializeField] Transform dialogBoxTrn;

    public string debugBoxString;

    [SerializeField] private Image dialogBox;
    [SerializeField] private Text dialogBoxText;
    [SerializeField] private Text dialogBoxTextShadow;
    [SerializeField] private Image dialogBoxBorder;

    [SerializeField] private Image choiceBox;
    [SerializeField] private Text choiceBoxText;
    [SerializeField] private Text choiceBoxTextShadow;
    [SerializeField] private Image choiceBoxSelect;

    [SerializeField] private AudioClip selectClip;

    private bool automaticContinueBlocked = false;
    InputData input = new InputData();

    private float charPerSec = 60f;
    public float scrollSpeed = 0.1f;

    public int chosenIndex;
    public int GetChosenIndex() => chosenIndex;

    public int defaultChoiceWidth = 86;
    public int defaultChoiceY = 0;
    public int defaultDialogLines = 2;

    private Dictionary<Effectiveness, string> EffectivenessToTextMap = new Dictionary<Effectiveness, string> {
        { Effectiveness.Ineffecitve, "Es hat keinen Effekt." },
        { Effectiveness.Strong, "Es ist sehr effektiv!" },
        { Effectiveness.Weak, "Es ist nicht sehr effektiv..." },
        { Effectiveness.Normal, "" },
    };

    public DialogBox() => Services.Register(this as IDialogBox);

    void Awake()
    {
        defaultDialogLines = Mathf.RoundToInt((dialogBoxBorder.rectTransform.sizeDelta.y - 16f) / 14f);
        defaultChoiceY = Mathf.FloorToInt(choiceBox.rectTransform.localPosition.y);
    }

    public bool ProcessInput(InputData input)
    {
        this.input = input;
        return false;
    }

    public void Continue() => automaticContinueBlocked = false;

    public void Open()
    {
        print("Open Dialogbox");
        gameObject.SetActive(true);
    }

    public void Close()
    {
        print("Close Dialogbox");
        gameObject.SetActive(false);
    }

    public bool IsOpen() => gameObject.activeSelf;

    public Coroutine DrawText(Effectiveness effectiveness, DialogBoxContinueMode continueMode, bool closeAfterFinish = false)
    {
        Open();
        return StartCoroutine(DrawStringsRoutine(new string[] { EffectivenessToTextMap[effectiveness] }, continueMode, closeAfterFinish));
    }

    public Coroutine DrawText(string text, DialogBoxContinueMode continueMode, bool closeAfterFinish = false)
        => DrawText(text.Split('|'), continueMode, closeAfterFinish);
    public Coroutine DrawText(string[] text, DialogBoxContinueMode continueMode, bool closeAfterFinish = false)
    {
        Open();
        return StartCoroutine(DrawStringsRoutine(text, continueMode, closeAfterFinish));
    }

    IEnumerator DrawStringsRoutine(string text, DialogBoxContinueMode continueMode, bool closeAfterFinish = false)
        => DrawStringsRoutine(new string[] { text }, continueMode, closeAfterFinish);

    IEnumerator DrawStringsRoutine(string[] text, DialogBoxContinueMode continueMode, bool closeAfterFinish = false)
    {
        DrawDialogBox();

        if (continueMode == DialogBoxContinueMode.User)
            InputManager.Instance.Register(this);

        for (int i = 0; i < text.Length; i++)
        {
            if (continueMode == DialogBoxContinueMode.External)
                automaticContinueBlocked = true;
            yield return StartCoroutine(DrawTextRoutine(text[i]));
            if (continueMode == DialogBoxContinueMode.User)
            {
                while (!input.submit.pressed && !input.chancel.pressed)
                    yield return null;
            }
            else if (continueMode == DialogBoxContinueMode.Automatic && i < text.Length - 1)
                // wait some time in between lines
                yield return new WaitForSeconds(1.5f);
            else
                yield return new WaitWhile(() => automaticContinueBlocked);
        }

        if (continueMode == DialogBoxContinueMode.User)
            InputManager.Instance.Unregister(this);

        if (closeAfterFinish)
            Close();
    }

    private IEnumerator DrawTextRoutine(string text)
    {
        dialogBoxText.text = "";
        yield return StartCoroutine(DrawTextRoutine(text, 1f / charPerSec, false));
    }

    private IEnumerator DrawTextRoutine(string text, float secPerChar)
    {
        yield return StartCoroutine(DrawTextRoutine(text, secPerChar, false));
    }

    private IEnumerator DrawTextRoutineSilent(string text)
    {
        yield return StartCoroutine(DrawTextRoutine(text, 1f / charPerSec, true));
    }

    private IEnumerator DrawTextRoutineInstant(string text)
    {
        yield return StartCoroutine(DrawTextRoutine(text, 0, false));
    }

    private IEnumerator DrawTextRoutine(string text, float secPerChar, bool silent)
    {
        string[] words = text.Split(new char[] {' '});

        if (!silent)
        {
            SfxHandler.Play(selectClip);
        }
        for (int i = 0; i < words.Length; i++)
        {
            if (secPerChar > 0)
            {
                yield return StartCoroutine(DrawWord(words[i], secPerChar));
            }
            else
            {
                StartCoroutine(DrawWord(words[i], secPerChar));
            }
        }
    }

    private IEnumerator DrawWord(string word, float secPerChar)
    {
        yield return StartCoroutine(DrawWord(word, false, false, false, secPerChar));
    }

    private IEnumerator DrawWord(string word, bool large, bool bold, bool italic, float secPerChar)
    {
        char[] chars = word.ToCharArray();
        float startTime = Time.time;
        if (chars.Length > 0)
        {
            //ensure no blank words get processed

            if (chars[0] == '\\')
            {
                //Apply Operator
                switch (chars[1])
                {
                    case ('p'): //Player
                        if (secPerChar > 0)
                        {
                        }
                        else
                        {
                        }
                        break;
                    case ('l'): //Large
                        large = true;
                        break;
                    case ('b'): //Bold
                        bold = true;
                        break;
                    case ('i'): //Italic
                        italic = true;
                        break;
                    case ('n'): //New Line
                        dialogBoxText.text += "\n";
                        break;
                }
                if (chars.Length > 2)
                {
                    //Run this function for the rest of the word
                    string remainingWord = "";
                    for (int i = 2; i < chars.Length; i++)
                    {
                        remainingWord += chars[i].ToString();
                    }
                    yield return StartCoroutine(DrawWord(remainingWord, large, bold, italic, secPerChar));
                }
            }
            else
            {
                //Draw Word
                string currentText = dialogBoxText.text;

                for (int i = 0; i <= chars.Length; i++)
                {
                    string added = "";

                    //apply open tags
                    added += (large) ? "<size=26>" : "";
                    added += (bold) ? "<b>" : "";
                    added += (italic) ? "<i>" : "";

                    //apply displayed text
                    for (int i2 = 0; i2 < i; i2++)
                    {
                        added += chars[i2].ToString();
                    }

                    //apply hidden text
                    added += "<color=#0000>";
                    for (int i2 = i; i2 < chars.Length; i2++)
                    {
                        added += chars[i2].ToString();
                    }
                    added += "</color>";

                    //apply close tags
                    added += (italic) ? "</i>" : "";
                    added += (bold) ? "</b>" : "";
                    added += (large) ? "</size>" : "";

                    dialogBoxText.text = currentText + added;
                    dialogBoxTextShadow.text = dialogBoxText.text;

                    while (Time.time < startTime + (secPerChar * (i + 1)))
                    {
                        yield return null;
                    }
                }

                //add a space after every word
                dialogBoxText.text += " ";
                dialogBoxTextShadow.text = dialogBoxText.text;
                while (Time.time < startTime + (secPerChar))
                {
                    yield return null;
                }
            }
        }
    }


    public void DrawDialogBox()
    {
        StartCoroutine(DrawDialogBox(defaultDialogLines, new Color(1, 1, 1, 1), false));
    }

    public void DrawDialogBox(int lines)
    {
        StartCoroutine(DrawDialogBox(lines, new Color(1, 1, 1, 1), false));
    }

    public void DrawSignBox(Color tint)
    {
        StartCoroutine(DrawDialogBox(defaultDialogLines, tint, true));
    }

    private IEnumerator DrawDialogBox(int lines, Color tint, bool sign)
    {
        dialogBox.gameObject.SetActive(true);
        //dialogBoxBorder.sprite = (sign)
        //    ? null
        //    : Resources.Load<Sprite>("Frame/dialog" + PlayerPrefs.GetInt("frameStyle"));
        //dialogBox.sprite = (sign) ? Resources.Load<Sprite>("Frame/signBG") : Resources.Load<Sprite>("Frame/dialogBG");
        dialogBox.color = tint;
        dialogBoxText.text = "";
        dialogBoxText.color = (sign) ? new Color(1f, 1f, 1f, 1f) : new Color(0.0625f, 0.0625f, 0.0625f, 1f);
        dialogBoxTextShadow.text = dialogBoxText.text;

        dialogBox.rectTransform.sizeDelta = new Vector2(dialogBox.rectTransform.sizeDelta.x,
            Mathf.Round((float) lines * 14f) + 16f);
        dialogBoxBorder.rectTransform.sizeDelta = new Vector2(dialogBox.rectTransform.sizeDelta.x,
            dialogBox.rectTransform.sizeDelta.y);
        dialogBoxText.rectTransform.localPosition = new Vector3(dialogBoxText.rectTransform.localPosition.x,
            -37f + Mathf.Round((float) lines * 14f), 0);
        dialogBoxTextShadow.rectTransform.localPosition = new Vector3(
            dialogBoxTextShadow.rectTransform.localPosition.x, dialogBoxText.rectTransform.localPosition.y - 1f, 0);

        if (sign)
        {
            float increment = 0f;
            while (increment < 1)
            {
                increment += (1f / 0.2f) * Time.deltaTime;
                if (increment > 1)
                {
                    increment = 1;
                }

                dialogBox.rectTransform.localPosition = new Vector2(dialogBox.rectTransform.localPosition.x,
                    -dialogBox.rectTransform.sizeDelta.y + (dialogBox.rectTransform.sizeDelta.y * increment));
                yield return null;
            }
        }
    }

    public IEnumerator DrawChoiceBox()
    {
        yield return
            StartCoroutine(DrawChoiceBox(new string[] {"Yes", "No"}, null, -1, defaultChoiceY, defaultChoiceWidth));
    }

    public Coroutine DrawChoiceBox(string text, string[] choices, int chancelIndex = -1)
    {
        Open();
        StartCoroutine(DrawStringsRoutine(text, DialogBoxContinueMode.External, true));
        return StartCoroutine(DrawChoiceBox(choices, null, -1, defaultChoiceY, defaultChoiceWidth, chancelIndex));
    }

    public IEnumerator DrawChoiceBox(int startIndex)
    {
        yield return
            StartCoroutine(DrawChoiceBox(new string[] {"Yes", "No"}, null, startIndex, defaultChoiceY,
                defaultChoiceWidth));
    }

    public IEnumerator DrawChoiceBox(string[] choices, int startIndex)
    {
        yield return StartCoroutine(DrawChoiceBox(choices, null, startIndex, defaultChoiceY, defaultChoiceWidth));
    }

    public IEnumerator DrawChoiceBox(string[] choices, string[] flavourText)
    {
        yield return StartCoroutine(DrawChoiceBox(choices, flavourText, -1, defaultChoiceY, defaultChoiceWidth));
    }

    public IEnumerator DrawChoiceBox(string[] choices, string[] flavourText, int startIndex)
    {
        yield return StartCoroutine(DrawChoiceBox(choices, flavourText, startIndex, defaultChoiceY, defaultChoiceWidth))
            ;
    }

    public IEnumerator DrawChoiceBox(string[] choices, int yPosition, int width)
    {
        yield return
            StartCoroutine(DrawChoiceBox(new string[] {"Yes", "No"}, null, -1, defaultChoiceY, defaultChoiceWidth));
    }

    public IEnumerator DrawChoiceBox(string[] choices, int startIndex, int yPosition, int width)
    {
        yield return
            StartCoroutine(DrawChoiceBox(new string[] {"Yes", "No"}, null, startIndex, defaultChoiceY,
                defaultChoiceWidth));
    }

    public IEnumerator DrawChoiceBox(string[] choices, string[] flavourText, int startIndex, int yPosition, int width, int chancelIndex = -1)
    {
        InputManager.Instance.Register(this);

        if (startIndex < 0)
            startIndex = 0;

        choiceBox.gameObject.SetActive(true);
        choiceBox.rectTransform.sizeDelta = new Vector2(width, 16f + (14f * choices.Length));
        choiceBoxSelect.rectTransform.localPosition = new Vector3(8, 9f + (14f * startIndex), 0);
        choiceBoxText.rectTransform.sizeDelta = new Vector2(width - 30, choiceBox.rectTransform.sizeDelta.y);
        choiceBoxTextShadow.rectTransform.sizeDelta = new Vector2(choiceBoxText.rectTransform.sizeDelta.x,
            choiceBoxText.rectTransform.sizeDelta.y);

        choiceBoxText.text = "";
        for (int i = 0; i < choices.Length; i++)
        {
            choiceBoxText.text += choices[i];
            if (i != choices.Length - 1)
            {
                choiceBoxText.text += "\n";
            }
        }
        choiceBoxTextShadow.text = choiceBoxText.text;

        bool selected = false;
        UpdateChosenIndex(startIndex, choices.Length, flavourText);
        while (!selected)
        {
            if (input.submit.pressed)
            {
                selected = true;
            }
            else if (input.chancel.pressed && chancelIndex > -1)
            {
                chosenIndex = chancelIndex;
                UpdateChosenIndex(0, choices.Length, flavourText);
                selected = true;
            }
            else if (input.digitalPad.pressed == Direction.Up)
                UpdateChosenIndex(chosenIndex - 1, choices.Length, flavourText);
            else if (input.digitalPad.pressed == Direction.Down)
                UpdateChosenIndex(chosenIndex + 1, choices.Length, flavourText);
            yield return null;
        }

        InputManager.Instance.Unregister(this);
        choiceBox.gameObject.SetActive(false);
    }

    private bool UpdateChosenIndex(int newIndex, int choicesLength, string[] flavourText)
    {
        //Check for an invalid new index
        if (newIndex < 0 || newIndex >= choicesLength)
        {
            return false;
        }
        //Even if new index is the same as old, set the graphics in case of needing to override modified graphics.
        choiceBoxSelect.rectTransform.localPosition = new Vector3(8, 9f + (14f * (choicesLength - newIndex) - 14), 0);
        if (flavourText != null)
        {
            DrawDialogBox();
            StartCoroutine(DrawTextRoutine(flavourText[flavourText.Length - 1 - newIndex], 0));
        }
        //If chosen index is the same as before, do not play a sound effect, then return false
        if (chosenIndex == newIndex)
        {
            return false;
        }
        chosenIndex = newIndex;
        SfxHandler.Play(selectClip);
        return true;
    }

    public IEnumerator UndrawSignBox()
    {
        float increment = 0f;
        while (increment < 1)
        {
            increment += (1f / 0.2f) * Time.deltaTime;
            if (increment > 1)
            {
                increment = 1;
            }

            dialogBox.rectTransform.localPosition = new Vector2(dialogBox.rectTransform.localPosition.x,
                -dialogBox.rectTransform.sizeDelta.y * increment);
            yield return null;
        }
        dialogBox.gameObject.SetActive(false);
    }

    public void UndrawChoiceBox()
    {
        choiceBox.gameObject.SetActive(false);
    }
}