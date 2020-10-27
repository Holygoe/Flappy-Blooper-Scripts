using UnityEngine;
using UnityEngine.UI;

namespace FlappyBlooper
{
    public class LoadSceneButton : MonoBehaviour
    {
        public GameScene scene;
        public bool slowLoad = true;
        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(() => SceneLoader.LoadScene(scene, slowLoad));
        }

        private void Start()
        {
            _button.interactable = !scene.IsLoaded();
        }

        private void OnEnable()
        {
            _button.interactable = !scene.IsLoaded();
        }
    }
}