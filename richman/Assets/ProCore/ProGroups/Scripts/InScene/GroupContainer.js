//#pragma strict

class Group
{
	var name : String;
	var objects : GameObject[];
	var frozen : boolean;
	var hidden : boolean;
}

var sceneGroups : Group[];

function NewGroup(theObjects : GameObject[], newGroupName : String)
{
	var newGroup = Group();
	newGroup.name = newGroupName;
	newGroup.objects = theObjects;
	newGroup.frozen = false;
	newGroup.hidden = false;
	
	if(sceneGroups != null)
	{
		var tempGroups = new Array(sceneGroups);
		tempGroups.Add(newGroup);
		sceneGroups = tempGroups.ToBuiltin(Group);
	}
	else
	{
		sceneGroups = new Group[1];
		sceneGroups[0] = newGroup;
	}
}

function ToggleFreeze(theGroup : Group)
{
	CleanGroup(theGroup);
	
	theGroup.frozen = !theGroup.frozen;
	for(var theObject : GameObject in theGroup.objects)
	{
		if(theGroup.frozen)
			theObject.hideFlags = HideFlags.NotEditable;
		else
		{
			if(!theGroup.hidden)
				theObject.hideFlags = 0;
		}
	}
}

function ToggleVis(theGroup : Group)
{
	if(theGroup.hidden)
	{
		ShowGroup(theGroup);
	}
	else
	{
		HideGroup(theGroup);
	}
}

function HideGroup(theGroup : Group)
{
	CleanGroup(theGroup);
	for(var theObject : GameObject in theGroup.objects)
	{
		theObject.hideFlags = HideFlags.NotEditable;
		#if UNITY_3_5
		theObject.active = false;
		#else
		theObject.SetActive(false);
		#endif
	}
	
	theGroup.hidden = true;
}

function ShowGroup(theGroup : Group)
{
	CleanGroup(theGroup);
	for(var theObject : GameObject in theGroup.objects)
	{
		if(!theGroup.frozen)
			theObject.hideFlags = 0;
			
		#if UNITY_3_5
		theObject.active = true;
		#else
		theObject.SetActive(true);
		#endif
	}
	
	theGroup.hidden = false;
}

function Isolate(i : int)
{
	for(var g : int = 0; g<sceneGroups.length;g++)
	{
		if(g!=i)
		{
			HideGroup(sceneGroups[g]);
		}
	}
	ShowGroup(sceneGroups[i]);
}

function RemoveGroup(i : int)
{
	CleanGroup(sceneGroups[i]);
	
	for(var theObject : GameObject in sceneGroups[i].objects)
	{
		#if UNITY_3_5
		theObject.active = true;
		#else
		theObject.SetActive(true);
		#endif
		theObject.hideFlags = 0;
	}
	
	var tempGroups = new Array(sceneGroups);
	tempGroups.RemoveAt(i);
	sceneGroups = tempGroups.ToBuiltin(Group);
}

function UpdateGroup(theGroup : Group, objects : GameObject[])
{
	CleanGroup(theGroup);
	
	for(var theObject : GameObject in theGroup.objects)
	{
		#if UNITY_3_5
		theObject.active = true;
		#else
		theObject.SetActive(true);
		#endif
		theObject.hideFlags = 0;
	}
	
	theGroup.objects = objects;
	theGroup.hidden = false;
	theGroup.frozen = false;
}

function MoveGroupUp(shiftIndex : int)
{
	var tempGroups = new Array();
	for(var i:int=0;i<shiftIndex-1;i++)
	{
		tempGroups.Add(sceneGroups[i]);
	}
	tempGroups.Add(sceneGroups[shiftIndex]);
	for(var o:int=shiftIndex-1;o<sceneGroups.length;o++)
	{
		if(o != shiftIndex)
		{
			tempGroups.Add(sceneGroups[o]);
		}
	}	
	sceneGroups = tempGroups.ToBuiltin(Group); 
}

function CleanGroup(theGroup : Group)
{
	var tempObjects = new Array();
	
	for(var theObject : GameObject in theGroup.objects)
	{
		if(theObject != null)
			tempObjects.Add(theObject);
	}
	
	theGroup.objects = tempObjects.ToBuiltin(GameObject);
}