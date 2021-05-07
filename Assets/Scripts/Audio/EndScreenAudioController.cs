using UnityEngine;
using Photon.Pun;

public class EndScreenAudioController : MonoBehaviour
{
    private FMOD.Studio.EventInstance winInstance;
    private FMOD.Studio.EventInstance lossInstance;

    void Start() {
        winInstance = FMODUnity.RuntimeManager.CreateInstance("event:/EndScreen/Win");
        lossInstance = FMODUnity.RuntimeManager.CreateInstance("event:/EndScreen/Loss");
    
        bool wasWin = (bool) PhotonNetwork.CurrentRoom.CustomProperties["win"];

        if (wasWin) {
            winInstance.start();
        } else {
            lossInstance.start();
        }
    }

    public void StopAll() {
        winInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        lossInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
}
