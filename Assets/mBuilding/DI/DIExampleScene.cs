using UnityEngine;

namespace mBuilding.DI
{
    public class DIExampleScene : MonoBehaviour
    {
        public void Init(DIContainer projectContainer)
        {
            //используем уровень проекта, чтобы вытащить то, что нам нужно
            // var serviceWithoutTag = projectContainer.Resolve<MyProjectService>();
            // var service1 = projectContainer.Resolve<MyProjectService>("option_1");
            // var service2 = projectContainer.Resolve<MyProjectService>("option_2");

            var sceneContainer = new DIContainer(projectContainer);
            sceneContainer.RegisterSingleton(c => new MySceneService(c.Resolve<MyProjectService>()));
            sceneContainer.RegisterSingleton(c => new MyFactory());
            sceneContainer.RegisterInstance(new MyObject("instance", 10));

            var objectsFactory = sceneContainer.Resolve<MyFactory>();
            for (int i = 0; i < 3; i++)
            {
                var id = $"o{i}";
                var o = objectsFactory.CreateInstance(id, i);
                Debug.Log($"Object created with factory. \n {o.ToString()}");
            }
        }
    }
}