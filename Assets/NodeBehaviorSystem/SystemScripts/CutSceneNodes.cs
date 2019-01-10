using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class CutSceneNode : ScriptableObject{

	private bool _hasExecutionEnded = false;
	[SerializeField]
	public CutScene cutScene;

	#if UNITY_EDITOR
	/*In this function, you define what will appear in the UI of the CutScene.
	Just populate with GUILayout funcitons*/
	virtual public void createUIDescription(CutScene cutScene,SerializedObject serializedObject){

	}
#endif

	//executed once to initialize the node
	virtual public void start(){
		//cutScene.css.toggleUIVisibility (true);
	}

	//executed each frame
	virtual public void update(){
	}

	//executed when the node finished executing. Finalize thng if you want to.
	virtual public void end(){
		//cutScene.css.toggleUIVisibility (false);
	}

	//executed each time the chatbox of the cutscene system is taped/clicked
	virtual public void tapAtScreen(){
		EndNodeExecution();
	}

    public void EndNodeExecution()
    {
        _hasExecutionEnded = true;
    }

    public bool HasExecutionEnded()
    {
        return _hasExecutionEnded;
    }
}


