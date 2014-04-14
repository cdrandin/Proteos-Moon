using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class ComponentLister : EditorWindow
{
    private Dictionary<System.Type, List<Component>> sets = new Dictionary<System.Type, List<Component>>();
    private List<System.Type> sortedList = new List<System.Type>();
    private int totalObjects = 0;
    private Vector2 scrollPosition;
	private string selectedName;
	
    [MenuItem("Component/Component lister")]
    public static void Launch()
    {
        EditorWindow window = GetWindow(typeof(ComponentLister));
        window.Show();
    }

    public void UpdateList()
    {
        Object[] objects;

        sets.Clear();

        objects = FindObjectsOfType(typeof(Component));
        foreach (Component component in objects)
        {
            if (component==null)
            {
                Debug.Log("BULLLO ");
            }
            if (!sets.ContainsKey(component.GetType()))
            {
                sets[component.GetType()] = new List<Component>();
            }

            sets[component.GetType()].Add(component);
        }

        //Check for missing MonoBehaviours
        Transform[] transforms = (Transform[])FindObjectsOfType(typeof(Transform));
        System.Type theType = typeof(MonoBehaviour);
        foreach (Transform trans in transforms)
        {
            int i = 0;
            foreach (Component child in trans.GetComponents(typeof(Component)))
            {
                if (child == null)
                {
                    if (!sets.ContainsKey(theType))
                    {
                        sets[theType] = new List<Component>();
                    }
                    sets[theType].Add(child);
                    continue;
                }
                i++;
            }
        }

        sortedList = new List<System.Type>(sets.Keys);
        sortedList.Sort(new SystemTypeSorter());
        totalObjects = 0;
        foreach (System.Type bla in sortedList)
        {
            int occuranceCount = sets[bla].Count;
            totalObjects += occuranceCount;
        }
    }


    public void OnGUI()
    {
        GUILayout.BeginHorizontal(GUI.skin.GetStyle("Box"));
        GUILayout.Label("Components in scene:");
        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Refresh"))
        {
            UpdateList();
        }
        GUILayout.EndHorizontal();

        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

   
        
           
        foreach (System.Type bla in sortedList)
        {

            string typeName = bla.ToString();
            int occuranceCount = sets[bla].Count;
            System.Type sysTtype = bla;

            if (typeName == selectedName)
            {
				GUILayout.BeginHorizontal();
                GUILayout.Label(typeName + " (" + occuranceCount + "):");	
				if (GUILayout.Button("Hide", GUILayout.MaxWidth(40)))
				{
					selectedName = "";
				}				
				GUILayout.EndHorizontal();

                foreach (Component comp in sets[sysTtype])
				{
                    Behaviour beh = comp as Behaviour;
                    if (comp == null) continue;
                    if (!comp.gameObject.active) GUI.color = Color.red;
                    else if (beh!=null && !beh.enabled) GUI.color = Color.yellow;
                    else GUI.color = Color.white;
                    if (comp!=null) 
					{
                        GUILayout.BeginHorizontal();                       
                        if(GUILayout.Button(comp.name))
						    Selection.activeObject = comp;
                        if (GUILayout.Button("GameObject"))
                            Selection.activeObject = comp.gameObject;
                        GUILayout.EndHorizontal();
					}
				}
                GUI.color = Color.white;
			}else{
				GUILayout.BeginHorizontal();
                GUILayout.Label(typeName + " (" + occuranceCount + "):");	
				if (GUILayout.Button("Show", GUILayout.MaxWidth(40)))
				{
                    selectedName = typeName;
				}				
				GUILayout.EndHorizontal();
			}
        }

        GUILayout.EndScrollView();
        GUILayout.Label("Total components: "+totalObjects);	
    }
}


public class SystemTypeSorter : IComparer<System.Type>
{
    public int Compare(System.Type x, System.Type y)
    {
        return x.ToString().CompareTo(y.ToString());
    }
}