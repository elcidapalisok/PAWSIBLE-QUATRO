// AnatomyManager.cs
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class AnatomyManager : MonoBehaviour
{
    public static AnatomyManager Instance { get; private set; }
    
    [System.Serializable]
    public class AnatomyObject
    {
        public string objectName; // Unique name to identify the object
        public GameObject sceneObject; // The actual GameObject in the scene
        public SceneObjectHighlighter highlighter; // Cached highlighter component
        public string category; // "Skeletal", "Muscular", or "Visceral"
        public AnatomyIconToggle associatedIcon; // Reference to the UI icon
    }
    
    [Header("Anatomy Objects")]
    public List<AnatomyObject> allAnatomyObjects = new List<AnatomyObject>();
    
    private Dictionary<string, AnatomyObject> objectDictionary = new Dictionary<string, AnatomyObject>();
    private AnatomyObject currentlyHighlightedObject = null;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeDictionary();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void InitializeDictionary()
    {
        objectDictionary.Clear();
        foreach (var obj in allAnatomyObjects)
        {
            if (obj.sceneObject != null && !string.IsNullOrEmpty(obj.objectName))
            {
                // Cache the highlighter component
                obj.highlighter = obj.sceneObject.GetComponent<SceneObjectHighlighter>();
                if (obj.highlighter == null)
                {
                    obj.highlighter = obj.sceneObject.GetComponentInChildren<SceneObjectHighlighter>();
                }
                
                objectDictionary[obj.objectName] = obj;
            }
        }
    }
    
    public void HighlightObject(string objectName)
    {
        // Remove current highlight if any
        if (currentlyHighlightedObject != null && currentlyHighlightedObject.highlighter != null)
        {
            currentlyHighlightedObject.highlighter.RemoveHighlight();
            
            // Deselect the associated icon
            if (currentlyHighlightedObject.associatedIcon != null)
            {
                currentlyHighlightedObject.associatedIcon.DeselectIcon();
            }
        }
        
        // Highlight the requested object
        if (objectDictionary.TryGetValue(objectName, out AnatomyObject anatomyObj))
        {
            if (anatomyObj.highlighter != null)
            {
                anatomyObj.highlighter.Highlight();
                currentlyHighlightedObject = anatomyObj;
                
                // Select the associated icon
                if (anatomyObj.associatedIcon != null)
                {
                    anatomyObj.associatedIcon.SelectIcon();
                }
            }
            else
            {
                Debug.LogWarning($"No highlighter found on {objectName}");
            }
        }
        else
        {
            Debug.LogWarning($"Anatomy object not found: {objectName}");
        }
    }
    
    public void RemoveAllHighlights()
    {
        foreach (var kvp in objectDictionary)
        {
            if (kvp.Value.highlighter != null)
            {
                kvp.Value.highlighter.RemoveHighlight();
            }
            
            // Deselect associated icons
            if (kvp.Value.associatedIcon != null)
            {
                kvp.Value.associatedIcon.DeselectIcon();
            }
        }
        
        currentlyHighlightedObject = null;
    }
    
    public List<AnatomyObject> GetObjectsByCategory(string category)
    {
        return allAnatomyObjects.FindAll(obj => obj.category == category);
    }
    
    public AnatomyObject GetAnatomyObject(string objectName)
    {
        if (objectDictionary.TryGetValue(objectName, out AnatomyObject obj))
        {
            return obj;
        }
        return null;
    }
    
    // Call this when an icon is instantiated to link it with its object
    public void RegisterIcon(string objectName, AnatomyIconToggle icon)
    {
        if (objectDictionary.TryGetValue(objectName, out AnatomyObject anatomyObj))
        {
            anatomyObj.associatedIcon = icon;
        }
    }
}