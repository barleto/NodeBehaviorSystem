using UnityEngine;
using System.Collections;

namespace BehaviorNode
{
    [System.Serializable]
    public class GameSwitch : ScriptableObject
    {

        public string name;
        public bool value;

        public GameSwitch(string name, bool value)
        {
            this.name = name;
            this.value = value;
            Debug.Log("Criado!");
        }

        // Use this for initialization
        void Start()
        {
            Debug.Log("Ainda estou aqui!");
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}

