using System;
using UnityEngine;

namespace mBuilding.DI
{
    public class MyProjectService{}

    public class MySceneService
    {
        private readonly MyProjectService _myProjectService;
        public MySceneService(MyProjectService myProjectService)
        {
            _myProjectService = myProjectService;

        }
    }

    public class MyFactory
    {
        public MyObject CreateInstance(string id, int par1)
        {
            return new MyObject(id, par1);
        }
    }

    public class MyObject
    {
        private readonly string _id;
        private readonly int _par1;
        public MyObject(string id, int par1)
        {
            _id = id;
            _par1 = par1;
        }

        public override string ToString()
        {
            return $"object with id: {_id} and param: {_par1}";
        }
    }

    public class DIExampleProject:MonoBehaviour
    {
        private void Awake()
        {
            var projectContainer = new DIContainer();
            projectContainer.RegisterSingleton(_=>new MyProjectService());
            projectContainer.RegisterSingleton("option_1",_=>new MyProjectService());
            projectContainer.RegisterSingleton("option_2",_=>new MyProjectService());


            var sceneRoot = FindFirstObjectByType<DIExampleScene>();
            sceneRoot.Init(projectContainer);
        }
    }
}