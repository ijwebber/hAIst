using UnityEngine;
using UnityEngine.UI;
using TMPro;


using System.Collections;



    public class PlayerUI : MonoBehaviour
    {

        private GameController target;


        [Tooltip("UI Text to display Player's Name")]
        [SerializeField]
        private TMP_Text playerNameText;


        void Update() 
        {
            if (target == null)
            {
                Destroy(this.gameObject);
                return;
            }
        }

        public void SetTarget(GameController _target)
        {
            if (_target == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> PlayMakerManager target for PlayerUI.SetTarget.", this);
                return;
            }
            // Cache references for efficiency
            target = _target;
            if (playerNameText != null)
            {
                playerNameText.text = target.photonView.Owner.NickName;
            }
        }

    }
