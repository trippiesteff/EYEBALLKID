using UnityEngine;
using FMODUnity;

[CreateAssetMenu(fileName = "FMODEvents", menuName = "Audio/FMOD Events")]
public class FMODEvents : ScriptableObject
{
    [Header("Player")]
    public EventReference playerJump;

    [Header("UI")]
    public EventReference uiClick;

    [Header("Ambience")]
    public EventReference backgroundLoop;

    [Header("Music")]
    public EventReference mainMusic;
}
