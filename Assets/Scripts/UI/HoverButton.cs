using UnityEngine;
using WS20.P3.Overcrowded;

public class HoverButton : MonoBehaviour
{

    [SerializeField]
    private GameObject hoverImage;
    [SerializeField]
    private GameObject myRipPoster;

    public void HoverImage()
    {
        hoverImage.SetActive(true);
    }

    public void StopHoverImage()
    {
        hoverImage.SetActive(false);
    }

    public void ClickButton()
    {
        myRipPoster.GetComponent<RipPosters>().AddPaper();
        this.gameObject.SetActive(false);
    }
}
