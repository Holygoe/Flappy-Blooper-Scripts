using UnityEngine;

namespace FlappyBlooper
{
    public class PopupWindow : MonoBehaviour
    {
#pragma warning disable CS0649
        [SerializeField] private bool lowpassMusic = true;
#pragma warning restore CS0649
        
        private PopupWindow _previousPopup;
        private bool _silentPopup;
        
        public static PopupWindow Opened { get; private set; }

        private void OnEnable()
        {
            if (!_silentPopup)
            {
                GetComponent<SoundHandler>().PlayOneShot();
            }
            else
            {
                _silentPopup = false;
            }
            
            if (Opened != null)
            {
                _previousPopup = Opened;
                Opened = this;
                _previousPopup.Close();
            }
            else
            {
                Opened = this;
            }


            if (lowpassMusic)
            {
                MusicMaster.Lowpass(true);
            }

            DimmerHandler.Switch(true);
        }

        private void OnDisable()
        {
            MusicMaster.Lowpass(false);
            DimmerHandler.Switch(false);
            
            if (Opened == this) Opened = null;
            if (_previousPopup is null) return;
            _previousPopup.Open(true);
            _previousPopup = null;
        }

        public void Open(bool silentPopup = false)
        {
            _silentPopup = silentPopup;
            gameObject.SetActive(true);
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }
    }
}