using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellUIController<T> : MonoBehaviour where T : CellUI
{
    public Transform CellListContainer;
    public T CellTemplate;
	protected List<T> CellTemplateList = new List<T>();

    [HideInInspector]
    public List<T> CellsList = new List<T>();

    public bool DidInit
    {
        get
        {
            return didInit;
        }
    }

    protected bool didInit = false;


    protected virtual void OnEnable()
    {

    }
    
    protected virtual void Start()
    {
		didInit = false;
        if (CellListContainer == null)
        {
            CellListContainer = transform;
        }
    }

    protected virtual void Update()
    {

    }

    protected virtual void Init()
    {
        CellTemplateList = new List<T>(CellListContainer.GetComponentsInChildren<T>());

        if (CellTemplate == null)
        {
            if (CellTemplateList.Count == 0)
            {
                Debug.LogError("Should setup cell prefabs : " + typeof(T));
            }
            else
            {
                CellTemplate = CellTemplateList[0];
            }
        }
        
		didInit = true;
    }

    public virtual T AddCell()
	{
		if (!didInit)
		{
			Init();
		}

        GameObject cell = Instantiate(CellTemplate.gameObject, CellListContainer);
        cell.SetActive(true);
        T cellUI = cell.GetComponent<T>();
        cellUI.Init();
		cellUI.MyController = this;
        CellsList.Add(cellUI);

        foreach (var item in CellTemplateList)
        {
            item.transform.SetSiblingIndex(item.transform.parent.childCount - 1);
            item.gameObject.SetActive(false);
        }

		return cellUI;
    }

	public virtual void ClearList()
	{
		foreach (var item in CellsList)
		{
			Destroy(item.gameObject);
		}
		CellsList.Clear();
	}
}
