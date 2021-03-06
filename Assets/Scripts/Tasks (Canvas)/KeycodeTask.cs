using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Unity.IO;

public class KeycodeTask : MonoBehaviour
{
    public Text _inputCode;
    public int codeLength = 5;
    public float codeReset = 0.5f;  // code reset time in seconds
    private SoundController soundController;
    private GameController gameController;

    private AudioController audioController;

    [SerializeField] GameObject keyCodeCanvas;
    private bool isReset = false;
    public bool codeCorrect = false; // This code should be attached to the keycode task object in the canvas
    public AudioSource buttonNoise;

    public int keypadID = 0;

    void Awake() {
        soundController = GameObject.FindObjectOfType<SoundController>();
        gameController = GameObject.FindObjectOfType<GameController>();
        audioController = GameObject.FindObjectOfType<AudioController>();
    }
    private void OnEnable()
    {       // when the UI is active, do the following
        codeCorrect = false;
        _inputCode.text = string.Empty;
    }


    public void ButtonClick(int num){
        /*if(isReset){  -----> can be removed
            return;
        }*/

        _inputCode.text += num;             // set inputcode box to what ever buttom input it is

        if (audioController != null) {
            audioController.PlayKeypadPress();
        }
        KeyPad myKeyPad = null;

        KeyPad[] keypads = GameObject.FindObjectsOfType<KeyPad>();
        foreach (KeyPad keypad in keypads)
        {
            if (keypad.id == keypadID) {
                myKeyPad = keypad;
            }
        }
        soundController.sendGrid(myKeyPad.transform.position, Random.Range(50,70));
        if (_inputCode.text.Length == codeLength) // check to see if input length is equal to expected length
        {
            // if code is correct
            if (_inputCode.text == myKeyPad.code || myKeyPad.id == keypadID &&_inputCode.text == "37911") // TODO remove this in a real version
            {
                // Debug.Log("code submitted: " + _inputCode.text);
                _inputCode.text = "Correct";

                audioController.PlayKeypadSuccess();

                if (gameController.gameState <= 2) {
                    gameController.gameState = 2;
                }
                myKeyPad.codeCorrect = true;
                codeCorrect = true;
                // StartCoroutine(ResetCode());
                StartCoroutine(correct(myKeyPad));
            }

            // else if entered code is incorrect
            else if (myKeyPad.id == keypadID && _inputCode.text != myKeyPad.code){
                _inputCode.text = "Failed";
                audioController.PlayKeypadFailure();
                StartCoroutine(ResetCode());
            }
        }
        else if (_inputCode.text.Length >= codeLength)
        {
            _inputCode.text = "Failed";
            StartCoroutine(ResetCode());
        }
    }

    private IEnumerator correct(KeyPad keypad) {
        yield return new WaitForSeconds(.5f);
        keyCodeCanvas.SetActive(false);
        keypad.GetComponent<PhotonView>().RPC("updateKeyCode", RpcTarget.Others, keypadID);
        //insert bool value to say successful if code was correct

    }


    private IEnumerator ResetCode(){  // reset the text inputs
        isReset = true;

        yield return new WaitForSeconds(codeReset);

        _inputCode.text = string.Empty;

        isReset = false;

    }


}
