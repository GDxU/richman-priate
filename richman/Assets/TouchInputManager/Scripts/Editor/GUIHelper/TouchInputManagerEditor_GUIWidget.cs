using UnityEngine;
using System.Collections;
using System;
public class TouchInputManagerEditor_GUIWidget {

	protected readonly Action Repaint;
	
	public TouchInputManagerEditor_GUIWidget(Action repaintFn)
	{
		Repaint = repaintFn;
	}
	
}
