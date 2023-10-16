//Original Scripts by IIColour (IIColour_Spectrum)

using UnityEngine;
using System.Collections;

public class GrassPatchHandler : MonoBehaviour
{
    [SerializeField] GameObject overlay;

    public AudioClip walkClip;

    private GrassCluster area;

    void Start()
    {
        overlay.SetActive(false);
        area = GrassCluster.ChildToInstanceMap[gameObject];
    }

    void OnTriggerEnter(Collider other)
    {
        overlay.SetActive(true);

        if (other.gameObject != PlayerCharacter.Instance.Collider.gameObject)
            return;

        area.Enter();
        SfxHandler.Play(walkClip, Random.Range(0.85f, 1.1f));
    }

    void OnTriggerExit(Collider other)
    {
        overlay.SetActive(false);

        if (other.gameObject != PlayerCharacter.Instance.Collider.gameObject)
            return;

        area.Leave();
    }
}