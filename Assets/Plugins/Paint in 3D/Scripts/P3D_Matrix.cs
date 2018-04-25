using UnityEngine;

public struct P3D_Matrix
{
	public float m00; public float m10; public float m20;
	public float m01; public float m11; public float m21;
	public float m02; public float m12; public float m22;
	
	public static P3D_Matrix Identity
	{
		get
		{
			return new P3D_Matrix
			{
				m00 = 1.0f, m10 = 0.0f, m20 = 0.0f,
				m01 = 0.0f, m11 = 1.0f, m21 = 0.0f,
				m02 = 0.0f, m12 = 0.0f, m22 = 1.0f
			};
		}
	}

	public static P3D_Matrix Translation(float x, float y)
	{
		return new P3D_Matrix
		{
			m00 = 1.0f, m10 = 0.0f, m20 = 0.0f,
			m01 = 0.0f, m11 = 1.0f, m21 = 0.0f,
			m02 =    x, m12 =    y, m22 = 1.0f
		};
	}

	public static P3D_Matrix Scaling(float x, float y)
	{
		return new P3D_Matrix
		{
			m00 =    x, m10 = 0.0f, m20 = 0.0f,
			m01 = 0.0f, m11 =    y, m21 = 0.0f,
			m02 = 0.0f, m12 = 0.0f, m22 = 1.0f
		};
	}

	public static P3D_Matrix Rotation(float a)
	{
		var s = Mathf.Sin(a);
		var c = Mathf.Cos(a);

		return new P3D_Matrix
		{
			m00 =    c, m10 =   -s, m20 = 0.0f,
			m01 =    s, m11 =    c, m21 = 0.0f,
			m02 = 0.0f, m12 = 0.0f, m22 = 1.0f
		};
	}

	public P3D_Matrix Inverse
	{
		get
		{
			double determinant = +m00*(m11*m22-m21*m12)
			                     -m01*(m10*m22-m12*m20)
			                     +m02*(m10*m21-m11*m20);

			if (determinant != 0.0f)
			{
				float invdet = (float)(1.0 / determinant);

				return new P3D_Matrix
				{
					m00 =  (m11*m22-m21*m12)*invdet, m10 = -(m10*m22-m12*m20)*invdet, m20 =  (m10*m21-m20*m11)*invdet,
					m01 = -(m01*m22-m02*m21)*invdet, m11 =  (m00*m22-m02*m20)*invdet, m12 = -(m00*m12-m10*m02)*invdet,
					m02 =  (m01*m12-m02*m11)*invdet, m22 =  (m00*m11-m10*m01)*invdet, m21 = -(m00*m21-m20*m01)*invdet
				};
			}
			
			return Identity;
		}
	}

	public Matrix4x4 Matrix4x4
	{
		get
		{
			var o = Matrix4x4.identity;

			o.m00 = m00; o.m10 = m10; o.m20 = m20;
			o.m01 = m01; o.m11 = m11; o.m21 = m21;
			o.m02 = m02; o.m12 = m12; o.m22 = m22;

			return o;
		}
	}

	public Vector2 MultiplyPoint(Vector2 v)
	{
		var o = default(Vector2);

		o.x = m00 * v.x + m01 * v.y + m02;
		o.y = m10 * v.x + m11 * v.y + m12;

		return o;
	}

	public Vector2 MultiplyPoint(float x, float y)
	{
		var o = default(Vector2);

		o.x = m00 * x + m01 * y + m02;
		o.y = m10 * x + m11 * y + m12;

		return o;
	}

	public static P3D_Matrix operator *(P3D_Matrix lhs, P3D_Matrix rhs)
	{
		return new P3D_Matrix
		{
			m00 = lhs.m00 * rhs.m00 + lhs.m01 * rhs.m10 + lhs.m02 * rhs.m20,
			m01 = lhs.m00 * rhs.m01 + lhs.m01 * rhs.m11 + lhs.m02 * rhs.m21,
			m02 = lhs.m00 * rhs.m02 + lhs.m01 * rhs.m12 + lhs.m02 * rhs.m22,

			m10 = lhs.m10 * rhs.m00 + lhs.m11 * rhs.m10 + lhs.m12 * rhs.m20,
			m11 = lhs.m10 * rhs.m01 + lhs.m11 * rhs.m11 + lhs.m12 * rhs.m21,
			m12 = lhs.m10 * rhs.m02 + lhs.m11 * rhs.m12 + lhs.m12 * rhs.m22,

			m20 = lhs.m20 * rhs.m00 + lhs.m21 * rhs.m10 + lhs.m22 * rhs.m20,
			m21 = lhs.m20 * rhs.m01 + lhs.m21 * rhs.m11 + lhs.m22 * rhs.m21,
			m22 = lhs.m20 * rhs.m02 + lhs.m21 * rhs.m12 + lhs.m22 * rhs.m22
		};
	}
}