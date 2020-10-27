using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FlappyBlooper
{
    public class ButtonSound : MonoBehaviour, IPointerDownHandler
    {
        public enum Type { SoundOnPointerDown, SoundOnClick }

        public Type type;
        public AudioClipBundle bundle;
        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();

            if (type == Type.SoundOnClick)
            {
                button.onClick.AddListener(() => SoundMaster.PlayOneShot(bundle));
            }

            if (bundle == null)
            {
                bundle = SoundMaster.Instance.defaultButtonClick;
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (button.interactable && type == Type.SoundOnPointerDown)
            {
                SoundMaster.PlayOneShot(bundle);
            }
        }
    }
}