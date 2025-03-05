using System;
using System.Numerics;
using SpaceDefence.Collision;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace SpaceDefence
{

    public class LinePieceCollider : Collider, IEquatable<LinePieceCollider>
    {

        public Vector2 Start;
        public Vector2 End;

        /// <summary>
        /// The length of the LinePiece, changing the length moves the end vector to adjust the length.
        /// </summary>
        public float Length 
        { 
            get { 
                return (End - Start).Length(); 
            } 
            set {
                End = Start + GetDirection() * value; 
            }
        }

        /// <summary>
        /// The A component from the standard line formula Ax + By + C = 0
        /// </summary>
        public float StandardA
        {
            get
            {
                return End.Y - Start.Y;
            }
        }

        /// <summary>
        /// The B component from the standard line formula Ax + By + C = 0
        /// </summary>
        public float StandardB
        {
            get
            {
                return Start.X - End.X;
            }
        }

        /// <summary>
        /// The C component from the standard line formula Ax + By + C = 0
        /// </summary>
        public float StandardC
        {
            get
            {
                return End.X * Start.Y - Start.X * End.Y;
            }
        }

        public LinePieceCollider(Vector2 start, Vector2 end)
        {
            Start = start;
            End = end;
        }
        
        public LinePieceCollider(Vector2 start, Vector2 direction, float length)
        {
            Start = start;
            End = start + direction * length;
        }

        /// <summary>
        /// Should return the angle between a given direction and the up vector.
        /// </summary>
        /// <param name="direction">The Vector2 pointing out from (0,0) to calculate the angle to.</param>
        /// <returns> The angle in radians between the up vector and the direction to the cursor.</returns>
        public static float GetAngle(Vector2 direction)
        {
            float angle = (float)Math.Atan2(direction.Y, direction.X);
            angle += MathHelper.PiOver2;
            return MathHelper.WrapAngle(angle);
        }


        /// <summary>
        /// Calculates the normalized vector pointing from point1 to point2
        /// </summary>
        /// <returns> A Vector2 containing the direction from point1 to point2. </returns>
        public static Vector2 GetDirection(Vector2 point1, Vector2 point2)
        {
            return Vector2.Normalize(point2 - point1);
        }


        /// <summary>
        /// Gets whether or not the Line intersects another Line
        /// </summary>
        /// <param name="other">The Line to check for intersection</param>
        /// <returns>true there is any overlap between the Circle and the Line.</returns>
        public override bool Intersects(LinePieceCollider other)
        {
            float denom = (StandardA * other.StandardB) - (other.StandardA * StandardB);
            
            if (Math.Abs(denom) < 0.001f)
                return false;
            
            float x = ((other.StandardB * StandardC) - (StandardB * other.StandardC)) / denom;
            float y = ((StandardA * other.StandardC) - (other.StandardA * StandardC)) / denom;
            Vector2 intersectionPoint = new Vector2(x, y);
            
            return Contains(intersectionPoint) && other.Contains(intersectionPoint);
        }


        /// <summary>
        /// Gets whether or not the line intersects a Circle.
        /// </summary>
        /// <param name="other">The Circle to check for intersection.</param>
        /// <returns>true there is any overlap between the two Circles.</returns>
        public override bool Intersects(CircleCollider other)
        {
            Vector2 nearestPoint = NearestPointOnLine(other.Center);
            return (nearestPoint - other.Center).Length() <= other.Radius;
        }

        /// <summary>
        /// Gets whether or not the Line intersects the Rectangle.
        /// </summary>
        /// <param name="other">The Rectangle to check for intersection.</param>
        /// <returns>true there is any overlap between the Circle and the Rectangle.</returns>
        public override bool Intersects(RectangleCollider other)
        {
            Vector2[] corners = new Vector2[]
            {
                new Vector2(other.shape.Left, other.shape.Top),     // Top-left
                new Vector2(other.shape.Right, other.shape.Top),    // Top-right
                new Vector2(other.shape.Left, other.shape.Bottom),  // Bottom-left
                new Vector2(other.shape.Right, other.shape.Bottom)  // Bottom-right
            };
            
            if (other.Contains(Start) || other.Contains(End))
                return true;
            
            for (int i = 0; i < corners.Length; i++)
            {
                LinePieceCollider rectangleEdge = new LinePieceCollider(
                    corners[i], 
                    corners[(i + 1) % corners.Length]
                );

                if (Intersects(rectangleEdge))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Calculates the intersection point between 2 lines.
        /// </summary>
        /// <param name="Other">The line to intersect with</param>
        /// <returns>A Vector2 with the point of intersection.</returns>
        public Vector2 GetIntersection(LinePieceCollider Other)
        {
            float denom = (StandardA * Other.StandardB) - (Other.StandardA * StandardB);
            
            if (Math.Abs(denom) < 0.001f)
                return Vector2.Zero;
            
            float x = ((Other.StandardB * StandardC) - (StandardB * Other.StandardC)) / denom;
            float y = ((StandardA * Other.StandardC) - (Other.StandardA * StandardC)) / denom;
            
            return new Vector2(x, y);
        }

        /// <summary>
        /// Finds the nearest point on a line to a given vector, taking into account if the line is .
        /// </summary>
        /// <param name="other">The Vector you want to find the nearest point to.</param>
        /// <returns>The nearest point on the line.</returns>
        public Vector2 NearestPointOnLine(Vector2 other)
        {
            var pq = Start - End;
            var pc = other - End;
            var dist = Vector2.Dot(pq, pc) / pq.LengthSquared();
            dist = Math.Clamp(dist, 0, 1);
            return End + pq * dist;
        }

        /// <summary>
        /// Returns the enclosing Axis Aligned Bounding Box containing the control points for the line.
        /// As an unbound line has infinite length, the returned bounding box assumes the line to be bound.
        /// </summary>
        /// <returns></returns>
        public override Rectangle GetBoundingBox()
        {
            Point topLeft = new Point((int)Math.Min(Start.X, End.X), (int)Math.Min(Start.Y, End.Y));
            Point size = new Point((int)Math.Max(Start.X, End.X), (int)Math.Max(Start.Y, End.X)) - topLeft;
            return new Rectangle(topLeft,size);
        }


        /// <summary>
        /// Gets whether or not the provided coordinates lie on the line.
        /// </summary>
        /// <param name="coordinates">The coordinates to check.</param>
        /// <returns>true if the coordinates are within the circle.</returns>
        public override bool Contains(Vector2 coordinates)
        {
            float lineEquationValue = Math.Abs(StandardA * coordinates.X + StandardB * coordinates.Y + StandardC);
            bool onLine = lineEquationValue < 0.001f;
            
            float distanceToStart = (coordinates - Start).Length();
            float distanceToEnd = (coordinates - End).Length();
            float lineLength = Length;

            return onLine && (distanceToStart + distanceToEnd <= lineLength + 0.001f);
        }

        public bool Equals(LinePieceCollider other)
        {
            return other.Start == this.Start && other.End == this.End;
        }

        /// <summary>
        /// Calculates the normalized vector pointing from point1 to point2
        /// </summary>
        /// <returns> A Vector2 containing the direction from point1 to point2. </returns>
        public static Vector2 GetDirection(Point point1, Point point2)
        {
            return GetDirection(point1.ToVector2(), point2.ToVector2());
        }


        /// <summary>
        /// Calculates the normalized vector pointing from point1 to point2
        /// </summary>
        /// <returns> A Vector2 containing the direction from point1 to point2. </returns>
        public Vector2 GetDirection()
        {
            return GetDirection(Start, End);
        }


        /// <summary>
        /// Should return the angle between a given direction and the up vector.
        /// </summary>
        /// <param name="direction">The Vector2 pointing out from (0,0) to calculate the angle to.</param>
        /// <returns> The angle in radians between the the up vector and the direction to the cursor.</returns>
        public float GetAngle()
        {
            return GetAngle(GetDirection());
        }
    }
}
