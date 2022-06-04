// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)
// Do not manually edit - this file is generated by MathfsCodegen.cs

using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Freya {

	/// <summary>An optimized uniform 2D Cubic bézier segment, with 4 control points</summary>
	[Serializable] public struct BezierCubic2D : IParamCubicSplineSegment2D {

		const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;

		/// <summary>Creates a uniform 2D Cubic bézier segment, from 4 control points</summary>
		/// <param name="p0">The starting point of the curve</param>
		/// <param name="p1">The second control point of the curve, sometimes called the start tangent point</param>
		/// <param name="p2">The third control point of the curve, sometimes called the end tangent point</param>
		/// <param name="p3">The end point of the curve</param>
		public BezierCubic2D( Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3 ) {
			( this.p0, this.p1, this.p2, this.p3 ) = ( p0, p1, p2, p3 );
			validCoefficients = false;
			curve = default;
		}

		Polynomial2D curve;
		public Polynomial2D Curve {
			get {
				ReadyCoefficients();
				return curve;
			}
		}
		#region Control Points

		[SerializeField] Vector2 p0, p1, p2, p3;

		/// <summary>The starting point of the curve</summary>
		public Vector2 P0 {
			[MethodImpl( INLINE )] get => p0;
			[MethodImpl( INLINE )] set => _ = ( p0 = value, validCoefficients = false );
		}

		/// <summary>The second control point of the curve, sometimes called the start tangent point</summary>
		public Vector2 P1 {
			[MethodImpl( INLINE )] get => p1;
			[MethodImpl( INLINE )] set => _ = ( p1 = value, validCoefficients = false );
		}

		/// <summary>The third control point of the curve, sometimes called the end tangent point</summary>
		public Vector2 P2 {
			[MethodImpl( INLINE )] get => p2;
			[MethodImpl( INLINE )] set => _ = ( p2 = value, validCoefficients = false );
		}

		/// <summary>The end point of the curve</summary>
		public Vector2 P3 {
			[MethodImpl( INLINE )] get => p3;
			[MethodImpl( INLINE )] set => _ = ( p3 = value, validCoefficients = false );
		}

		/// <summary>Get or set a control point position by index. Valid indices from 0 to 3</summary>
		public Vector2 this[ int i ] {
			get =>
				i switch {
					0 => P0,
					1 => P1,
					2 => P2,
					3 => P3,
					_ => throw new ArgumentOutOfRangeException( nameof(i), $"Index has to be in the 0 to 3 range, and I think {i} is outside that range you know" )
				};
			set {
				switch( i ) {
					case 0:
						P0 = value;
						break;
					case 1:
						P1 = value;
						break;
					case 2:
						P2 = value;
						break;
					case 3:
						P3 = value;
						break;
					default: throw new ArgumentOutOfRangeException( nameof(i), $"Index has to be in the 0 to 3 range, and I think {i} is outside that range you know" );
				}
			}
		}

		#endregion
		[NonSerialized] bool validCoefficients;

		[MethodImpl( INLINE )] void ReadyCoefficients() {
			if( validCoefficients )
				return; // no need to update
			validCoefficients = true;
			curve = CharMatrix.cubicBezier.GetCurve( p0, p1, p2, p3 );
		}
		public static bool operator ==( BezierCubic2D a, BezierCubic2D b ) => a.P0 == b.P0 && a.P1 == b.P1 && a.P2 == b.P2 && a.P3 == b.P3;
		public static bool operator !=( BezierCubic2D a, BezierCubic2D b ) => !( a == b );
		public bool Equals( BezierCubic2D other ) => P0.Equals( other.P0 ) && P1.Equals( other.P1 ) && P2.Equals( other.P2 ) && P3.Equals( other.P3 );
		public override bool Equals( object obj ) => obj is BezierCubic2D other && Equals( other );
		public override int GetHashCode() => HashCode.Combine( p0, p1, p2, p3 );

		public override string ToString() => $"({p0}, {p1}, {p2}, {p3})";
		/// <summary>Returns this spline segment in 3D, where z = 0</summary>
		/// <param name="curve2D">The 2D curve to cast to 3D</param>
		public static explicit operator BezierCubic3D( BezierCubic2D curve2D ) {
			return new BezierCubic3D( curve2D.p0, curve2D.p1, curve2D.p2, curve2D.p3 );
		}
		/// <summary>Returns a linear blend between two bézier curves</summary>
		/// <param name="a">The first spline segment</param>
		/// <param name="b">The second spline segment</param>
		/// <param name="t">A value from 0 to 1 to blend between <c>a</c> and <c>b</c></param>
		public static BezierCubic2D Lerp( BezierCubic2D a, BezierCubic2D b, float t ) =>
			new(
				Vector2.LerpUnclamped( a.p0, b.p0, t ),
				Vector2.LerpUnclamped( a.p1, b.p1, t ),
				Vector2.LerpUnclamped( a.p2, b.p2, t ),
				Vector2.LerpUnclamped( a.p3, b.p3, t )
			);

		/// <summary>Returns a linear blend between two bézier curves, where the tangent directions are spherically interpolated</summary>
		/// <param name="a">The first spline segment</param>
		/// <param name="b">The second spline segment</param>
		/// <param name="t">A value from 0 to 1 to blend between <c>a</c> and <c>b</c></param>
		public static BezierCubic2D Slerp( BezierCubic2D a, BezierCubic2D b, float t ) {
			Vector2 p0 = Vector2.LerpUnclamped( a.p0, b.p0, t );
			Vector2 p3 = Vector2.LerpUnclamped( a.p3, b.p3, t );
			return new BezierCubic2D(
				p0,
				p0 + (Vector2)Vector3.SlerpUnclamped( a.p1 - a.p0, b.p1 - b.p0, t ),
				p3 + (Vector2)Vector3.SlerpUnclamped( a.p2 - a.p3, b.p2 - b.p3, t ),
				p3
			);
		}

		/// <summary>Splits this curve at the given t-value, into two curves of the exact same shape</summary>
		/// <param name="t">The t-value along the curve to sample</param>
		public (BezierCubic2D pre, BezierCubic2D post) Split( float t ) {
			Vector2 a = new Vector2(
				P0.x + ( P1.x - P0.x ) * t,
				P0.y + ( P1.y - P0.y ) * t );
			float bx = P1.x + ( P2.x - P1.x ) * t;
			float by = P1.y + ( P2.y - P1.y ) * t;
			Vector2 c = new Vector2(
				P2.x + ( P3.x - P2.x ) * t,
				P2.y + ( P3.y - P2.y ) * t );
			Vector2 d = new Vector2(
				a.x + ( bx - a.x ) * t,
				a.y + ( by - a.y ) * t );
			Vector2 e = new Vector2(
				bx + ( c.x - bx ) * t,
				by + ( c.y - by ) * t );
			Vector2 p = new Vector2(
				d.x + ( e.x - d.x ) * t,
				d.y + ( e.y - d.y ) * t );
			return ( new BezierCubic2D( P0, a, d, p ), new BezierCubic2D( p, e, c, P3 ) );
		}

		public UBSCubic2D ToUniformCubicBSpline() {
			// todo: channel split for performance
			return new UBSCubic2D(
				6 * p0 - 7 * p1 + 2 * p2,
				2 * p1 - p2,
				-p1 + 2 * p2,
				2 * p1 - 7 * p2 + 6 * p3 );
		}

		public CatRomCubic2D ToUniformCubicCatRom() {
			// todo: channel split for performance
			return new CatRomCubic2D(
				6 * p0 - 6 * p1 + p3,
				p0,
				p3,
				p0 - 6 * p2 + 6 * p3 );
		}

		public HermiteCubic2D ToHermite() {
			// todo: channel split for performance
			return new HermiteCubic2D( p0, ( p1 - p0 ) * 3, p3, ( p3 - p2 ) * 3 );
		}
	}
}
