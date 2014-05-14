using UnityEngine;
using System.Collections;
 
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour {
	
    protected static T instance;
 
    //Returns the instance of this singleton
    public static T Instance {
		
        get {
			//if this does not have access the the instance:
            if (instance == null) {
				//try to find it
                instance = (T)FindObjectOfType(typeof(T));
				//if we can't find it:
                if (instance == null) {
					//create it
                    GameObject container = new GameObject();
					//name it
                    container.name = typeof(T)+"Container";
					//add the appropriate script
                    instance = (T)container.AddComponent(typeof(T));
                }
            }
            return instance;
        }
    }
}
