using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class KeycodeTask : MonoBehaviour
{
    public Text _cardCode;

    public Text _inputCode;
    public int codeLength = 5;
    public float codeReset = 0.5f;  // code reset time in seconds
    private bool isReset = false;
    public bool codeCorrect = false;
    private void OnEnable() {       // when the UI is active, do the following
        string code = string.Empty;

        codeCorrect = false;

        for(int i = 0; i < codeLength; i++){            // create a random code
            code += Random.Range(1,10);
        }

        _cardCode.text = code;                          // input text
        _inputCode.text = string.Empty;   
    }


    public void ButtonClick(int num){
        /*if(isReset){  -----> can be removed
            return;
        }*/

        _inputCode.text += num;             // set inputcode box to what ever buttom input it is

        if(_inputCode.text == _cardCode.text){
            _inputCode.text = "Correct";
            //insert bool value to say successful if code was correct
            StartCoroutine(ResetCode());
            codeCorrect=true;
        }

        else if(_inputCode.text.Length >= codeLength){
            _inputCode.text = "Failed";
            StartCoroutine(ResetCode());
        }
    }

    private IEnumerator ResetCode(){  // reset the text inputs
        isReset = true;

        yield return new WaitForSeconds(codeReset);

        _inputCode.text = string.Empty;

        isReset = false;

    }


}
