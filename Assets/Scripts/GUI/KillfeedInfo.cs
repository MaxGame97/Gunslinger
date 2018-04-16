using UnityEngine.UI;
using UnityEngine;

public class KillfeedInfo : MonoBehaviour {

    [SerializeField] private Text playerKiller;     // Reference to the display text of the killer
    [SerializeField] private Text playerDied;       // Reference to the display text of the player who died
    [SerializeField] private Image typeIcon;        // Reference to the image displayed as kill icon
    [SerializeField] private Sprite killIcon;       // Reference to the sprite displaying "regular" kill icon
    [SerializeField] private Sprite headshotIcon;   // Reference to the sprite displaying "headshot" kill icon

    public void Setup(string _killer, string _died, bool _headshot)
    {
        playerKiller.text = _killer;            // Set killer name
        playerDied.text = _died;                // Set death name

        if (_headshot)                          // Determine what icon to show
            typeIcon.sprite = headshotIcon;     // Set kill icon as "headshot" icon
        else
            typeIcon.sprite = killIcon;         // Set kill icon as "regular" icon
    }
}
