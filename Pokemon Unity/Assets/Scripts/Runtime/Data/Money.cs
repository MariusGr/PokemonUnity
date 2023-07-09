using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Money
{
    public static string FormatMoneyToString(float money) => $"{money.ToString("0.00")}€";
}
