using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFadeBlack : IService
{
    public IEnumerator FadeToBlack();
    public IEnumerator FadeFromBlack();
}
