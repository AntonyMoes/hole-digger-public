using _Game.Scripts.UI;
using UnityEngine;

namespace _Game.Scripts.Data.Configs.Works.UI {
    [CreateAssetMenu(menuName = Configs.UIWorkMenuItem + nameof(DefaultUIWorkConfig), fileName = nameof(DefaultUIWorkConfig))]
    public class DefaultUIWorkConfig : SimpleUIWorkConfig<DefaultUIPresenter, UIView> { }
}