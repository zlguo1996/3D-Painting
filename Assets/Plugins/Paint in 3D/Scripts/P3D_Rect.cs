[System.Serializable]
public struct P3D_Rect
{
	public int XMin;

	public int XMax;

	public int YMin;

	public int YMax;

	public bool IsSet
	{
		get
		{
			return XMin != XMax && YMin != YMax;
		}
	}

	public int Width
	{
		get
		{
			return XMax - XMin;
		}
	}

	public int Height
	{
		get
		{
			return YMax - YMin;
		}
	}

	public P3D_Rect(int newXMin, int newXMax, int newYMin, int newYMax)
	{
		XMin = newXMin;
		XMax = newXMax;
		YMin = newYMin;
		YMax = newYMax;
	}

	public void Set(int newXMin, int newXMax, int newYMin, int newYMax)
	{
		XMin = newXMin;
		XMax = newXMax;
		YMin = newYMin;
		YMax = newYMax;
	}

	public void Add(int newX, int newY)
	{
		Add(newX, newX + 1, newY, newY + 1);
	}

	public void Add(P3D_Rect rect)
	{
		Add(rect.XMin, rect.XMax, rect.YMin, rect.YMax);
	}

	public void Add(int newXMin, int newXMax, int newYMin, int newYMax)
	{
		if (Width == 0)
		{
			XMin = newXMin;
			XMax = newXMax;
		}
		else
		{
			if (newXMin < XMin) XMin = newXMin;
			if (newXMax > XMax) XMax = newXMax;
		}

		if (Height == 0)
		{
			YMin = newYMin;
			YMax = newYMax;
		}
		else
		{
			if (newYMin < YMin) YMin = newYMin;
			if (newYMax > YMax) YMax = newYMax;
		}
	}

	public bool Overlaps(P3D_Rect other)
	{
		if (IsSet == false || other.IsSet == false) return false;

		if (XMax <= other.XMin) return false;
		if (XMin >= other.XMax) return false;

		if (YMax <= other.YMin) return false;
		if (YMin >= other.YMax) return false;

		return true;
	}

	public void Clear()
	{
		XMin = 0;
		XMax = 0;
		YMin = 0;
		YMax = 0;
	}
}
