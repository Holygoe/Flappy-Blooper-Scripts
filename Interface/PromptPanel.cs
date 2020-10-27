using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace FlappyBlooper
{
    public class PromptPanel : MonoBehaviour
    {
        private const float DisplayTime = 3;
        
#pragma warning disable CS0649
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI numberText;
        [SerializeField] private Animator animator;
#pragma warning restore CS0649

        private readonly Queue<Consumable> _queue = new Queue<Consumable>();
        private bool _busy;

        public void OnEnable()
        {
            AssetTag.Consumables.ToAsset().OnItemChanged += WhenConsumableItemChanged;
        }

        private void OnDisable()
        {
            AssetTag.Consumables.ToAsset().OnItemChanged -= WhenConsumableItemChanged;
        }

        private void WhenConsumableItemChanged(object sender, ItemChangedEventArgs e)
        {
            if (!(e.Item is Consumable consumable)) return;
            _queue.Enqueue(consumable);
            if (!_busy)
            {
                StartCoroutine(DisplayValueAsync());    
            }
        }

        private IEnumerator DisplayValueAsync()
        {
            if (_queue.Count <= 0) yield break;
            
            _busy = true;
            var consumable = _queue.Dequeue();
            
            while (_queue.Count > 0 && consumable == _queue.Peek())
            {
                _queue.Dequeue();
            }
            
            iconImage.sprite = consumable.Icon;
            numberText.text = consumable.Stock.ToString();
            animator.SetTrigger(Triggers.Popup);
            yield return new WaitForSecondsRealtime(DisplayTime);
            animator.SetTrigger(Triggers.Popout);
            yield return new WaitForSecondsRealtime(0.5f);

            if (_queue.Count > 0)
            {
                StartCoroutine(DisplayValueAsync());
                yield break;
            }
            
            _busy = false;
        }
    }
}